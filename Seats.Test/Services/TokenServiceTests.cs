using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Seats.Infrastructure.Services;
using Xunit;

namespace Seats.Test.Infrastructure.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            
            _configurationMock = new Mock<IConfiguration>();
            // Setup configuration mock for Keycloak section
            var keycloakSection = new Mock<IConfigurationSection>();
            keycloakSection.Setup(s => s["BaseUrl"]).Returns("http://keycloak");
            keycloakSection.Setup(s => s["Realm"]).Returns("test-realm");
            keycloakSection.Setup(s => s["ClientId"]).Returns("test-client");
            keycloakSection.Setup(s => s["ClientSecret"]).Returns("test-secret");
            
            _configurationMock.Setup(c => c.GetSection("Keycloak")).Returns(keycloakSection.Object);

            _tokenService = new TokenService(_httpClient, _configurationMock.Object);
        }

        [Fact]
        public async Task GetTokenAsync_Should_Return_Token()
        {
            // Arrange
            var tokenResponse = new
            {
                access_token = "test-token",
                expires_in = 3600
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
                });

            // Act
            var token = await _tokenService.GetTokenAsync();

            // Assert
            Assert.Equal("test-token", token);
            
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == "http://keycloak/realms/test-realm/protocol/openid-connect/token"
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetTokenAsync_Should_Return_CachedToken()
        {
            // Arrange
            var tokenResponse = new
            {
                access_token = "test-token-cached",
                expires_in = 3600
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
                });

            // Act
            var token1 = await _tokenService.GetTokenAsync();
            var token2 = await _tokenService.GetTokenAsync(); 

            // Assert
            Assert.Equal("test-token-cached", token1);
            Assert.Equal("test-token-cached", token2);

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
