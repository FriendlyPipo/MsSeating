using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Seats.Core.Dtos;
using Seats.Core.Services;
using Seats.Infrastructure.Services;
using Xunit;

namespace Seats.Test.Infrastructure.Services
{
    public class EventServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<EventService>> _loggerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly HttpClient _httpClient;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://events-service/")
            };

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["EventsUrl"]).Returns("http://events-service/");

            _loggerMock = new Mock<ILogger<EventService>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _tokenServiceMock.Setup(t => t.GetTokenAsync()).ReturnsAsync("mock-token");

            _eventService = new EventService(_httpClient, _configurationMock.Object, _loggerMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task GetZoneByIdAsync_Should_Return_Zone()
        {
            // Arrange
            var zoneId = Guid.NewGuid();
            var zoneDto = new ZoneDto { Id = zoneId, Name = "Test Zone" };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(zoneDto))
                });

            // Act
            var result = await _eventService.GetZoneByIdAsync(zoneId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zoneId, result.Id);

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains($"api/zone/{zoneId}") &&
                    req.Headers.Authorization.Scheme == "Bearer" &&
                    req.Headers.Authorization.Parameter == "mock-token"
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetZoneByIdAsync_Should_Return_Null()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            var result = await _eventService.GetZoneByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}
