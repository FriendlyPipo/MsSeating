using Seats.Core.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seats.Infrastructure.RabbitMQ.Connection
{
    public class RabbitMQBackgroundService : BackgroundService
    {
        private readonly IRabbitMQConsumer _rabbitMQConsumer;
        private readonly ILogger<RabbitMQBackgroundService> _logger;

        public RabbitMQBackgroundService(IRabbitMQConsumer rabbitMQConsumer, ILogger<RabbitMQBackgroundService> logger)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(" Esperando la inicialización de Rabbit");

            await Task.Delay(3000, stoppingToken); 
            await _rabbitMQConsumer.ConsumeMessagesAsync("seats_queue");

            _logger.LogInformation(" Consumidor de Rabbit iniciado.");
        }
    }
}
