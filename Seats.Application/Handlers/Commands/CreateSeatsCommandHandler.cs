using MediatR;
using Seats.Application.Commands;
using Seats.Application.Dtos;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Commands
{
    public class CreateSeatsCommandHandler : IRequestHandler<CreateSeatsCommand>
    {
        private readonly ISeatRepository _repository;

        public CreateSeatsCommandHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateSeatsCommand request, CancellationToken cancellationToken)
        {
            var seats = new List<Seat>();

            foreach (var seatDto in request.SeatsDto.Seats)
            {
                var seat = new Seat(
                    SeatId.New(),
                    EventId.Create(seatDto.EventId),
                    FunctionId.Create(seatDto.FunctionId),
                    ZoneId.Create(seatDto.ZoneId),
                    VenueId.Create(seatDto.VenueId),
                    SeatRow.Create(seatDto.Row),
                    SeatNumber.Create(seatDto.Number)
                );
                seats.Add(seat);
            }

            await _repository.AddMultipleSeatsAsync(seats);
        }
    }
}
