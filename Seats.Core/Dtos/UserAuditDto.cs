namespace Seats.Core.Dtos
{
    public record UserAuditDto
    {
        public Guid? UserId { get; init; }
        public string Title { get; init; }
        public string JsonData { get; init; }
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
