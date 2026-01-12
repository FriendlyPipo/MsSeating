using FluentValidation;
using MediatR;
using Seats.Application.Commands;
using Seats.Core.RabbitMQ;
using Seats.Application.Dtos;

namespace Seats.Application.Handlers.Commands
{
    public class DeleteSeatCommandHandler : IRequestHandler<DeleteSeatCommand>
    {
        private readonly IEventBus<DeleteSeatDto> _eventBus;
        private readonly IValidator<DeleteSeatCommand> _validator;

        public DeleteSeatCommandHandler(IEventBus<DeleteSeatDto> eventBus, IValidator<DeleteSeatCommand> validator)
        {
            _eventBus = eventBus;
            _validator = validator;
        }

        public async Task Handle(DeleteSeatCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _eventBus.PublishMessageAsync(request.DeleteDto, "seats_queue", "DeleteSeat");
        }
    }
}
