using FluentValidation;
using Seats.Application.Commands;
using Seats.Core.Repositories;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Validators
{
    public class DeleteSeatsCommandValidator : AbstractValidator<DeleteSeatsCommand>
    {
        private readonly ISeatRepository _repository;

        public DeleteSeatsCommandValidator(ISeatRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.DeleteDto).NotNull().WithMessage("Los datos son obligatorios.");

            RuleFor(x => x.DeleteDto.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

            RuleFor(x => x.DeleteDto.EventId)
                .NotEmpty().WithMessage("El ID del evento es obligatorio.");

            RuleFor(x => x.DeleteDto.FunctionId)
                .NotEmpty().WithMessage("El ID de la función es obligatorio.");

            RuleFor(x => x.DeleteDto.ZoneId)
                .NotEmpty().WithMessage("El ID de la zona es obligatorio.");

            RuleFor(x => x.DeleteDto.VenueId)
                .NotEmpty().WithMessage("El ID del recinto es obligatorio.");

            RuleFor(x => x)
                .MustAsync(EnoughSeats)
                .WithMessage("No hay suficientes asientos para eliminar en la ubicación especificada.");
        }

        private async Task<bool> EnoughSeats(DeleteSeatsCommand command, CancellationToken token)
        {
            var seat = command.DeleteDto;

            if (seat.Quantity <= 0 ||
                seat.EventId == Guid.Empty || seat.FunctionId == Guid.Empty ||
                seat.ZoneId == Guid.Empty || seat.VenueId == Guid.Empty)
            {
                return true;
            }

            try
            {
                var seats = await _repository.GetSeatByZoneAsync(
                    ZoneId.Create(seat.ZoneId),
                    EventId.Create(seat.EventId),
                    FunctionId.Create(seat.FunctionId),
                    VenueId.Create(seat.VenueId)
                );

                return seats.Count >= seat.Quantity;
            }
            catch
            {
                return true;
            }
        }
    }
}
