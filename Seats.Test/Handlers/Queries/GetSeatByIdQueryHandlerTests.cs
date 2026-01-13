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
    public class GetSeatByIdQueryHandlerTests
    {
        private readonly Mock<ISeatRepository> _repositoryMock;
        private readonly GetSeatByIdQueryHandler _handler;

        public GetSeatByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<ISeatRepository>();
            _handler = new GetSeatByIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturn_Seat()
        {
            // Arrange
            var seatId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var functionId = Guid.NewGuid();
            var zoneId = Guid.NewGuid();
            var venueId = Guid.NewGuid();

            var seat = new Seat(
                SeatId.Create(seatId),
                EventId.Create(eventId),
                FunctionId.Create(functionId),
                ZoneId.Create(zoneId),
                VenueId.Create(venueId),
                SeatNumber.Create(1)
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(
                It.Is<SeatId>(x => x.Value == seatId),
                It.Is<EventId>(x => x.Value == eventId),
                It.Is<FunctionId>(x => x.Value == functionId),
                It.Is<ZoneId>(x => x.Value == zoneId),
                It.Is<VenueId>(x => x.Value == venueId)))
                .ReturnsAsync(seat);

            var query = new GetSeatByIdQuery(seatId, eventId, functionId, zoneId, venueId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(seat);
        }

        [Fact]
        public async Task Handle_ShouldReturn_Null()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(
                It.IsAny<SeatId>(), It.IsAny<EventId>(), It.IsAny<FunctionId>(), 
                It.IsAny<ZoneId>(), It.IsAny<VenueId>()))
                .ReturnsAsync((Seat?)null);

            var query = new GetSeatByIdQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
