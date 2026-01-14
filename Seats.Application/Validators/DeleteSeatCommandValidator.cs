using FluentValidation;
using Seats.Application.Commands;

namespace Seats.Application.Validators
{
    public class DeleteSeatCommandValidator : AbstractValidator<DeleteSeatCommand>
    {
        public DeleteSeatCommandValidator()
        {
            RuleFor(x => x.DeleteDto).NotNull().WithMessage("Los datos para eliminar el asiento son obligatorios.");
                RuleFor(x => x.DeleteDto.SeatId).NotEmpty().WithMessage("Por favor, ingrese un SeatId válido.");
                RuleFor(x => x.DeleteDto.EventId).NotEmpty().WithMessage("Por favor, ingrese un EventId válido.");
                RuleFor(x => x.DeleteDto.FunctionId).NotEmpty().WithMessage("Por favor, ingrese un FunctionId válido.");
                RuleFor(x => x.DeleteDto.ZoneId).NotEmpty().WithMessage("Por favor, ingrese un ZoneId válido.");
                RuleFor(x => x.DeleteDto.VenueId).NotEmpty().WithMessage("Por favor, ingrese un VenueId válido.");
        }
    }
}
