using FluentValidation;
using MediatR;
using Seats.Core.Exceptions;
using Seats.Application.Commands;
using Seats.Core.RabbitMQ;
using Seats.Domain.Entities;
using Seats.Core.Dtos;
using Seats.Core.Repositories;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Commands
{
    public class UpdateSeatStatusCommandHandler : IRequestHandler<UpdateSeatStatusCommand>
    {
        private readonly IEventBus<UpdateSeatStatusDto> _eventBus;
        private readonly ISeatRepository _seatRepository;
        private readonly IValidator<UpdateSeatStatusCommand> _validator;

        public UpdateSeatStatusCommandHandler(IEventBus<UpdateSeatStatusDto> eventBus, ISeatRepository seatRepository, IValidator<UpdateSeatStatusCommand> validator)
        {
            _eventBus = eventBus;
            _seatRepository = seatRepository;
            _validator = validator;
        }

        public async Task Handle(UpdateSeatStatusCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (!Enum.TryParse<SeatStatus>(request.SeatStatus.Status, true, out var newStatus))
            {
                 throw new InvalidSeatStatusException(request.SeatStatus.Status);
            }

            var seatId = SeatId.Create(request.SeatStatus.SeatId);
            var eventId = EventId.Create(request.SeatStatus.EventId);
            var functionId = FunctionId.Create(request.SeatStatus.FunctionId);
            var zoneId = ZoneId.Create(request.SeatStatus.ZoneId);
            var venueId = VenueId.Create(request.SeatStatus.VenueId);

            var seat = await _seatRepository.GetByIdAsync(seatId, eventId, functionId, zoneId, venueId);

            if (seat == null)
            {
                throw new SeatNotFoundException(request.SeatStatus.SeatId);
            }

            if (seat.Status == SeatStatus.Disponible && newStatus == SeatStatus.Vendido)
            {
                throw new InvalidOperationException("El estado de un asiento no puede pasar de disponible a vendido");
            }

            if (seat.Status == SeatStatus.Vendido && newStatus != SeatStatus.Disponible)
            {
                throw new InvalidOperationException("Un asiento vendido solo puede cambiar a estado Disponible.");
            }

            // Validar reserva
            if (newStatus == SeatStatus.Reservado)
            {
                if (seat.Status == SeatStatus.Reservado && seat.UserId != null && request.SeatStatus.UserId != null)
                {
                    // Si ya está reservado por otro usuario
                    if (seat.UserId.Value.Value != request.SeatStatus.UserId)
                    {
                         throw new InvalidOperationException($"El asiento ya está reservado por otro usuario.");
                    }
                }
                else if (seat.Status == SeatStatus.Vendido)
                {
                     throw new InvalidOperationException($"El asiento ya ha sido vendido.");
                }
            }

            await _eventBus.PublishMessageAsync(request.SeatStatus, "seats_queue", "UpdateSeatStatus");
        }
    }
}
