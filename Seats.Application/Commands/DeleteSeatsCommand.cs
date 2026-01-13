using MediatR;
using Seats.Core.Dtos;

namespace Seats.Application.Commands
{
    public record DeleteSeatsCommand(DeleteSeatsDto DeleteDto) : IRequest;
}
