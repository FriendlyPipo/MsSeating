namespace Seats.Application.Dtos
{
    public record UpdateSeatStatusDto
    {
        public Guid SeatId { get; init; }
        public string Status { get; init; }
    }
}
