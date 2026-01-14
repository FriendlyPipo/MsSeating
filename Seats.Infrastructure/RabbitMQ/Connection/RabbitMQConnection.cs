using Seats.Core.RabbitMQ;
using RabbitMQ.Client;
using Seats.Core.Exceptions;

namespace Seats.Infrastructure.RabbitMQ.Connection
{
    public class RabbitMQConnection : IConnectionRabbitMQ
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync()
        {
            try 
            {
                _connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw new RabbitMQException("Error al conectar con el servidor RabbitMQ.", ex);
            }

            if (_connection == null)
            {
                throw new RabbitMQException("No se pudo establecer la conexión con RabbitMQ.");
            }

            _channel = await _connection.CreateChannelAsync();

            if (_channel == null)
            {
                throw new RabbitMQException("No se pudo crear el canal de comunicación con RabbitMQ.");
            }

            await _channel.QueueDeclareAsync("seats_queue", true, false, false);
        }

        public IChannel GetChannel()
        {
            if (_channel == null)
            {
                throw new RabbitMQException("RabbitMQ aún no está inicializado correctamente.");
            }
            return _channel;
        }

    }
}
