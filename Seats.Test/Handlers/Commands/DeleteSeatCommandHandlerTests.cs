using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Seats.Application.Commands;
using Seats.Core.Dtos;
using Seats.Application.Handlers.Commands;
using Seats.Core.RabbitMQ;
using Xunit;

namespace Seats.Test.Handlers.Commands
{
    public class DeleteSeatCommandHandlerTests
    {
        private readonly Mock<IEventBus<DeleteSeatDto>> _eventBusMock;
        private readonly Mock<IValidator<DeleteSeatCommand>> _validatorMock;
        private readonly DeleteSeatCommandHandler _handler;

        private readonly DeleteSeatDto _seatToDelete;
        private readonly DeleteSeatCommand _command;

        public DeleteSeatCommandHandlerTests()
        {
            _eventBusMock = new Mock<IEventBus<DeleteSeatDto>>();
            _validatorMock = new Mock<IValidator<DeleteSeatCommand>>();

            _handler = new DeleteSeatCommandHandler(_eventBusMock.Object, _validatorMock.Object);

            _seatToDelete = new DeleteSeatDto
            {
                SeatId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                FunctionId = Guid.NewGuid(),
                ZoneId = Guid.NewGuid(),
                VenueId = Guid.NewGuid()
            };

            _command = new DeleteSeatCommand(_seatToDelete);
        }

        [Fact]
        public async Task Handle_ShouldPublishDeleteSeat()
        {
            // Arrange
            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            await _handler.Handle(_command, CancellationToken.None);

            // Assert
            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.Is<DeleteSeatDto>(d => d.SeatId == _seatToDelete.SeatId), "seats_queue", "DeleteSeat"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException()
        {
            // Arrange
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("SeatId", "SeatId is required")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .Where(e => e.Errors.Count() == 1);
            
            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.IsAny<DeleteSeatDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
