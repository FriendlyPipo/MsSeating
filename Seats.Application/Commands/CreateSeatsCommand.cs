using MediatR;
using Seats.Application.Dtos;

namespace Seats.Application.Commands
{
    public record CreateSeatsCommand(CreateSeatsDto SeatsDto) : IRequest;
}
