using MediatR;
using Seats.Domain.Entities;

namespace Seats.Application.Queries
{
    public record GetAllSeatsQuery : IRequest<List<Seat>>;
}
