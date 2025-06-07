using Microsoft.Extensions.Configuration;
using Natia.Application.Contracts;

namespace Natia.Application.Services;

public class TemperatureService : ITempperatureService
{
    private readonly HttpClient client;
    private readonly IConfiguration _config;
    public TemperatureService(HttpClient client, IConfiguration config)
    {
        this.client = client;
        _config = config;
    }


    public async Task<(string, string)> GetCurrentData()
    {
        try
        {
            var response = await client.GetAsync($"http://192.168.100.104:8080/monitoring");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var res = responseData.Split(new string[] { "<div id=\"temperature\" class=\"temperature\">", "°C</div>" }, StringSplitOptions.None)[1];
            var Humidity = responseData.Split(new string[] { "<div class=\"humidity\">", "%</div>" }, StringSplitOptions.None)[1];
            return (res, Humidity);
        }
        catch
        {
            return ("შეცდომა ტემპერატურის წამოღებისას", "");
        }
    }
}
