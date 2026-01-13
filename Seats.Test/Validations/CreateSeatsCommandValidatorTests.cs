using FluentValidation.TestHelper;
using Seats.Application.Commands;
using Seats.Application.Validators;
using Seats.Core.Dtos;
using Xunit;

namespace Seats.Test.Validations
{
    public class CreateSeatsCommandValidatorTests
    {
        private readonly CreateSeatsCommandValidator _validator;

        public CreateSeatsCommandValidatorTests()
        {
            _validator = new CreateSeatsCommandValidator();
        }

        [Fact]
        public void Should_Error_When_Seats_IsNull()
        {
            var command = new CreateSeatsCommand(null!);
            Assert.Throws<NullReferenceException>(() => _validator.TestValidate(command));
        }

        [Fact]
        public void Should_Error_When_Quantity_Is_Zer0()
        {
            var command = new CreateSeatsCommand(new CreateSeatsDto { Quantity = 0 });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatsDto.Quantity);
        }

        [Fact]
        public void Should_Error_When_EventId_IsEmpty()
        {
            var command = new CreateSeatsCommand(new CreateSeatsDto { Quantity = 1, EventId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatsDto.EventId);
        }

        [Fact]
        public void Should_Error_When_FunctionId_IsEmpty()
        {
            var command = new CreateSeatsCommand(new CreateSeatsDto { Quantity = 1, FunctionId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatsDto.FunctionId);
        }

        [Fact]
        public void Should_Error_When_ZoneId_IsEmpty()
        {
            var command = new CreateSeatsCommand(new CreateSeatsDto { Quantity = 1, ZoneId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatsDto.ZoneId);
        }

        [Fact]
        public void Should_Error_When_VenueId_IsEmpty()
        {
            var command = new CreateSeatsCommand(new CreateSeatsDto { Quantity = 1, VenueId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatsDto.VenueId);
        }

        [Fact]
        public void Should_Succed_When_Command_IsValid()
        {
            var command = new CreateSeatsCommand(new CreateSeatsDto
            {
                Quantity = 5,
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
