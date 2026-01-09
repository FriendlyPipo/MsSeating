using MediatR;
using Seats.Application.Queries;
using Seats.Core.Repositories;
using Seats.Domain.Entities;

namespace Seats.Application.Handlers.Queries
{
    public class GetAllSeatsQueryHandler : IRequestHandler<GetAllSeatsQuery, List<Seat>>
    {
        private readonly ISeatRepository _repository;

        public GetAllSeatsQueryHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Seat>> Handle(GetAllSeatsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}
