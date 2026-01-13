using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;
using Seats.Infrastructure.Database.Context;
using Seats.Infrastructure.Repositories;
using Xunit;

namespace Seats.Test.Repositories
{
    public class SeatRepositoryTests
    {
        private readonly DbContextOptions<SeatsDbContext> _options;

        public SeatRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<SeatsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private async Task<SeatsDbContext> GetDbContext()
        {
            var context = new SeatsDbContext(_options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task AddAsync_Should_AddSeat()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new SeatRepository(context);
            var seat = new Seat(
                SeatId.Create(Guid.NewGuid()), 
                EventId.Create(Guid.NewGuid()), 
                FunctionId.Create(Guid.NewGuid()), 
                ZoneId.Create(Guid.NewGuid()), 
                VenueId.Create(Guid.NewGuid()), 
                SeatNumber.Create(1)
            );

            // Act
            await repository.AddAsync(seat);

            // Assert
            var addedSeat = await context.Seat.FirstOrDefaultAsync();
            addedSeat.Should().NotBeNull();
            addedSeat!.SeatId.Should().Be(seat.SeatId);
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnSeat()
        {
             // Arrange
            using var context = await GetDbContext();
            var repository = new SeatRepository(context);
            var seat = new Seat(
                SeatId.Create(Guid.NewGuid()), 
                EventId.Create(Guid.NewGuid()), 
                FunctionId.Create(Guid.NewGuid()), 
                ZoneId.Create(Guid.NewGuid()), 
                VenueId.Create(Guid.NewGuid()), 
                SeatNumber.Create(1)
            );
            await repository.AddAsync(seat);

            // Act
            var result = await repository.GetByIdAsync(seat.SeatId, seat.EventId, seat.FunctionId, seat.ZoneId, seat.VenueId);

            // Assert
            result.Should().NotBeNull();
            result!.SeatId.Should().Be(seat.SeatId);
        }

        [Fact]
        public async Task GetSeatByZoneAsync_Should_Return_SeatsInZone()
        {
             // Arrange
            using var context = await GetDbContext();
            var repository = new SeatRepository(context);
            var zoneId = ZoneId.Create(Guid.NewGuid());
            var eventId = EventId.Create(Guid.NewGuid());
            var functionId = FunctionId.Create(Guid.NewGuid());
            var venueId = VenueId.Create(Guid.NewGuid());

            var seat1 = new Seat(
                SeatId.Create(Guid.NewGuid()), 
                eventId, 
                functionId, 
                zoneId, 
                venueId, 
                SeatNumber.Create(1)
            );
            var seat2 = new Seat(
                SeatId.Create(Guid.NewGuid()), 
                eventId, 
                functionId, 
                zoneId, 
                venueId, 
                SeatNumber.Create(2)
            );
            var seatOther = new Seat(
                SeatId.Create(Guid.NewGuid()), 
                EventId.Create(Guid.NewGuid()), 
                functionId, 
                zoneId, 
                venueId, 
                SeatNumber.Create(3)
            );

            await repository.AddAsync(seat1);
            await repository.AddAsync(seat2);
            await repository.AddAsync(seatOther);

            var result = await repository.GetSeatByZoneAsync(zoneId, eventId, functionId, venueId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(s => s.SeatId.Value == seat1.SeatId.Value);
            result.Should().Contain(s => s.SeatId.Value == seat2.SeatId.Value);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSeat()
        {
             // Arrange
            using var context = await GetDbContext();
            var repository = new SeatRepository(context);
            var seat = new Seat(
                SeatId.Create(Guid.NewGuid()), 
                EventId.Create(Guid.NewGuid()), 
                FunctionId.Create(Guid.NewGuid()), 
                ZoneId.Create(Guid.NewGuid()), 
                VenueId.Create(Guid.NewGuid()), 
                SeatNumber.Create(1)
            );
            await repository.AddAsync(seat);

            // Act
            seat.AssignUser(UserId.Create(Guid.NewGuid()));
            seat.ChangeStatus(SeatStatus.Reservado);
            await repository.UpdateAsync(seat, CancellationToken.None);

            // Assert
            var updatedSeat = await context.Seat.FindAsync(seat.SeatId, seat.EventId, seat.FunctionId, seat.ZoneId, seat.VenueId);
            updatedSeat.Should().NotBeNull();
            updatedSeat!.Status.Should().Be(SeatStatus.Reservado);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveSeat()
        {
             // Arrange
            using var context = await GetDbContext();
            var repository = new SeatRepository(context);
            var seat = new Seat(
                SeatId.Create(Guid.NewGuid()), 
                EventId.Create(Guid.NewGuid()), 
                FunctionId.Create(Guid.NewGuid()), 
                ZoneId.Create(Guid.NewGuid()), 
                VenueId.Create(Guid.NewGuid()), 
                SeatNumber.Create(1)
            );
            await repository.AddAsync(seat);

            // Act
            await repository.DeleteAsync(seat.SeatId);

            // Assert
            var deletedSeat = await context.Seat.FindAsync(seat.SeatId, seat.EventId, seat.FunctionId, seat.ZoneId, seat.VenueId);
            deletedSeat.Should().BeNull();
        }
    }
}
