using FluentValidation;
using Seats.Application.Commands;

namespace Seats.Application.Validators
{
    public class DeleteSeatsCommandValidator : AbstractValidator<DeleteSeatsCommand>
    {
        public DeleteSeatsCommandValidator()
        {
             RuleFor(x => x.DeleteDto).NotNull().WithMessage("Data is required");
             RuleFor(x => x.DeleteDto.SeatIds).NotEmpty().WithMessage("SeatIds list cannot be empty.");
        }
    }
}
