using FluentAssertions;
using Moq;
using Seats.Application.Handlers.Queries;
using Seats.Application.Queries;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;
using Xunit;

namespace Seats.Test.Handlers.Queries
{
    public class GetSeatsByZoneQueryHandlerTests
    {
        private readonly Mock<ISeatRepository> _repositoryMock;
        private readonly GetSeatsByZoneQueryHandler _handler;

        private readonly GetSeatsByZoneQuery _query;
        private readonly Guid _zoneId;
        private readonly Guid _eventId;
        private readonly Guid _functionId;
        private readonly Guid _venueId;
        private readonly List<Seat> _seats;

        public GetSeatsByZoneQueryHandlerTests()
        {
            _repositoryMock = new Mock<ISeatRepository>();
            _handler = new GetSeatsByZoneQueryHandler(_repositoryMock.Object);

            _zoneId = Guid.NewGuid();
            _eventId = Guid.NewGuid();
            _functionId = Guid.NewGuid();
            _venueId = Guid.NewGuid();

            _query = new GetSeatsByZoneQuery(_zoneId, _eventId, _functionId, _venueId);

            _seats = new List<Seat>
            {
                new Seat(
                    SeatId.New(),
                    EventId.Create(_eventId),
                    FunctionId.Create(_functionId),
                    ZoneId.Create(_zoneId),
                    VenueId.Create(_venueId),
                    SeatNumber.Create(1)),
                new Seat(
                    SeatId.New(),
                    EventId.Create(_eventId),
                    FunctionId.Create(_functionId),
                    ZoneId.Create(_zoneId),
                    VenueId.Create(_venueId),
                    SeatNumber.Create(2))
            };
        }

        [Fact]
        public async Task Handle_ShouldReturn_Seats()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetSeatByZoneAsync(
                    It.Is<ZoneId>(id => id.Value == _zoneId),
                    It.Is<EventId>(id => id.Value == _eventId),
                    It.Is<FunctionId>(id => id.Value == _functionId),
                    It.Is<VenueId>(id => id.Value == _venueId)))
                .ReturnsAsync(_seats);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.ForEach(s => 
            {
                s.ZoneId.Value.Should().Be(_zoneId);
                s.EventId.Value.Should().Be(_eventId);
            });
        }

        [Fact]
        public async Task Handle_ShouldReturn_EmptyList()
        {
             // Arrange
            var query = new GetSeatsByZoneQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _repositoryMock
                .Setup(r => r.GetSeatByZoneAsync(It.IsAny<ZoneId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(), It.IsAny<VenueId>()))
                .ReturnsAsync(new List<Seat>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
