using MediatR;
using Seats.Application.Dtos;

namespace Seats.Application.Commands
{
    public record CreateSeatCommand(CreateSeatDto Seat) : IRequest<Guid>;
}
