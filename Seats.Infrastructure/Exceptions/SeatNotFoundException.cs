namespace Seats.Infrastructure.Exceptions
{
    public class SeatNotFoundException : Exception
    {
        public SeatNotFoundException(Guid id) 
            : base($"El asiento con Id '{id}' no fue encontrado.")
        {
        }
    }
}
