using FluentValidation;
using MediatR;
using Seats.Application.Commands;
using Seats.Core.RabbitMQ;
using Seats.Domain.ValueObjects;
using Seats.Application.Dtos;

namespace Seats.Application.Handlers.Commands
{
    public class CreateSeatCommandHandler : IRequestHandler<CreateSeatCommand, Guid>
    {
        private readonly IEventBus<CreateSeatDto> _eventBus;
        private readonly IValidator<CreateSeatCommand> _validator;

        public CreateSeatCommandHandler(IEventBus<CreateSeatDto> eventBus, IValidator<CreateSeatCommand> validator)
        {
            _eventBus = eventBus;
            _validator = validator;
        }

        public async Task<Guid> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var seatId = SeatId.New();

            var createSeatMessage = new CreateSeatDto
            {
                SeatId = seatId.Value,
                EventId = request.Seat.EventId,
                FunctionId = request.Seat.FunctionId,
                ZoneId = request.Seat.ZoneId,
                VenueId = request.Seat.VenueId,
                Row = request.Seat.Row,
                Number = request.Seat.Number
            };

            await _eventBus.PublishMessageAsync(createSeatMessage, "seats_queue", "CreateSeat");

            return seatId.Value;
        }
    }
}
