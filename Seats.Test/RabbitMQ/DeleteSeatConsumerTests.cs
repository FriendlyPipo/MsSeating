using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Seats.Core.RabbitMQ;
using Seats.Infrastructure.Database.Context;
using Seats.Infrastructure.RabbitMQ.Consumer;
using Xunit;

namespace Seats.Test.Infrastructure.RabbitMQ
{
    public class DeleteSeatConsumerTests
    {
        private readonly Mock<IConnectionRabbitMQ> _connectionMock;
        private readonly Mock<IChannel> _channelMock;
        private readonly Mock<IDbContextFactory<SeatsDbContext>> _dbContextFactoryMock;
        private readonly Mock<ILogger<DeleteSeatConsumer>> _loggerMock;
        private readonly DeleteSeatConsumer _consumer;

        public DeleteSeatConsumerTests()
        {
            _connectionMock = new Mock<IConnectionRabbitMQ>();
            _channelMock = new Mock<IChannel>();
            _dbContextFactoryMock = new Mock<IDbContextFactory<SeatsDbContext>>();
            _loggerMock = new Mock<ILogger<DeleteSeatConsumer>>();

            _connectionMock.Setup(c => c.GetChannel()).Returns(_channelMock.Object);

            _consumer = new DeleteSeatConsumer(_connectionMock.Object, _dbContextFactoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ConsumeMessagesAsync_Should_Consume()
        {
            // Act
            await _consumer.ConsumeMessagesAsync("testQueue");

            // Assert
            _channelMock.Verify(c => c.BasicConsumeAsync(
                "testQueue", 
                false, 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<IDictionary<string, object?>>(), 
                It.IsAny<IAsyncBasicConsumer>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
