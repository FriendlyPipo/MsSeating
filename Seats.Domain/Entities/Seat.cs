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
        public SeatRow Row { get; private set; }

        protected Seat() { }

        public Seat(SeatId id, EventId eventId, FunctionId functionId, ZoneId zoneId, VenueId venueId, SeatRow row, SeatNumber number)
        {
            SeatId = id;
            EventId = eventId;
            FunctionId = functionId;
            ZoneId = zoneId;
            VenueId = venueId;
            Row = row;
            Number = number;
            Status = SeatStatus.Disponible;
        }

        public void ChangeStatus(SeatStatus newStatus)
        {
            if (Status == SeatStatus.Disponible && newStatus == SeatStatus.Vendido)
            {
                throw new InvalidOperationException("El estado de un asiento no puede pasar de disponible a vendido");
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