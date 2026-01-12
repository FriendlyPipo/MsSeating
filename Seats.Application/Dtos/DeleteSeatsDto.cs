namespace Seats.Application.Dtos
{
    public record DeleteSeatsDto
    {
        public int Quantity { get; init; }
        public Guid EventId { get; init; }
        public Guid FunctionId { get; init; }
        public Guid ZoneId { get; init; }
        public Guid VenueId { get; init; }
        public int Row { get; init; }
    }
}
