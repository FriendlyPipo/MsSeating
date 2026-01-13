using FluentValidation.TestHelper;
using Seats.Application.Commands;
using Seats.Application.Validators;
using Seats.Core.Dtos;
using Seats.Domain.Entities;
using Xunit;

namespace Seats.Test.Validations
{
    public class UpdateSeatStatusCommandValidatorTests
    {
        private readonly UpdateSeatStatusCommandValidator _validator;

        public UpdateSeatStatusCommandValidatorTests()
        {
            _validator = new UpdateSeatStatusCommandValidator();
        }

        [Fact]
        public void Should_Error_When_SeatStatus_IsNull()
        {
            var command = new UpdateSeatStatusCommand(null!);
            Assert.Throws<NullReferenceException>(() => _validator.TestValidate(command));
        }

        [Fact]
        public void Should_Error_When_SeatId_IsNull()
        {
            var command = new UpdateSeatStatusCommand(new UpdateSeatStatusDto { Status = "Disponible" });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatStatus.SeatId);
        }

        [Fact]
        public void Should_Error_When_Status_IsNull()
        {
            var command = new UpdateSeatStatusCommand(new UpdateSeatStatusDto { SeatId = Guid.NewGuid() });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatStatus.Status);
        }

        [Fact]
        public void Should_Error_When_Status_IsInvalid()
        {
            var command = new UpdateSeatStatusCommand(new UpdateSeatStatusDto { SeatId = Guid.NewGuid(), Status = "Invalid" });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SeatStatus.Status);
        }

        [Fact]
        public void Should_Succed_When_Command_IsValid()
        {
            var command = new UpdateSeatStatusCommand(new UpdateSeatStatusDto 
            { 
                 SeatId = Guid.NewGuid(), 
                 Status = nameof(SeatStatus.Vendido) 
            });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
