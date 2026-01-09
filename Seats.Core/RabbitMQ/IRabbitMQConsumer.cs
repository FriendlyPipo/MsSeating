namespace Seats.Core.RabbitMQ
{
    public interface IRabbitMQConsumer
    {
        Task ConsumeMessagesAsync(string queueName);
    }
}
