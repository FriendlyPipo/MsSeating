using Microsoft.EntityFrameworkCore;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;
using Seats.Infrastructure.Database.Context;

namespace Seats.Infrastructure.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly SeatsDbContext _db;

        public SeatRepository(SeatsDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Seat seat)
        {
            await _db.Seat.AddAsync(seat);
            await _db.SaveChangesAsync(); 
        }

        public async Task AddMultipleSeatsAsync(IEnumerable<Seat> seats)
        {
            await _db.Seat.AddRangeAsync(seats);
            await _db.SaveChangesAsync();
        }

        public async Task<Seat?> GetByIdAsync(SeatId seatId, EventId eventId, FunctionId functionId, ZoneId zoneId, VenueId venueId)
        {
            return await _db.Seat.FindAsync(seatId, eventId, functionId, zoneId, venueId);
        }

        public async Task<List<Seat>> GetAllAsync()
        {
            return await _db.Seat.AsNoTracking().ToListAsync();
        }

        public async Task<List<Seat>> GetSeatByZoneAsync(ZoneId zoneId, EventId eventId, FunctionId functionId, VenueId venueId)
        {
            return await _db.Seat
                .AsNoTracking()
                .Where(s => s.ZoneId.Equals(zoneId) &&
                            s.EventId.Equals(eventId) &&
                            s.FunctionId.Equals(functionId) &&
                            s.VenueId.Equals(venueId))
                .ToListAsync();
        }

        public async Task<int> GetSeatCountByZoneAsync(ZoneId zoneId, EventId eventId, FunctionId functionId, VenueId venueId)
        {
            return await _db.Seat
                 .AsNoTracking()
                 .CountAsync(s => s.ZoneId.Equals(zoneId) &&
                             s.EventId.Equals(eventId) &&
                             s.FunctionId.Equals(functionId) &&
                             s.VenueId.Equals(venueId));
        }

        public async Task<int> GetLastSeatNumberAsync(EventId eventId, FunctionId functionId, ZoneId zoneId, VenueId venueId)
        {
            var seats = await _db.Seat
                .AsNoTracking()
                .Where(s => s.EventId.Equals(eventId) &&
                            s.FunctionId.Equals(functionId) &&
                            s.ZoneId.Equals(zoneId) &&
                            s.VenueId.Equals(venueId))
                .Select(s => s.Number)
                .ToListAsync();

            return seats.Any() ? seats.Max(s => s.Value) : 0;
        }

        public async Task<Seat?> UpdateAsync(Seat seat, CancellationToken cancellationToken)
        {
            _db.Seat.Update(seat);
            await _db.SaveEfContextChanges("system", cancellationToken);
            return seat;
        }

        public async Task DeleteAsync(SeatId id)
        {
            var entity = await _db.Seat.FirstOrDefaultAsync(s => s.SeatId.Equals(id)); 
            if (entity != null)
            {
                 _db.Seat.Remove(entity);
                 await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleSeatsAsync(IEnumerable<SeatId> ids)
        {
            var seats = await _db.Seat
                .Where(s => ids.Contains(s.SeatId))
                .ToListAsync();

            if (seats.Any())
            {
                _db.Seat.RemoveRange(seats);
                await _db.SaveChangesAsync();
            }
        }
    }
}
