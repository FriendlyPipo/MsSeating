using MediatR;
using Seats.Application.Dtos;

namespace Seats.Application.Commands
{
    public record DeleteSeatsCommand(DeleteSeatsDto DeleteDto) : IRequest;
}
