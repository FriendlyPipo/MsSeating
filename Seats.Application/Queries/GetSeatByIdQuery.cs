using MediatR;
using Seats.Domain.Entities;

namespace Seats.Application.Queries
{
    public record GetSeatByIdQuery(Guid SeatId, Guid EventId, Guid FunctionId, Guid ZoneId, Guid VenueId) : IRequest<Seat?>;
}
