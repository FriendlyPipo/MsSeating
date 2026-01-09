using MediatR;
using Seats.Domain.Entities;

namespace Seats.Application.Queries
{
    public record GetSeatByIdQuery(Guid Id) : IRequest<Seat?>;
}
