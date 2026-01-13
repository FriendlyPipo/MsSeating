using FluentValidation.TestHelper;
using Seats.Application.Commands;
using Seats.Application.Validators;
using Seats.Core.Dtos;
using Xunit;

namespace Seats.Test.Validations
{
    public class DeleteSeatCommandValidatorTests
    {
        private readonly DeleteSeatCommandValidator _validator;

        public DeleteSeatCommandValidatorTests()
        {
            _validator = new DeleteSeatCommandValidator();
        }

        [Fact]
        public void Should_Error_When_DeleteDto_IsNull()
        {
            var command = new DeleteSeatCommand(null!);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto);
        }

        [Fact]
        public void Should_Error_When_Ids_AreNull()
        {
            var command = new DeleteSeatCommand(new DeleteSeatDto());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.SeatId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.EventId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.FunctionId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.ZoneId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.VenueId);
        }

        [Fact]
        public void Should_Succeed_When_Command_Is_Valid()
        {
            var command = new DeleteSeatCommand(new DeleteSeatDto
            {
                SeatId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                FunctionId = Guid.NewGuid(),
                ZoneId = Guid.NewGuid(),
                VenueId = Guid.NewGuid()
            });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
