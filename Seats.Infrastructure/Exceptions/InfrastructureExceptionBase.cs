using System.Diagnostics.CodeAnalysis;

namespace Seats.Infrastructure.Exceptions
{
    [ExcludeFromCodeCoverage]
    public abstract class InfrastructureExceptionBase : Exception
    {
        protected InfrastructureExceptionBase(string message)
            : base(message) { }

        protected InfrastructureExceptionBase(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
