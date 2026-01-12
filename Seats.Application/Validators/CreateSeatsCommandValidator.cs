using FluentValidation;
using Seats.Application.Commands;

namespace Seats.Application.Validators
{
    public class CreateSeatsCommandValidator : AbstractValidator<CreateSeatsCommand>
    {
        public CreateSeatsCommandValidator()
        {
            RuleFor(x => x.SeatsDto.Quantity).GreaterThan(0).WithMessage("Por favor, ingrese una cantidad válida de asientos a crear.");
            RuleFor(x => x.SeatsDto.EventId).NotEmpty().WithMessage("Por favor, ingrese un EventId válido.");
            RuleFor(x => x.SeatsDto.FunctionId).NotEmpty().WithMessage("Por favor, ingrese un FunctionId válido.");
            RuleFor(x => x.SeatsDto.ZoneId).NotEmpty().WithMessage("Por favor, ingrese un ZoneId válido.");
            RuleFor(x => x.SeatsDto.VenueId).NotEmpty().WithMessage("Por favor, ingrese un VenueId válido.");
            RuleFor(x => x.SeatsDto.Row).GreaterThanOrEqualTo(0).WithMessage("Por favor, ingrese un numero de fila válido.");
        }
    }
}
