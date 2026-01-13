using MediatR;
using Seats.Core.Dtos;

namespace Seats.Application.Commands
{
    public record UpdateSeatStatusCommand(UpdateSeatStatusDto SeatStatus) : IRequest;
}
