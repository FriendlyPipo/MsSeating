using FluentValidation;
using Seats.Application.Commands;
using Seats.Domain.Entities;

namespace Seats.Application.Validators
{
    public class UpdateSeatStatusCommandValidator : AbstractValidator<UpdateSeatStatusCommand>
    {
        public UpdateSeatStatusCommandValidator()
        {
            RuleFor(x => x.SeatStatus).NotNull().WithMessage("Los datos son obligatorios.");
            RuleFor(x => x.SeatStatus.SeatId).NotEmpty().WithMessage("El Id del Asiento es obligatorio.");
            RuleFor(x => x.SeatStatus.Status).NotEmpty().WithMessage("El Estado es obligatorio.")
                .IsEnumName(typeof(SeatStatus), caseSensitive: false)
                .WithMessage("Estado del Asiento inválido.");
        }
    }
}
