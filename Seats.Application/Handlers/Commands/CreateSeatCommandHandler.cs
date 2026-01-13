using FluentValidation;
using MediatR;
using Seats.Application.Commands;
using Seats.Core.RabbitMQ;
using Seats.Domain.ValueObjects;
using Seats.Core.Dtos;
using Seats.Core.Repositories;
using Seats.Core.Services;
using Seats.Core.Exceptions;




namespace Seats.Application.Handlers.Commands
{
    public class CreateSeatCommandHandler : IRequestHandler<CreateSeatCommand, Guid>
    {
        private readonly IEventBus<CreateSeatDto> _eventBus;
        private readonly IValidator<CreateSeatCommand> _validator;
        private readonly ISeatRepository _repository;
        private readonly IUserLogService _userLogService;
        private readonly IEventService _eventService;

        public CreateSeatCommandHandler(IEventBus<CreateSeatDto> eventBus, IValidator<CreateSeatCommand> validator, ISeatRepository repository, IUserLogService userLogService, IEventService eventService)
        {
            _eventBus = eventBus;
            _validator = validator;
            _repository = repository;
            _userLogService = userLogService;
            _eventService = eventService;
        }

        public async Task<Guid> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var zone = await _eventService.GetZoneByIdAsync(request.Seat.ZoneId);
            if (zone == null)
            {
                throw new EventZoneException(request.Seat.ZoneId);
            }

            var currentSeatCount = await _repository.GetSeatCountByZoneAsync(
                ZoneId.Create(request.Seat.ZoneId),
                EventId.Create(request.Seat.EventId),
                FunctionId.Create(request.Seat.FunctionId),
                VenueId.Create(request.Seat.VenueId)
            );

            if (currentSeatCount + 1 > zone.Capacity)
            {
                throw new EventZoneException(zone.Capacity);
            }

            var lastNumber = await _repository.GetLastSeatNumberAsync(
                EventId.Create(request.Seat.EventId),
                FunctionId.Create(request.Seat.FunctionId),
                ZoneId.Create(request.Seat.ZoneId),
                VenueId.Create(request.Seat.VenueId)
            );

            var seatId = SeatId.New();

            var createSeatMessage = new CreateSeatDto
            {
                SeatId = seatId.Value,
                EventId = request.Seat.EventId,
                FunctionId = request.Seat.FunctionId,
                ZoneId = request.Seat.ZoneId,
                VenueId = request.Seat.VenueId,
                Number = lastNumber + 1
            };

            await _eventBus.PublishMessageAsync(createSeatMessage, "seats_queue", "CreateSeat");

            return seatId.Value;
        }
    }
}
