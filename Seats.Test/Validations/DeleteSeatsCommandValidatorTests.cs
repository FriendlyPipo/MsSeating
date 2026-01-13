using FluentValidation.TestHelper;
using Moq;
using Seats.Application.Commands;
using Seats.Application.Validators;
using Seats.Core.Dtos;
using Seats.Core.Repositories;
using Seats.Domain.ValueObjects;
using Xunit;

namespace Seats.Test.Validations
{
    public class DeleteSeatsCommandValidatorTests
    {
        private readonly Mock<ISeatRepository> _repositoryMock;
        private readonly DeleteSeatsCommandValidator _validator;

        public DeleteSeatsCommandValidatorTests()
        {
            _repositoryMock = new Mock<ISeatRepository>();
            _validator = new DeleteSeatsCommandValidator(_repositoryMock.Object);
        }

        [Fact]
        public async Task Should_Error_When_DeleteDto_IsNull()
        {
            var command = new DeleteSeatsCommand(null!);
            await Assert.ThrowsAsync<NullReferenceException>(() => _validator.TestValidateAsync(command));
        }

        [Fact]
        public async Task Should_Error_When_Quantity_IsZero()
        {
            var command = new DeleteSeatsCommand(new DeleteSeatsDto { Quantity = 0 });
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.Quantity);
        }

        [Fact]
        public async Task Should_Error_When_Ids_AreNull()
        {
            var command = new DeleteSeatsCommand(new DeleteSeatsDto { Quantity = 1 });
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.EventId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.FunctionId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.ZoneId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteDto.VenueId);
        }

         [Fact]
        public async Task Should_Error_When_NotEnoughSeats()
        {
            var command = new DeleteSeatsCommand(new DeleteSeatsDto 
            { 
                Quantity = 5,
                EventId = Guid.NewGuid(),
                FunctionId = Guid.NewGuid(),
                ZoneId = Guid.NewGuid(),
                VenueId = Guid.NewGuid()
            });

            _repositoryMock
                .Setup(r => r.GetSeatByZoneAsync(It.IsAny<ZoneId>(), It.IsAny<EventId>(),It.IsAny<FunctionId>(),It.IsAny<VenueId>()))
                .ReturnsAsync(new List<Seats.Domain.Entities.Seat>()); 

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x);
        }
    }
}
