namespace Seats.Core.Exceptions
{
    public class InvalidSeatStatusException : Exception
    {
        public InvalidSeatStatusException(string status) 
            : base($"El estado '{status}' es inválido.")
        {
        }
    }
}
