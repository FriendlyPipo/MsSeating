using FluentValidation;
using Seats.Application.Commands;

namespace Seats.Application.Validators
{
    public class DeleteSeatCommandValidator : AbstractValidator<DeleteSeatCommand>
    {
        public DeleteSeatCommandValidator()
        {
             RuleFor(x => x.DeleteDto).NotNull().WithMessage("Data is required");
             RuleFor(x => x.DeleteDto.SeatId).NotEmpty().WithMessage("SeatId is required.");
        }
    }
}
