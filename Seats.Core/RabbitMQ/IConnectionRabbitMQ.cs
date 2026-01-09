using RabbitMQ.Client;

namespace Seats.Core.RabbitMQ
{
    public interface IConnectionRabbitMQ
    {
        Task InitializeAsync();
        IChannel GetChannel();
    }
}
