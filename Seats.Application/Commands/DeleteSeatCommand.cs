using MediatR;
using Seats.Application.Dtos;

namespace Seats.Application.Commands
{
    public record DeleteSeatCommand(DeleteSeatDto DeleteDto) : IRequest;
}
