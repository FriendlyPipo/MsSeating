using FluentValidation;
using MediatR;
using Seats.Application.Commands;
using Seats.Application.Dtos;
using Seats.Core.Repositories;
using Seats.Domain.ValueObjects;
using Seats.Core.RabbitMQ;

namespace Seats.Application.Handlers.Commands
{
    public class DeleteSeatsCommandHandler : IRequestHandler<DeleteSeatsCommand>
    {
        private readonly ISeatRepository _repository;
        private readonly IEventBus<DeleteSeatDto> _eventBus;
        private readonly IValidator<DeleteSeatsCommand> _validator;

        public DeleteSeatsCommandHandler(ISeatRepository repository, IEventBus<DeleteSeatDto> eventBus, IValidator<DeleteSeatsCommand> validator)
        {
            _repository = repository;
            _eventBus = eventBus;
            _validator = validator;
        }

        public async Task Handle(DeleteSeatsCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var seats = request.DeleteDto;

            var specificSeat = await _repository.GetSpecificSeatsAsync(
                EventId.Create(seats.EventId),
                FunctionId.Create(seats.FunctionId),
                ZoneId.Create(seats.ZoneId),
                VenueId.Create(seats.VenueId),
                SeatRow.Create(seats.Row)
            );

            var seatsToDelete = specificSeat
                .OrderByDescending(s => s.Number.Value)
                .Take(seats.Quantity)
                .Select(s => s.SeatId)
                .ToList();

            foreach (var seatId in seatsToDelete)
            {
                var deleteSeatMessage = new DeleteSeatDto 
                { 
                    SeatId = seatId.Value,
                    EventId = seats.EventId,
                    FunctionId = seats.FunctionId,
                    ZoneId = seats.ZoneId,
                    VenueId = seats.VenueId 
                };
                
                await _eventBus.PublishMessageAsync(deleteSeatMessage, "seats_queue", "DeleteSeat");
            }
        }
    }
}
