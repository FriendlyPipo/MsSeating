using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Seats.Core.Dtos;
using Seats.Core.Services;

namespace Seats.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EventService> _logger;
        private readonly ITokenService _tokenService;

        public EventService(HttpClient httpClient, IConfiguration configuration, ILogger<EventService> logger, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _tokenService = tokenService;
            
            var baseUrl = configuration["EventsUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<ZoneDto?> GetZoneByIdAsync(Guid zoneId)
        {
            try
            {
                var token = await _tokenService.GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"api/zone/{zoneId}");
                
                if (response.IsSuccessStatusCode)
                {
                     return await response.Content.ReadFromJsonAsync<ZoneDto>();
                }
                else
                {
                    _logger.LogWarning($"Fallo al obtener zona {zoneId}. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo informacion de la zona {zoneId}");
            }
            return null;
        }
    }
}
