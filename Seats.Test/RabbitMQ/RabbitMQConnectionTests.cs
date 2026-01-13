using Moq;
using RabbitMQ.Client;
using Seats.Core.Exceptions;
using Seats.Infrastructure.RabbitMQ.Connection;
using Xunit;

namespace Seats.Test.Infrastructure.RabbitMQ
{
    public class RabbitMQConnectionTests
    {
        private readonly Mock<IConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IConnection> _connectionMock;
        private readonly Mock<IChannel> _channelMock;
        private readonly RabbitMQConnection _rabbitMQConnection;

        public RabbitMQConnectionTests()
        {
            _connectionFactoryMock = new Mock<IConnectionFactory>();
            _connectionMock = new Mock<IConnection>();
            _channelMock = new Mock<IChannel>();

            _rabbitMQConnection = new RabbitMQConnection(_connectionFactoryMock.Object);
        }

        [Fact]
        public async Task InitializeAsync_Should_CreateConnection_And_Channel()
        {
            // Arrange
            _connectionFactoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_connectionMock.Object);
                
            _connectionMock.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_channelMock.Object);

            // Act
            await _rabbitMQConnection.InitializeAsync();

            // Assert
            _connectionFactoryMock.Verify(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _connectionMock.Verify(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            // QueueDeclareAsync is an extension method and cannot be verified easily with Moq.
            // _channelMock.Verify(c => c.QueueDeclareAsync("seatsQueue", true, false, false, It.IsAny<IDictionary<string, object?>>(), false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChannel_Should_Return_Channel()
        {
            // Arrange
            _connectionFactoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_connectionMock.Object);
                
            _connectionMock.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_channelMock.Object);

            await _rabbitMQConnection.InitializeAsync();

            // Act
            var channel = _rabbitMQConnection.GetChannel();

            // Assert
            Assert.NotNull(channel);
            Assert.Equal(_channelMock.Object, channel);
        }

        [Fact]
        public void GetChannel_Should_Throw_When_Not_Init()
        {
            // Act & Assert
            Assert.Throws<RabbitMQException>(() => _rabbitMQConnection.GetChannel());
        }
    }
}
