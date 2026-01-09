using MediatR;
using Seats.Application.Exceptions;
using Seats.Application.Commands;
using Seats.Core.Repositories;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Commands
{
    public class DeleteSeatCommandHandler : IRequestHandler<DeleteSeatCommand>
    {
        private readonly ISeatRepository _repository;

        public DeleteSeatCommandHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteSeatCommand request, CancellationToken cancellationToken)
        {
            var seatId = SeatId.Create(request.DeleteDto.SeatId);
            
            await _repository.DeleteAsync(seatId);
        }
    }
}
