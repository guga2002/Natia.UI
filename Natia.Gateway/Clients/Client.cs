using Natia.Persistance.Model;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Natia.Gateway.Clients;

public partial class Client
{
    private readonly ILogger<Client> _logger;

    public Client(ILogger<Client> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetSystemHealth()
    {
        var requestUrl = "http://192.168.1.102:3395/api/CheckHealth/health";
        try
        {
            _logger.LogInformation("Requesting system health from {Url}", requestUrl);

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient();

            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode && res.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                _logger.LogInformation("System health check succeeded.");
                return resultString ?? string.Empty;
            }

            _logger.LogWarning("System health check failed. Status: {StatusCode}", res.StatusCode);
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch system health from {Url}", requestUrl);
            return string.Empty;
        }
    }

    public async Task<string> CheckForAnnomalies()
    {
        var requestUrl = "http://192.168.1.102:3395/api/CheckAnommaliesOverSystem/CheckForAnomalies";
        try
        {
            _logger.LogInformation("Checking for anomalies from {Url}", requestUrl);
            var client = new HttpClient();
            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode && res.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                _logger.LogInformation("Anomalies check successful.");
                return resultString ?? string.Empty;
            }

            _logger.LogWarning("Anomalies check failed. Status: {StatusCode}", res.StatusCode);
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for anomalies at {Url}", requestUrl);
            return string.Empty;
        }
    }

    public async Task<ICollection<string>> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        var requestUrl = "http://192.168.1.102:3395/api/NatiaCore/info";
        try
        {
            _logger.LogInformation("Fetching info from {Url}", requestUrl);

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient();
            var res = await client.GetAsync(requestUrl, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<ICollection<string>>(resultString);
                _logger.LogInformation("Fetched info successfully.");
                return result ?? new List<string>();
            }

            _logger.LogWarning("Failed to fetch info from {Url}. Status: {StatusCode}", requestUrl, res.StatusCode);
            throw new CommonException("Error occurred", 500, "Fix API client");
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "Error occurred while fetching info from {Url}", requestUrl);
            throw;
        }
    }

    public async Task<string> GetAnniversaryDates(CancellationToken cancellationToken = default)
    {
        var requestUrl = "http://192.168.1.102:3395/api/NatiaCore/GetAnniversaryDates";
        try
        {
            _logger.LogInformation("Fetching anniversary dates from {Url}", requestUrl);
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true };
            var client = new HttpClient();
            var res = await client.GetAsync(requestUrl, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                _logger.LogInformation("Fetched anniversary dates successfully.");
                return resultString ?? string.Empty;
            }

            _logger.LogWarning("Failed to fetch anniversary dates. Status: {StatusCode}", res.StatusCode);
            return string.Empty;
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "Error occurred while fetching anniversary dates from {Url}", requestUrl);
            throw;
        }
    }

    public async Task<bool> MakeNatiaSpeake(string sityva)
    {
        var requestUrl = $@"http://192.168.1.102:3395/Robot/start?sentence={sityva}";
        try
        {
            _logger.LogInformation("Sending Natia to speak: {Text}", sityva);
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true };
            var client = new HttpClient();
            var res = await client.GetAsync(requestUrl);

            var success = res.IsSuccessStatusCode;
            _logger.LogInformation("Natia speech command {Result} (Status: {StatusCode})", success ? "succeeded" : "failed", res.StatusCode);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Natia speech command: {Text}", sityva);
            throw;
        }
    }

    public async Task<List<ExcellDataMode3l>> GetExcelInfo()
    {
        var requestUrl = "http://192.168.0.13:9999/api/ExcelData";
        try
        {
            _logger.LogInformation("Fetching Excel data from {Url}", requestUrl);
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true };
            var client = new HttpClient(handler);
            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<List<ExcellDataMode3l>>(resultString);
                _logger.LogInformation("Fetched Excel data successfully.");
                return result ?? new List<ExcellDataMode3l>();
            }

            _logger.LogWarning("Failed to fetch Excel data. Status: {StatusCode}", res.StatusCode);
            throw new CommonException("Error occurred", 500, "Fix API client");
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "Error fetching Excel data from {Url}", requestUrl);
            return new List<ExcellDataMode3l>();
        }
    }

    public async Task<List<string>> GetCardWhichNeedActivate(CancellationToken cancellationToken = default)
    {
        var requestUrl = "http://192.168.1.102:3395/api/Descrambler/CardsThatNeedToBeActivated";
        try
        {
            _logger.LogInformation("Fetching cards needing activation from {Url}", requestUrl);
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true };
            var client = new HttpClient();
            var res = await client.GetAsync(requestUrl, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<string>>(resultString);
                _logger.LogInformation("Fetched {Count} cards for activation.", result?.Count ?? 0);
                return result ?? throw new ArgumentException("Shecdoma");
            }

            _logger.LogWarning("Failed to fetch cards for activation. Status: {StatusCode}", res.StatusCode);
            return new List<string>();
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "Error fetching activation cards from {Url}", requestUrl);
            throw;
        }
    }

    public async Task<string> GetTextToSentInMail(CancellationToken cancellationToken = default)
    {
        var requestUrl = "http://192.168.1.102:3395/api/RegionData";
        try
        {
            _logger.LogInformation("Fetching text to send in mail from {Url}", requestUrl);
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true };
            var client = new HttpClient();
            var res = await client.GetAsync(requestUrl, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<string>(resultString);
                _logger.LogInformation("Fetched mail text successfully.");
                return result ?? string.Empty;
            }

            _logger.LogWarning("Failed to fetch mail text. Status: {StatusCode}", res.StatusCode);
            return string.Empty;
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "Error fetching mail text from {Url}", requestUrl);
            throw;
        }
    }

    public async Task<DateTime> HeartBeat(CancellationToken cancellationToken = default)
    {
        var requestUrl = "http://192.168.0.79:2000/api/controll/heartbeat/true";
        try
        {
            _logger.LogInformation("Sending heartbeat request to {Url}", requestUrl);
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true };
            var client = new HttpClient();
            var res = await client.GetAsync(requestUrl, cancellationToken);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<DateTime>(resultString);
                _logger.LogInformation("Heartbeat successful. Last beat: {BeatTime}", result);
                return result;
            }

            _logger.LogWarning("Heartbeat request failed. Status: {StatusCode}", res.StatusCode);
            return DateTime.MinValue;
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "Heartbeat request failed for {Url}", requestUrl);
            throw;
        }
    }

    public class CommonException : Exception
    {
        public int StatusCode { get; }
        public string Response { get; }

        public CommonException(string message, int statusCode, string response)
            : base($"{message}\n\nStatus: {statusCode}\nResponse: \n{response}")
        {
            StatusCode = statusCode;
            Response = response;
        }
    }
}
