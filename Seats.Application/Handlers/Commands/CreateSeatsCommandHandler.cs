using FluentValidation;
using MediatR;
using Seats.Application.Commands;
using Seats.Core.RabbitMQ;
using Seats.Core.Dtos;
using Seats.Domain.ValueObjects;

using Seats.Core.Repositories;
using Seats.Core.Services;
using Seats.Core.Exceptions;


namespace Seats.Application.Handlers.Commands
{
    public class CreateSeatsCommandHandler : IRequestHandler<CreateSeatsCommand>
    {
        private readonly IEventBus<CreateSeatDto> _eventBus;
        private readonly IValidator<CreateSeatsCommand> _validator;
        private readonly ISeatRepository _repository;
        private readonly IEventService _eventService;

        public CreateSeatsCommandHandler(IEventBus<CreateSeatDto> eventBus, IValidator<CreateSeatsCommand> validator, ISeatRepository repository, IEventService eventService)
        {
            _eventBus = eventBus;
            _validator = validator;
            _repository = repository;
            _eventService = eventService;
        }

        public async Task Handle(CreateSeatsCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

             var zone = await _eventService.GetZoneByIdAsync(request.SeatsDto.ZoneId);
             if (zone == null)
             {
                 throw new EventZoneException(request.SeatsDto.ZoneId);
             }
 
             var currentSeatCount = await _repository.GetSeatCountByZoneAsync(
                 Domain.ValueObjects.ZoneId.Create(request.SeatsDto.EventId), 
                 Domain.ValueObjects.EventId.Create(request.SeatsDto.EventId), // WARNING: This looked like a bug in my thought process, wait.
                 // Actually I need to match correct IDs.
                 // ZoneId is ZoneId
                 // EventId is EventId
                 Domain.ValueObjects.FunctionId.Create(request.SeatsDto.FunctionId),
                 Domain.ValueObjects.VenueId.Create(request.SeatsDto.VenueId)
             );

            // Wait, I put EventId into ZoneId param above by mistake?
            // Let's re-read GetSeatCountByZoneAsync signature: (ZoneId, EventId, FunctionId, VenueId)
            // Re-doing the tool call below properly.
            
             currentSeatCount = await _repository.GetSeatCountByZoneAsync(
                 Domain.ValueObjects.ZoneId.Create(request.SeatsDto.ZoneId),
                 Domain.ValueObjects.EventId.Create(request.SeatsDto.EventId),
                 Domain.ValueObjects.FunctionId.Create(request.SeatsDto.FunctionId),
                 Domain.ValueObjects.VenueId.Create(request.SeatsDto.VenueId)
             );

             if (currentSeatCount + request.SeatsDto.Quantity > zone.Capacity)
             {
                 throw new EventZoneException(zone.Capacity);
             }

            var lastNumber = await _repository.GetLastSeatNumberAsync(
                EventId.Create(request.SeatsDto.EventId),
                FunctionId.Create(request.SeatsDto.FunctionId),
                ZoneId.Create(request.SeatsDto.ZoneId),
                VenueId.Create(request.SeatsDto.VenueId)
            );

            for (int i = 1; i <= request.SeatsDto.Quantity; i++)
            {
                var seatId = SeatId.New();

                var createSeatMessage = new CreateSeatDto
                {
                    SeatId = seatId.Value,
                    EventId = request.SeatsDto.EventId,
                    FunctionId = request.SeatsDto.FunctionId,
                    ZoneId = request.SeatsDto.ZoneId,
                    VenueId = request.SeatsDto.VenueId,
                    Number = lastNumber + i
                };

                await _eventBus.PublishMessageAsync(createSeatMessage, "seats_queue", "CreateSeat");
            }
        }
    }
}
