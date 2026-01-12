using MediatR;
using Seats.Application.Dtos;
using Seats.Application.Queries;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Queries
{
    public class GetSeatByIdQueryHandler : IRequestHandler<GetSeatByIdQuery, Seat?>
    {
        private readonly ISeatRepository _repository;

        public GetSeatByIdQueryHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task<Seat?> Handle(GetSeatByIdQuery request, CancellationToken cancellationToken)
        {
            var seatId = SeatId.Create(request.SeatId);
            var eventId = EventId.Create(request.EventId);
            var functionId = FunctionId.Create(request.FunctionId);
            var zoneId = ZoneId.Create(request.ZoneId);
            var venueId = VenueId.Create(request.VenueId);

            return await _repository.GetByIdAsync(seatId, eventId, functionId, zoneId, venueId);
        }
    }
}
