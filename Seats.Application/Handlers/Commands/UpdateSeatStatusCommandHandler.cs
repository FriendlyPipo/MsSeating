using MediatR;
using Seats.Application.Exceptions;
using Seats.Application.Commands;
using Seats.Core.Repositories;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Application.Handlers.Commands
{
    public class UpdateSeatStatusCommandHandler : IRequestHandler<UpdateSeatStatusCommand>
    {
        private readonly ISeatRepository _repository;

        public UpdateSeatStatusCommandHandler(ISeatRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateSeatStatusCommand request, CancellationToken cancellationToken)
        {
            var seatId = SeatId.Create(request.SeatStatus.SeatId);
            var seat = await _repository.GetByIdAsync(seatId);

            if (seat == null)
            {
                throw new SeatNotFoundException(request.SeatStatus.SeatId); 
            }

            if (!Enum.TryParse<SeatStatus>(request.SeatStatus.Status, true, out var status))
            {
                 throw new ArgumentException($"Estado inválido: {request.SeatStatus.Status}");
            }

            seat.ChangeStatus(status);


            await _repository.UpdateAsync(seat, cancellationToken);
        }
    }
}
