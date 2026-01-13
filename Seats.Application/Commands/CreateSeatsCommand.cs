using MediatR;
using Seats.Core.Dtos;

namespace Seats.Application.Commands
{
    public record CreateSeatsCommand(CreateSeatsDto SeatsDto) : IRequest;
}
