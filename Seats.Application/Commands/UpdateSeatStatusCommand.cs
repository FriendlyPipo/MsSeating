using MediatR;
using Seats.Application.Dtos;

namespace Seats.Application.Commands
{
    public record UpdateSeatStatusCommand(UpdateSeatStatusDto SeatStatus) : IRequest;
}
