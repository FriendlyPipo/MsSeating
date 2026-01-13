using FluentValidation.TestHelper;
using Seats.Application.Commands;
using Seats.Application.Validators;
using Seats.Core.Dtos;
using Xunit;

namespace Seats.Test.Validations
{
    public class CreateSeatCommandValidatorTests
    {
        private readonly CreateSeatCommandValidator _validator;

        public CreateSeatCommandValidatorTests()
        {
            _validator = new CreateSeatCommandValidator();
        }

        [Fact]
        public void Should_Error_When_Seat_IsNull()
        {
            var command = new CreateSeatCommand(null!);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Seat);
        }

        [Fact]
        public void Should_Error_When_EventId_IsEmpty()
        {
            var command = new CreateSeatCommand(new CreateSeatDto { EventId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Seat.EventId);
        }

        [Fact]
        public void Should_Error_When_FunctionId_IsEmpty()
        {
            var command = new CreateSeatCommand(new CreateSeatDto { FunctionId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Seat.FunctionId);
        }

        [Fact]
        public void Should_Error_When_ZoneId_IsEmpty()
        {
            var command = new CreateSeatCommand(new CreateSeatDto { ZoneId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Seat.ZoneId);
        }

        [Fact]
        public void Should_Error_When_VenueId_IsEmpty()
        {
            var command = new CreateSeatCommand(new CreateSeatDto { VenueId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Seat.VenueId);
        }

        [Fact]
        public void Should_Succed_When_Command_IsValid()
        {
            var command = new CreateSeatCommand(new CreateSeatDto
            {
                EventId = Guid.NewGuid(),
                FunctionId = Guid.NewGuid(),
                ZoneId = Guid.NewGuid(),
                VenueId = Guid.NewGuid(),
                Number = 1
            });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
