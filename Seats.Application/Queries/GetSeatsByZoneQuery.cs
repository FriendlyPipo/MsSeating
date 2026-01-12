using MediatR;
using Seats.Domain.Entities;

namespace Seats.Application.Queries
{
    public record GetSeatsByZoneQuery(Guid ZoneId, Guid EventId, Guid FunctionId, Guid VenueId) : IRequest<List<Seat>>;
}
