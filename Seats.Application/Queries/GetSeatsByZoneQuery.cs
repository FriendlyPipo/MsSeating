using MediatR;
using Seats.Domain.Entities;

namespace Seats.Application.Queries
{
    public record GetSeatsByZoneQuery(Guid ZoneId) : IRequest<List<Seat>>;
}
