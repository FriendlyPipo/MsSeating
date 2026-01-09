using MediatR;
using Seats.Application.Commands;
using Seats.Core.Repositories;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Commands
{
    public class DeleteSeatsCommandHandler : IRequestHandler<DeleteSeatsCommand>
    {
        private readonly ISeatRepository _repository;

        public DeleteSeatsCommandHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteSeatsCommand request, CancellationToken cancellationToken)
        {
            var seatIds = request.DeleteDto.SeatIds.Select(SeatId.Create).ToList();
            await _repository.DeleteMultipleSeatsAsync(seatIds);
        }
    }
}
