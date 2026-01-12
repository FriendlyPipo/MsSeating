using FluentValidation;
using MediatR;
using Seats.Application.Commands;
using Seats.Core.RabbitMQ;
using Seats.Application.Dtos;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Commands
{
    public class CreateSeatsCommandHandler : IRequestHandler<CreateSeatsCommand>
    {
        private readonly IEventBus<CreateSeatDto> _eventBus;
        private readonly IValidator<CreateSeatsCommand> _validator;

        public CreateSeatsCommandHandler(IEventBus<CreateSeatDto> eventBus, IValidator<CreateSeatsCommand> validator)
        {
            _eventBus = eventBus;
            _validator = validator;
        }

        public async Task Handle(CreateSeatsCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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
                    Row = request.SeatsDto.Row,
                    Number = i
                };

                await _eventBus.PublishMessageAsync(createSeatMessage, "seats_queue", "CreateSeat");
            }
        }
    }
}
