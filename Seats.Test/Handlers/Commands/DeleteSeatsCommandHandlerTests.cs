using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Seats.Application.Commands;
using Seats.Core.Dtos;
using Seats.Application.Handlers.Commands;
using Seats.Core.RabbitMQ;
using Seats.Core.Repositories;
using Seats.Domain.ValueObjects;
using Seats.Domain.Entities;
using Xunit;

namespace Seats.Test.Handlers.Commands
{
    public class DeleteSeatsCommandHandlerTests
    {
        private readonly Mock<ISeatRepository> _repositoryMock;
        private readonly Mock<IEventBus<DeleteSeatDto>> _eventBusMock;
        private readonly Mock<IValidator<DeleteSeatsCommand>> _validatorMock;
        private readonly DeleteSeatsCommandHandler _handler;

        private readonly DeleteSeatsDto _deleteSeatsDto;
        private readonly DeleteSeatsCommand _command;

        public DeleteSeatsCommandHandlerTests()
        {
            _repositoryMock = new Mock<ISeatRepository>();
            _eventBusMock = new Mock<IEventBus<DeleteSeatDto>>();
            _validatorMock = new Mock<IValidator<DeleteSeatsCommand>>();

            _handler = new DeleteSeatsCommandHandler(_repositoryMock.Object, _eventBusMock.Object, _validatorMock.Object);

            _deleteSeatsDto = new DeleteSeatsDto
            {
                Quantity = 3,
                EventId = Guid.NewGuid(),
                FunctionId = Guid.NewGuid(),
                ZoneId = Guid.NewGuid(),
                VenueId = Guid.NewGuid()
            };

            _command = new DeleteSeatsCommand(_deleteSeatsDto);
        }

        [Fact]
        public async Task Handle_ShouldPublishMessages()
        {
            // Arrange
            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var seatsList = new List<Seat>
            {
                new Seat(SeatId.New(), EventId.Create(_deleteSeatsDto.EventId), FunctionId.Create(_deleteSeatsDto.FunctionId), 
                         ZoneId.Create(_deleteSeatsDto.ZoneId), VenueId.Create(_deleteSeatsDto.VenueId), SeatNumber.Create(10)),
                new Seat(SeatId.New(), EventId.Create(_deleteSeatsDto.EventId), FunctionId.Create(_deleteSeatsDto.FunctionId), 
                         ZoneId.Create(_deleteSeatsDto.ZoneId), VenueId.Create(_deleteSeatsDto.VenueId), SeatNumber.Create(9)),
                new Seat(SeatId.New(), EventId.Create(_deleteSeatsDto.EventId), FunctionId.Create(_deleteSeatsDto.FunctionId), 
                         ZoneId.Create(_deleteSeatsDto.ZoneId), VenueId.Create(_deleteSeatsDto.VenueId), SeatNumber.Create(8))
            };

            _repositoryMock
                .Setup(r => r.GetSeatByZoneAsync(It.IsAny<ZoneId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(), It.IsAny<VenueId>()))
                .ReturnsAsync(seatsList);

            // Act
            await _handler.Handle(_command, CancellationToken.None);

            // Assert
            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.IsAny<DeleteSeatDto>(), "seats_queue", "DeleteSeat"), Times.Exactly(3));
        }

        [Fact]
        public async Task Handle_ShouldThrow_ValidationException()
        {
            // Arrange
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Quantity", "Must be > 0")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.IsAny<DeleteSeatDto>(),It.IsAny<string>(),It.IsAny<string>()), Times.Never);
        }
    }
}
