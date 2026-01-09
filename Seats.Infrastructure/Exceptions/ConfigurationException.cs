using System.Diagnostics.CodeAnalysis;

namespace Seats.Infrastructure.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ConfigurationException : InfrastructureExceptionBase
    {
        public ConfigurationException(string message)
            : base(message) { }
    }
}
