using FluentValidation;
using Seats.Application.Commands;

namespace Seats.Application.Validators
{
    public class CreateSeatCommandValidator : AbstractValidator<CreateSeatCommand>
    {
        public CreateSeatCommandValidator()
        {
            RuleFor(x => x.Seat).NotNull().WithMessage("Los datos del asiento son obligatorios.");

            When(x => x.Seat != null, () =>
            {
                RuleFor(x => x.Seat.EventId).NotEmpty().WithMessage("El Id del Evento es obligatorio.");
                RuleFor(x => x.Seat.FunctionId).NotEmpty().WithMessage("El Id de la Función es obligatorio.");
                RuleFor(x => x.Seat.ZoneId).NotEmpty().WithMessage("El Id de la Zona es obligatorio.");
                RuleFor(x => x.Seat.VenueId).NotEmpty().WithMessage("El Id del Lugar es obligatorio.");
            });
        }
    }
}
