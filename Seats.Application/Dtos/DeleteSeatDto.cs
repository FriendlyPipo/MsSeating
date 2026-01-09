namespace Seats.Application.Dtos
{
    public record DeleteSeatDto
    {
        public Guid SeatId { get; init; }
    }
}
