using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Core.Repositories
{
    public interface ISeatRepository
    {
        Task AddAsync(Seat seat);
        Task<Seat?> GetByIdAsync(SeatId id);
        Task<List<Seat>> GetAllAsync();
        Task DeleteAsync(SeatId id);
        Task DeleteMultipleSeatsAsync(IEnumerable<SeatId> ids);
        Task AddMultipleSeatsAsync(IEnumerable<Seat> seats);

    }
}
