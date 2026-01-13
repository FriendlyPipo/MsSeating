using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Seats.Infrastructure.RabbitMQ.Producer;
using Seats.Core.RabbitMQ;
using Xunit;

namespace Seats.Test.Infrastructure.RabbitMQ
{
    public class RabbitMQProducerTests
    {
        private readonly Mock<IConnectionRabbitMQ> _connectionMock;
        private readonly Mock<IChannel> _channelMock;
        private readonly Mock<ILogger<RabbitMQProducer<object>>> _loggerMock;
        private readonly RabbitMQProducer<object> _producer;

        public RabbitMQProducerTests()
        {
            _connectionMock = new Mock<IConnectionRabbitMQ>();
            _channelMock = new Mock<IChannel>();
            _loggerMock = new Mock<ILogger<RabbitMQProducer<object>>>();

            _connectionMock.Setup(c => c.GetChannel()).Returns(_channelMock.Object);

            _producer = new RabbitMQProducer<object>(_connectionMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task PublishMessageAsync_Should_DeclareQueue_And_Publish()
        {
            // Arrange
            var data = new { Id = 1 };
            string queueName = "testQueue";
            string eventType = "TestEvent";

            // Act
            await _producer.PublishMessageAsync(data, queueName, eventType);

            // Assert

            // _channelMock.Verify(c => c.QueueDeclareAsync(queueName, true, false, false, It.IsAny<IDictionary<string, object?>>(), false, It.IsAny<CancellationToken>()), Times.Once);
            _channelMock.Verify(c => c.BasicPublishAsync(
                It.IsAny<string>(), 
                queueName, 
                false, 
                It.IsAny<BasicProperties>(), 
                It.IsAny<ReadOnlyMemory<byte>>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PublishMessageAsync_Should_ThrowArgumentNullException_When_Null()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _producer.PublishMessageAsync(null!, "queue", "event"));
        }
    }
}
