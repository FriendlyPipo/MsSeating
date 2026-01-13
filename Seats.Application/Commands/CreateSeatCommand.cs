using MediatR;
using Seats.Core.Dtos;

namespace Seats.Application.Commands
{
    public record CreateSeatCommand(CreateSeatDto Seat) : IRequest<Guid>;
}
