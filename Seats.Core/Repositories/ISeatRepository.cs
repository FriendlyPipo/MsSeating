using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Core.Repositories
{
    public interface ISeatRepository
    {
        Task AddAsync(Seat seat);
        Task<Seat?> GetByIdAsync(SeatId id);
        Task<List<Seat>> GetAllAsync();
        Task<List<Seat>> GetByZoneAsync(ZoneId zoneId);
        Task DeleteAsync(SeatId id);
        Task DeleteMultipleSeatsAsync(IEnumerable<SeatId> ids);
        Task AddMultipleSeatsAsync(IEnumerable<Seat> seats);
        Task<Seat?> UpdateAsync(Seat seat, CancellationToken cancellationToken);
    }
}
