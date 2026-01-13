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
    public class GetAllSeatsQueryHandlerTests
    {
        private readonly Mock<ISeatRepository> _repositoryMock;
        private readonly GetAllSeatsQueryHandler _handler;

        public GetAllSeatsQueryHandlerTests()
        {
            _repositoryMock = new Mock<ISeatRepository>();
            _handler = new GetAllSeatsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturn_Seats()
        {
            // Arrange
            var seat1 = new Seat(
                    SeatId.Create(Guid.NewGuid()), 
                    EventId.Create(Guid.NewGuid()), 
                    FunctionId.Create(Guid.NewGuid()), 
                    ZoneId.Create(Guid.NewGuid()), 
                    VenueId.Create(Guid.NewGuid()), 
                    SeatNumber.Create(1));

            var seat2 = new Seat(
                    SeatId.Create(Guid.NewGuid()), 
                    EventId.Create(Guid.NewGuid()), 
                    FunctionId.Create(Guid.NewGuid()), 
                    ZoneId.Create(Guid.NewGuid()), 
                    VenueId.Create(Guid.NewGuid()), 
                    SeatNumber.Create(2));
            
            seat2.ChangeStatus(SeatStatus.Reservado);

            var seats = new List<Seat> { seat1, seat2 };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(seats);

            var query = new GetAllSeatsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(seats);
        }
    }
}
