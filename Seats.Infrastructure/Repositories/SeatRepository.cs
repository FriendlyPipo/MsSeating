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

        public async Task<Seat?> GetByIdAsync(SeatId id)
        {
            return await _db.Seat
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SeatId.Value == id.Value);
        }

        public async Task<List<Seat>> GetAllAsync()
        {
            return await _db.Seat.AsNoTracking().ToListAsync();
        }

        public async Task<List<Seat>> GetByZoneAsync(ZoneId zoneId)
        {
            return await _db.Seat
                .AsNoTracking()
                .Where(s => s.ZoneId.Value == zoneId.Value)
                .ToListAsync();
        }

        public async Task<Seat?> UpdateAsync(Seat seat, CancellationToken cancellationToken)
        {
            _db.Seat.Update(seat);
            await _db.SaveEfContextChanges("system", cancellationToken);
            return seat;
        }

        public async Task DeleteAsync(SeatId id)
        {
            var entity = await _db.Seat.FindAsync(id); 
            if (entity != null)
            {
                 _db.Seat.Remove(entity);
                 await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleSeatsAsync(IEnumerable<SeatId> ids)
        {
            var idValues = ids.Select(x => x.Value).ToList();
            var seats = await _db.Seat
                .Where(s => idValues.Contains(s.SeatId.Value))
                .ToListAsync();

            if (seats.Any())
            {
                _db.Seat.RemoveRange(seats);
                await _db.SaveChangesAsync();
            }
        }
    }
}
