using FluentValidation;
using Seats.Application.Commands;

namespace Seats.Application.Validators
{
    public class CreateSeatsCommandValidator : AbstractValidator<CreateSeatsCommand>
    {
        public CreateSeatsCommandValidator()
        {
            RuleFor(x => x.SeatsDto).NotNull().WithMessage("List of seats is required.");
            RuleFor(x => x.SeatsDto.Seats).NotEmpty().WithMessage("At least one seat is required.");
            RuleForEach(x => x.SeatsDto.Seats).ChildRules(seat =>
            {
                seat.RuleFor(x => x.EventId).NotEmpty().WithMessage("EventId is required.");
                seat.RuleFor(x => x.FunctionId).NotEmpty().WithMessage("FunctionId is required.");
                seat.RuleFor(x => x.ZoneId).NotEmpty().WithMessage("ZoneId is required.");
                seat.RuleFor(x => x.VenueId).NotEmpty().WithMessage("VenueId is required.");
                seat.RuleFor(x => x.Row).NotEmpty().WithMessage("Row is required.");
                seat.RuleFor(x => x.Number).GreaterThan(0).WithMessage("Number must be greater than 0.");
            });
        }
    }
}
