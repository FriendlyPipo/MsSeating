using FluentValidation;
using Seats.Application.Commands;

namespace Seats.Application.Validators
{
    public class CreateSeatCommandValidator : AbstractValidator<CreateSeatCommand>
    {
        public CreateSeatCommandValidator()
        {
            RuleFor(x => x.Seat).NotNull().WithMessage("Los datos del asiento son obligatorios.");
            RuleFor(x => x.Seat.EventId).NotEmpty().WithMessage("El Id del Evento es obligatorio.");
            RuleFor(x => x.Seat.FunctionId).NotEmpty().WithMessage("El Id de la Función es obligatorio.");
            RuleFor(x => x.Seat.ZoneId).NotEmpty().WithMessage("El Id de la Zona es obligatorio.");
            RuleFor(x => x.Seat.VenueId).NotEmpty().WithMessage("El Id del Lugar es obligatorio.");
            RuleFor(x => x.Seat.Row).GreaterThanOrEqualTo(0).WithMessage("La Fila debe ser mayor o igual a 0.");
            RuleFor(x => x.Seat.Number).GreaterThan(0).WithMessage("El Número debe ser mayor que 0.");
        }
    }
}
