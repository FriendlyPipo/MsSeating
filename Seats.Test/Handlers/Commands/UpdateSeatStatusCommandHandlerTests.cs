using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Seats.Application.Commands;
using Seats.Core.Dtos;
using Seats.Application.Handlers.Commands;
using Seats.Core.RabbitMQ;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;
using Seats.Core.Exceptions;
using Xunit;

namespace Seats.Test.Handlers.Commands
{
    public class UpdateSeatStatusCommandHandlerTests
    {
        private readonly Mock<IEventBus<UpdateSeatStatusDto>> _eventBusMock;
        private readonly Mock<ISeatRepository> _repositoryMock;
        private readonly Mock<IValidator<UpdateSeatStatusCommand>> _validatorMock;
        private readonly UpdateSeatStatusCommandHandler _handler;

        private readonly UpdateSeatStatusDto _seatToUpdate;
        private readonly UpdateSeatStatusCommand _command;
        private readonly Seat _seat;

        public UpdateSeatStatusCommandHandlerTests()
        {
            _eventBusMock = new Mock<IEventBus<UpdateSeatStatusDto>>();
            _repositoryMock = new Mock<ISeatRepository>();
            _validatorMock = new Mock<IValidator<UpdateSeatStatusCommand>>();
            _handler = new UpdateSeatStatusCommandHandler(_eventBusMock.Object, _repositoryMock.Object, _validatorMock.Object);

            var seatId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var functionId = Guid.NewGuid();
            var zoneId = Guid.NewGuid();
            var venueId = Guid.NewGuid();

            _seatToUpdate = new UpdateSeatStatusDto
            {
                SeatId = seatId,
                EventId = eventId,
                FunctionId = functionId,
                ZoneId = zoneId,
                VenueId = venueId,
                Status = "Reservado",
                UserId = Guid.NewGuid()
            };

            _command = new UpdateSeatStatusCommand(_seatToUpdate);

            _seat = new Seat(
                 SeatId.Create(seatId),
                 EventId.Create(eventId),
                 FunctionId.Create(functionId),
                 ZoneId.Create(zoneId),
                 VenueId.Create(venueId),
                 SeatNumber.Create(1)
             );
        }

        [Fact]
        public async Task Handle_Should_SendMessage_When_Valid()
        {
            // Arrange
            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<SeatId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(),
                    It.IsAny<ZoneId>(), It.IsAny<VenueId>()))
                .ReturnsAsync(_seat);

            // Act
            await _handler.Handle(_command, CancellationToken.None);

            // Assert
            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.Is<UpdateSeatStatusDto>(msg => msg.SeatId == _seatToUpdate.SeatId), "seats_queue", "UpdateSeatStatus"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_SeatNotFoundException()
        {
             // Arrange
            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<SeatId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(), It.IsAny<ZoneId>(), It.IsAny<VenueId>()))
                .ReturnsAsync((Seat?)null);

            // Act
            var act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<SeatNotFoundException>();
            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.IsAny<UpdateSeatStatusDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_InvalidSeatStatusException()
        {
            // Arrange
            var invalidDto = _seatToUpdate with { Status = "Test_status_invalid" };
            var command = new UpdateSeatStatusCommand(invalidDto);

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidSeatStatusException>();
        }

        [Fact]
        public async Task Handle_ShouldThrow_InvalidOperationException_When_ChagingInvalidStatus()
        {
            // Arrange
            // Seat is Available by default
            var soldDto = _seatToUpdate with { Status = "Vendido" };
            var command = new UpdateSeatStatusCommand(soldDto);

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
             _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<SeatId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(), It.IsAny<ZoneId>(), It.IsAny<VenueId>()))
                .ReturnsAsync(_seat);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El estado de un asiento no puede pasar de disponible a vendido");
        }
    }
}
