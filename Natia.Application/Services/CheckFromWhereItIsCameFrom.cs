using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Natia.Application.Services;

public class CheckFromWhereItIsCameFrom
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CheckFromWhereItIsCameFrom> _logger;

    public CheckFromWhereItIsCameFrom(HttpClient httpClient, ILogger<CheckFromWhereItIsCameFrom> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int> checkAsync(string emr, string chanellName)
    {
        try
        {
            _logger.LogInformation("Checking source for EMR={Emr}, Channel={Channel}", emr, chanellName);

            string username = "dima";
            string password = "dima123";
            string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            HttpResponseMessage response = await _httpClient.GetAsync($"http://192.168.20.{emr}/mux/mux_config_en.asp");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to retrieve data for EMR={Emr}, Channel={Channel}. StatusCode={StatusCode}", emr, chanellName, response.StatusCode);
                return -1;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            var lis = responseBody.Split(new char[] { '\n' }).Select(io => io.ToLower()).ToList();

            var res = lis.FirstOrDefault(io => io.Contains(chanellName.ToLower()) && io.Contains("card7->phy1"));

            if (res is null)
            {
                _logger.LogWarning("Channel '{Channel}' not found for EMR={Emr}.", chanellName, emr);
                return -1;
            }

            string portInfo = Regex.Match(res, @"port(\d+)").Groups[1].Value;

            if (int.TryParse(portInfo, out var port))
            {
                _logger.LogInformation("Found Channel '{Channel}' for EMR={Emr} on Port={Port}", chanellName, emr, port);
                return port;
            }
            else
            {
                _logger.LogError("Failed to parse port information for EMR={Emr}, Channel={Channel}. RawText={Raw}", emr, chanellName, res);
                return -1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking source for EMR={Emr}, Channel={Channel}", emr, chanellName);
            return -1;
        }
    }
}
