using Seats.Core.RabbitMQ;

namespace Seats.Infrastructure.RabbitMQ.Consumer
{
    public class CompositeRabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IEnumerable<IRabbitMQConsumer> _consumers;

        public CompositeRabbitMQConsumer(IEnumerable<IRabbitMQConsumer> consumers)
        {
            _consumers = consumers;
        }

        public async Task ConsumeMessagesAsync(String queueName)
        {
            var tasks = _consumers.Select(c => c.ConsumeMessagesAsync(queueName));
            await Task.WhenAll(tasks);
        }
    }
}
