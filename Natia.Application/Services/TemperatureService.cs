using Microsoft.Extensions.Configuration;
using Natia.Application.Contracts;
using System.Text.Json;

namespace Natia.Application.Services;

public class TemperatureService : ITempperatureService
{

    private const string serverUrl = "https://192.168.0.79:2000";

    public async Task<(string, string)> GetCurrentData()
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);

            var response = await client.GetAsync($"{serverUrl}/api/Temprature/GetCurrentTemperature");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();

            var res = JsonSerializer.Deserialize<TemperatureModel>(responseData);
            if (res == null)
            {
                return ("0.0", "0.0");
            }
            return (res.temperature,res.humidity);
        }
        catch (Exception ex)
        {
            return ("0.0", "0.0");
        }
    }

    public class TemperatureModel
    {
        public string? humidity { get; set; }
        public string? temperature { get; set; }
    }
}
