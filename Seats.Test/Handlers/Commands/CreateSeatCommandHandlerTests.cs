using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Seats.Application.Commands;
using Seats.Core.Dtos;
using Seats.Application.Handlers.Commands;
using Seats.Core.RabbitMQ;
using Seats.Core.Repositories;
using Seats.Core.Services;
using Seats.Core.Exceptions;
using Seats.Domain.ValueObjects;
using Xunit;

namespace Seats.Test.Handlers.Commands
{
    public class CreateSeatCommandHandlerTests
    {
        private readonly Mock<IEventBus<CreateSeatDto>> _eventBusMock;
        private readonly Mock<IValidator<CreateSeatCommand>> _validatorMock;
        private readonly Mock<ISeatRepository> _repositoryMock;
        private readonly Mock<IUserLogService> _userLogServiceMock;
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly CreateSeatCommandHandler _handler;

        private readonly Guid _eventId;
        private readonly Guid _functionId;
        private readonly Guid _zoneId;
        private readonly Guid _venueId;
        private readonly int _seatNumber;
        private readonly CreateSeatDto _seat;
        private readonly CreateSeatCommand _command;
        private readonly ZoneDto _zone;

        public CreateSeatCommandHandlerTests()
        {
            _eventBusMock = new Mock<IEventBus<CreateSeatDto>>();
            _validatorMock = new Mock<IValidator<CreateSeatCommand>>();
            _repositoryMock = new Mock<ISeatRepository>();
            _userLogServiceMock = new Mock<IUserLogService>();
            _eventServiceMock = new Mock<IEventService>();
            
            _handler = new CreateSeatCommandHandler(
                _eventBusMock.Object, 
                _validatorMock.Object, 
                _repositoryMock.Object, 
                _userLogServiceMock.Object,
                _eventServiceMock.Object
            );

            _eventId = Guid.NewGuid();
            _functionId = Guid.NewGuid();
            _zoneId = Guid.NewGuid();
            _venueId = Guid.NewGuid();
            _seatNumber = 10;

            _seat = new CreateSeatDto
            {
                EventId = _eventId,
                FunctionId = _functionId,
                ZoneId = _zoneId,
                VenueId = _venueId,
                Number = _seatNumber
            };

            _command = new CreateSeatCommand(_seat);

            _zone = new ZoneDto
            {
                Id = _zoneId,
                Capacity = 100,
                EventId = _eventId,
                Name = "Test_zona",
                Price = 50
            };
        }

        [Fact]
        public async Task Handle_ShouldReturn_SeatId_When_Valid()
        {
            // Arrange
            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _eventServiceMock
                .Setup(s => s.GetZoneByIdAsync(_zoneId))
                .ReturnsAsync(_zone);

            _repositoryMock
                .Setup(r => r.GetSeatCountByZoneAsync(It.IsAny<ZoneId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(), It.IsAny<VenueId>()))
                .ReturnsAsync(50); 

            _repositoryMock
                .Setup(r => r.GetLastSeatNumberAsync(It.IsAny<EventId>(), It.IsAny<FunctionId>(), It.IsAny<ZoneId>(), It.IsAny<VenueId>()))
                .ReturnsAsync(_seatNumber);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.Is<CreateSeatDto>(d => d.Number == _seatNumber + 1), "seats_queue", "CreateSeat"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEventZoneException_When_ZoneNotFound()
        {
            // Arrange
            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _eventServiceMock
                .Setup(s => s.GetZoneByIdAsync(_zoneId))
                .ReturnsAsync((ZoneDto?)null);

            // Act
            var act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EventZoneException>()
                .WithMessage($"La zona con ID {_zoneId} no existe o no se pudo recuperar.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_EventZoneException()
        {
            // Arrange
            _validatorMock
               .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

            _eventServiceMock
                .Setup(s => s.GetZoneByIdAsync(_zoneId))
                .ReturnsAsync(_zone);

            _repositoryMock
                .Setup(r => r.GetSeatCountByZoneAsync(It.IsAny<ZoneId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(), It.IsAny<VenueId>()))
                .ReturnsAsync(100); 

            // Act
            var act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EventZoneException>()
                 .WithMessage($"No se puede crear el asiento. La capacidad de la zona ({_zone.Capacity}) ha sido alcanzada.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_ValidationException()
        {
            // Arrange
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("EventId", "EventId is required")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(_command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .Where(e => e.Errors.Count() == 1);

            _eventBusMock.Verify(eb => eb.PublishMessageAsync(It.IsAny<CreateSeatDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
