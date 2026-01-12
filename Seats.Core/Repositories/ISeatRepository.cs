using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Core.Repositories
{
    public interface ISeatRepository
    {
        Task AddAsync(Seat seat);
        Task<Seat?> GetByIdAsync(SeatId seatId, EventId eventId, FunctionId functionId, ZoneId zoneId, VenueId venueId);
        Task<List<Seat>> GetAllAsync();
        Task<List<Seat>> GetSeatByZoneAsync(ZoneId zoneId, EventId eventId, FunctionId functionId, VenueId venueId);
        Task<List<Seat>> GetSpecificSeatsAsync(EventId eventId, FunctionId functionId, ZoneId zoneId, VenueId venueId, SeatRow row);
        Task DeleteAsync(SeatId id);
        Task DeleteMultipleSeatsAsync(IEnumerable<SeatId> ids);
        Task AddMultipleSeatsAsync(IEnumerable<Seat> seats);
        Task<Seat?> UpdateAsync(Seat seat, CancellationToken cancellationToken);
    }
}
