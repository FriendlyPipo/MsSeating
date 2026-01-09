using MediatR;
using Seats.Application.Commands;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Commands
{
    public class CreateSeatCommandHandler : IRequestHandler<CreateSeatCommand, Guid>
    {
        private readonly ISeatRepository _repository;

        public CreateSeatCommandHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
        {
            var seatId = SeatId.New();
            
            var seat = new Seat(
                seatId,
                EventId.Create(request.Seat.EventId),
                FunctionId.Create(request.Seat.FunctionId),
                ZoneId.Create(request.Seat.ZoneId),
                VenueId.Create(request.Seat.VenueId),
                SeatRow.Create(request.Seat.Row),
                SeatNumber.Create(request.Seat.Number)
            );

            await _repository.AddAsync(seat);

            return seatId.Value;
        }
    }
}
