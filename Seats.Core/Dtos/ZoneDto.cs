namespace Seats.Core.Dtos
{
    public class ZoneDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public Guid EventId { get; set; }
    }
}
