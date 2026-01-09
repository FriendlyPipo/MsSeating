namespace Seats.Application.Dtos
{
    public record DeleteSeatsDto
    {
        public List<Guid> SeatIds { get; init; } = new();
    }
}
