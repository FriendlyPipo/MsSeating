using Seats.Core.Dtos;

namespace Seats.Core.Services
{
    public interface IEventService
    {
        Task<ZoneDto?> GetZoneByIdAsync(Guid zoneId);
    }
}
