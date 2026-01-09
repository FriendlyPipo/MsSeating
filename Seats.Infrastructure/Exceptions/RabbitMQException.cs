using System.Diagnostics.CodeAnalysis;

namespace Seats.Infrastructure.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class RabbitMQException : InfrastructureExceptionBase
    {
        public RabbitMQException(string message)
            : base(message) { }
            
        public RabbitMQException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
