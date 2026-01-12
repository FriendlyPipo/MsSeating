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
    public class DeleteSeatConsumer : IRabbitMQConsumer
    {
        private readonly IConnectionRabbitMQ _rabbitMQConnection;
        private readonly IDbContextFactory<SeatsDbContext> _dbContextFactory;
        private readonly ILogger<DeleteSeatConsumer> _logger;

        public DeleteSeatConsumer(
            IConnectionRabbitMQ rabbitMQConnection,
            IDbContextFactory<SeatsDbContext> dbContextFactory,
            ILogger<DeleteSeatConsumer> logger)
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
                    if (envelope?.EventType == "DeleteSeat")
                    {
                        var deleteMessage = JsonConvert.DeserializeObject<EventMessage<DeleteSeatMessageDto>>(raw);
                        if (deleteMessage?.Data != null)
                        {
                            await ProcessDeleteSeatAsync(deleteMessage.Data);
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
            _logger.LogInformation($"Recibiendo en RabbitMQ a travez de:'{queueName}'");
        }

        private async Task ProcessDeleteSeatAsync(DeleteSeatMessageDto deleteSeatEvent)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var seatId = SeatId.Create(deleteSeatEvent.SeatId);
            var eventId = Seats.Domain.ValueObjects.EventId.Create(deleteSeatEvent.EventId);
            var functionId = FunctionId.Create(deleteSeatEvent.FunctionId);
            var zoneId = ZoneId.Create(deleteSeatEvent.ZoneId);
            var venueId = VenueId.Create(deleteSeatEvent.VenueId);
            
            var seat = await context.Seat.FindAsync(seatId, eventId, functionId, zoneId, venueId);
            if (seat != null)
            {
                context.Seat.Remove(seat);
                await context.SaveChangesAsync();
                _logger.LogInformation($"Asiento eliminado ID:{seat.SeatId.Value}");
            }
            else
            {
                _logger.LogWarning($"El asiento no fue encontrado ID:{seatId.Value}");
            }
        }

        private sealed class DeleteSeatMessageDto
        {
             public Guid SeatId { get; set; }
             public Guid EventId { get; set; }
             public Guid FunctionId { get; set; }
             public Guid ZoneId { get; set; }
             public Guid VenueId { get; set; }
        }
    }
}
