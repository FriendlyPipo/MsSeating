using MediatR;
using Seats.Application.Queries;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Queries
{
    public class GetSeatsByZoneQueryHandler : IRequestHandler<GetSeatsByZoneQuery, List<Seat>>
    {
        private readonly ISeatRepository _repository;

        public GetSeatsByZoneQueryHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Seat>> Handle(GetSeatsByZoneQuery request, CancellationToken cancellationToken)
        {
            var zoneId = ZoneId.Create(request.ZoneId);
            var eventId = EventId.Create(request.EventId);
            var functionId = FunctionId.Create(request.FunctionId);
            var venueId = VenueId.Create(request.VenueId);

            return await _repository.GetSeatByZoneAsync(zoneId, eventId, functionId, venueId);
        }
    }
}
