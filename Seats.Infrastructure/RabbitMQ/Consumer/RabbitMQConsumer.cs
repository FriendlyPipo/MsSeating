using AutoMapper;
using Seats.Core.RabbitMQ;
using Seats.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Seats.Infrastructure.RabbitMQ.Consumer
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IConnectionRabbitMQ _rabbitMQConnection;
        private readonly IDbContextFactory<SeatsDbContext> _dbContextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<RabbitMQConsumer> _logger;

        public RabbitMQConsumer(
            IConnectionRabbitMQ rabbitMQConnection,
            IDbContextFactory<SeatsDbContext> dbContextFactory,
            IMapper mapper,
            ILogger<RabbitMQConsumer> logger
        )
        {
            _rabbitMQConnection = rabbitMQConnection;
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task ConsumeMessagesAsync(string queueName)
        {
            var channel = _rabbitMQConnection.GetChannel();
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var raw = Encoding.UTF8.GetString(body);

                try
                {
                    var envelope = JsonConvert.DeserializeObject<BaseEventMessage>(raw);
                    if (envelope is null || string.IsNullOrWhiteSpace(envelope.EventType))
                        throw new InvalidOperationException("Mensaje sin EventType.");

                    switch (envelope.EventType)
                    {
                        default:
                            _logger.LogInformation($"Evento ignorado por el Tipo: {envelope.EventType}");
                            break;
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                        _logger.LogError($"[RabbitMQConsumer] Error procesando mensaje: {ex.Message}");
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation($"[RabbitMQConsumer] Escuchando cola '{queueName}'...");
        }

        private sealed class BaseEventMessage
        {
            public string? EventType { get; set; }
        }
    }
}
