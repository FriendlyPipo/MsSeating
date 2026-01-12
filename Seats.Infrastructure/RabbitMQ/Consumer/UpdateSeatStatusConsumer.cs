using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Seats.Core.RabbitMQ;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;
using Seats.Infrastructure.Database.Context;
using Seats.Infrastructure.Exceptions;

namespace Seats.Infrastructure.RabbitMQ.Consumer
{
    public class UpdateSeatStatusConsumer : IRabbitMQConsumer
    {
        private readonly IConnectionRabbitMQ _rabbitMQConnection;
        private readonly IDbContextFactory<SeatsDbContext> _dbContextFactory;
        private readonly ILogger<UpdateSeatStatusConsumer> _logger;

        public UpdateSeatStatusConsumer(
            IConnectionRabbitMQ rabbitMQConnection,
            IDbContextFactory<SeatsDbContext> dbContextFactory,
            ILogger<UpdateSeatStatusConsumer> logger)
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
                    if (envelope?.EventType == "UpdateSeatStatus")
                    {
                        var updateMessage = JsonConvert.DeserializeObject<EventMessage<UpdateSeatStatusMessageDto>>(raw);
                        if (updateMessage?.Data != null)
                        {
                            await ProcessUpdateSeatStatusAsync(updateMessage.Data);
                            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                        }
                    }
                    else
                    {  await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error procesando mensaje: {ex.Message}");
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation($"Recibiendo en RabbitMQ a travez de:'{queueName}'");
        }

        private async Task ProcessUpdateSeatStatusAsync(UpdateSeatStatusMessageDto updateSeatEvent)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var seatId = SeatId.Create(updateSeatEvent.SeatId);
            var eventId = Seats.Domain.ValueObjects.EventId.Create(updateSeatEvent.EventId);
            // Causaba conflicto con el logger

            var functionId = FunctionId.Create(updateSeatEvent.FunctionId);
            var zoneId = ZoneId.Create(updateSeatEvent.ZoneId);
            var venueId = VenueId.Create(updateSeatEvent.VenueId);
            
            var seat = await context.Seat.FindAsync(seatId, eventId, functionId, zoneId, venueId);

            if (seat != null)
            {
                if (Enum.TryParse<SeatStatus>(updateSeatEvent.Status, true, out var status))
                {
                    seat.ChangeStatus(status);

                    if (status == SeatStatus.Reservado && updateSeatEvent.UserId.HasValue)
                    {
                        seat.AssignUser(UserId.Create(updateSeatEvent.UserId.Value));
                    }
                    else if (status == SeatStatus.Disponible)
                    {
                        seat.RemoveUser();
                    }

                    context.Seat.Update(seat);
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Estado de asiento actualizado a{status}");
                }
            }
            else
            {
                _logger.LogWarning($"El asiento no fue encontrado ID:{seatId.Value}");
            }
        }

        private sealed class UpdateSeatStatusMessageDto
        {
            public Guid SeatId { get; set; }
            public Guid EventId { get; set; }
            public Guid FunctionId { get; set; }
            public Guid ZoneId { get; set; }
            public Guid VenueId { get; set; }
            public string Status { get; set; } = string.Empty;
            public Guid? UserId { get; set; }
        }
    }
}
