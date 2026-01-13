using MediatR;
using Seats.Core.Dtos;

namespace Seats.Application.Commands
{
    public record DeleteSeatCommand(DeleteSeatDto DeleteDto) : IRequest;
}
