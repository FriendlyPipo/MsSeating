namespace Seats.Core.Dtos
{
    public record UpdateSeatStatusDto
    {
        public Guid SeatId { get; init; }
        public Guid EventId { get; init; }
        public Guid FunctionId { get; init; }
        public Guid ZoneId { get; init; }
        public Guid VenueId { get; init; }
        public string Status { get; init; }
        public Guid? UserId { get; init; }
    }
}
