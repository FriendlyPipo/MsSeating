namespace Seats.Application.Dtos
{
    public record CreateSeatDto
    {
        public Guid EventId { get; init; }
        public Guid FunctionId { get; init; }
        public Guid ZoneId { get; init; }
        public Guid VenueId { get; init; }
        public string Row { get; init; }
        public int Number { get; init; }
    }
}
