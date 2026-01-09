namespace Seats.Application.Dtos
{
    public record CreateSeatsDto
    {
        public List<CreateSeatDto> Seats { get; init; } = new();
    }
}
