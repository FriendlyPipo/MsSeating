namespace Seats.Core.Dtos
{
    public record UserLogDto
    {
        public Guid? UserId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
    }
}
