using Seats.Domain.ValueObjects;
using Seats.Domain.Primitives;

namespace Seats.Domain.Entities
{
    public class Seat : AggregateRoot
    {
        public SeatId SeatId { get; private set; }
        public EventId EventId { get; private set; }
        public FunctionId FunctionId { get; private set; }
        public ZoneId ZoneId { get; private set; }
        public VenueId VenueId { get; private set; }
        public UserId? UserId { get; private set; }
        public SeatStatus Status { get; private set; }
        public SeatNumber Number { get; private set; }

        protected Seat() { }

        public Seat(SeatId id, EventId eventId, FunctionId functionId, ZoneId zoneId, VenueId venueId, SeatNumber number)
        {
            SeatId = id;
            EventId = eventId;
            FunctionId = functionId;
            ZoneId = zoneId;
            VenueId = venueId;
            Number = number;
            Status = SeatStatus.Disponible;
        }

        public void ChangeStatus(SeatStatus newStatus, UserId? newUserId = null)
        {
            if (newStatus == SeatStatus.Reservado)
            {
                if (Status == SeatStatus.Vendido)
                {
                    throw new InvalidOperationException("El asiento ya está vendido y no puede ser reservado.");
                }

                if (Status == SeatStatus.Reservado && UserId.HasValue && newUserId.HasValue && UserId.Value.Value != newUserId.Value.Value)
                {
                    throw new InvalidOperationException("El asiento ya está reservado por otro usuario.");
                }

                UserId = newUserId;
            }
            else if (newStatus == SeatStatus.Disponible)
            {
                UserId = null;
            }
            else if (newStatus == SeatStatus.Vendido)
            {
                if (Status == SeatStatus.Disponible)
                {
                    throw new InvalidOperationException("El estado de un asiento no puede pasar de disponible a vendido sin una reserva previa.");
                }
                
                if (newUserId.HasValue)
                {
                    UserId = newUserId;
                }
            }

            Status = newStatus;
        }

        public void AssignUser(UserId userId)
        {
            UserId = userId;
        }

        public void RemoveUser()
        {
            UserId = null;
        }
    }
}