namespace Seats.Core.Dtos
{
    public record CreateSeatsDto
    {
        public int Quantity { get; init; }
        public Guid EventId { get; init; }
        public Guid FunctionId { get; init; }
        public Guid ZoneId { get; init; }
        public Guid VenueId { get; init; }
        public Guid? UserId { get; init; }
    }
}
