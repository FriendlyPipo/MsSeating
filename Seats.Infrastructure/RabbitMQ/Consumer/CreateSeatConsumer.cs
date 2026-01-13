using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Seats.Core.RabbitMQ;
using Seats.Domain.Entities;
using Seats.Infrastructure.Database.Context;
using Seats.Core.Exceptions;

namespace Seats.Infrastructure.RabbitMQ.Consumer
{
    using Seats.Domain.ValueObjects; // Generaba conflicto con El logger, por eso lo pongo aqui

    public class CreateSeatConsumer : IRabbitMQConsumer
    {
        private readonly IConnectionRabbitMQ _rabbitMQConnection;
        private readonly IDbContextFactory<SeatsDbContext> _dbContextFactory;
        private readonly ILogger<CreateSeatConsumer> _logger;

        public CreateSeatConsumer(
            IConnectionRabbitMQ rabbitMQConnection,
            IDbContextFactory<SeatsDbContext> dbContextFactory,
            ILogger<CreateSeatConsumer> logger)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _dbContextFactory = dbContextFactory;
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
                    var envelope = JsonConvert.DeserializeObject<EventMessage<object>>(raw);
                    if (envelope?.EventType == "CreateSeat")
                    {
                        var createMessage = JsonConvert.DeserializeObject<EventMessage<SeatMessageDto>>(raw);
                        if (createMessage?.Data != null)
                        {
                            await ProcessCreateSeatAsync(createMessage.Data);
                            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                        }
                    }
                    else
                    {
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error procesando mensaje: {ex.Message}");
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation($"Recibiendo en RabbitMQ a travez de: '{queueName}'");
        }

        private async Task ProcessCreateSeatAsync(SeatMessageDto createSeatEvent)
        {
             using var context = await _dbContextFactory.CreateDbContextAsync();
            var seat = new Seat(
                SeatId.Create(createSeatEvent.SeatId),
                EventId.Create(createSeatEvent.EventId),
                FunctionId.Create(createSeatEvent.FunctionId),
                ZoneId.Create(createSeatEvent.ZoneId),
                VenueId.Create(createSeatEvent.VenueId),
                SeatNumber.Create(createSeatEvent.Number)
            );

            await context.Seat.AddAsync(seat);
            await context.SaveChangesAsync();

            _logger.LogInformation($"Asiento creado exitosamente ID:{seat.SeatId}, Numero:{seat.Number}");
        }

        private sealed class SeatMessageDto
        {
            public Guid SeatId { get; set; }
            public Guid EventId { get; set; }
            public Guid FunctionId { get; set; }
            public Guid ZoneId { get; set; }
            public Guid VenueId { get; set; }
            public int Number { get; set; }
        }
    }
}
