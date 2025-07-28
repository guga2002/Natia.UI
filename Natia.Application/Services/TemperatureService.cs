using Microsoft.Extensions.Logging;
using Natia.Application.Contracts;
using System.Text.Json;

namespace Natia.Application.Services;

public class TemperatureService : ITempperatureService
{
    private const string serverUrl = "http://192.168.0.79:2000";
    private readonly ILogger<TemperatureService> _logger;

    public TemperatureService(ILogger<TemperatureService> logger)
    {
        _logger = logger;
    }

    public async Task<(string, string)> GetCurrentData()
    {
        try
        {
            _logger.LogInformation("Fetching current temperature and humidity data from {Url}", serverUrl);

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            using var client = new HttpClient();

            var response = await client.GetAsync($"{serverUrl}/api/Temprature/GetCurrentTemperature");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Temperature API call returned non-success status: {StatusCode}", response.StatusCode);
                return ("0.0", "0.0");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<TemperatureModel>(responseData);

            if (res == null)
            {
                _logger.LogWarning("Failed to deserialize temperature response. Returning default values.");
                return ("0.0", "0.0");
            }

            _logger.LogInformation("Successfully fetched temperature: {Temp} °C, humidity: {Humidity}%", res.temperature, res.humidity);
            return (res.temperature ?? "0.0", res.humidity ?? "0.0");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching temperature data from {Url}", serverUrl);
            return ("0.0", "0.0");
        }
    }

    public class TemperatureModel
    {
        public string? humidity { get; set; }
        public string? temperature { get; set; }
    }
}
