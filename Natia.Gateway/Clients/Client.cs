namespace Natia.Gateway.Clients;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Natia.Persistance.Model;
using Newtonsoft.Json;

public partial class Client
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _jsonSettings;

    public Client(string baseUrl, HttpClient httpClient)
    {
        if (string.IsNullOrEmpty(baseUrl))
            throw new ArgumentNullException(nameof(baseUrl));
        if (httpClient == null)
            throw new ArgumentNullException(nameof(httpClient));

        _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
        _httpClient = httpClient;
        _jsonSettings = new JsonSerializerSettings();

        UpdateJsonSerializerSettings(_jsonSettings);
    }

    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings);


    public async Task<string> GetSystemHealth()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        var client = new HttpClient(handler);
        var requestUrl = "https://192.168.1.102:3999/api/CheckHealth/CheckSystemHealth";

        var res = await client.GetAsync(requestUrl);

        if (res.IsSuccessStatusCode)
        {
            var resultString = await res.Content.ReadAsStringAsync();

            return resultString ?? string.Empty;
        }
        else
        {
            return string.Empty;
        }
    }

    public async Task<string> CheckForAnnomalies()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        var client = new HttpClient(handler);
        var requestUrl = "https://192.168.1.102:3999/api/CheckAnommaliesOverSystem/CheckForAnomalies";

        var res = await client.GetAsync(requestUrl);

        if (res.IsSuccessStatusCode)
        {
            var resultString = await res.Content.ReadAsStringAsync();

            return resultString ?? string.Empty;
        }
        else
        {
            return string.Empty;
        }
    }


    public async Task<ICollection<string>> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            var requestUrl = "https://192.168.1.102:3999/api/NatiaCore/info";

            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<ICollection<string>>(resultString);

                return result ?? new List<string>();
            }
            else
            {
                throw new CommonException("Error occurred", 500, "Fix API client");
            }
        }
        catch (Exception exp)
        {
            await Console.Out.WriteLineAsync(exp.StackTrace);
            throw;
        }
    }

    //

    public async Task<string> GetAnniversaryDates(CancellationToken cancellationToken = default)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            var requestUrl = "https://192.168.1.102:3999/api/NatiaCore/GetAnniversaryDates";

            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();

                return resultString ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception exp)
        {
            await Console.Out.WriteLineAsync(exp.StackTrace);
            throw;
        }
    }

    public async Task<bool> MakeNatiaSpeake(string sityva)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            var requestUrl = $@"https://192.168.1.102:3999/Robot/start?sentence={sityva}";

            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<ExcellDataMode3l>> GetExcelInfo()
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            var requestUrl = "http://192.168.0.13:9999/api/ExcelData";

            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<List<ExcellDataMode3l>>(resultString);

                return result ?? new List<ExcellDataMode3l>();
            }
            else
            {
                throw new CommonException("Error occurred", 500, "Fix API client");
            }
        }
        catch (Exception exp)
        {
            await Console.Out.WriteLineAsync(exp.StackTrace);
            return new List<ExcellDataMode3l>();
        }
    }

    public async Task<List<string>> GetCardWhichNeedActivate(CancellationToken cancellationToken = default)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            var requestUrl = "https://192.168.1.102:3999/api/Descrambler/CardsThatNeedToBeActivated";

            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<string>>(resultString);
                return result ?? throw new ArgumentException("Shecdoma");
            }
            else
            {
                return new List<string>();
            }
        }
        catch (Exception exp)
        {
            await Console.Out.WriteLineAsync(exp.StackTrace);
            throw;
        }
    }

    public async Task<string> GetTextToSentInMail(CancellationToken cancellationToken = default)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            var requestUrl = "https://192.168.1.102:3999/api/RegionChecker/GetRegionData";

            var res = await client.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<string>(resultString);
                return result ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception exp)
        {
            await Console.Out.WriteLineAsync(exp.StackTrace);
            throw;
        }
    }


    public async Task<DateTime> HeartBeat(CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUrl = "http://192.168.1.102:3999/api/controll/heartbeat/true";

            var res = await _httpClient.GetAsync(requestUrl);

            if (res.IsSuccessStatusCode)
            {
                var resultString = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<DateTime>(resultString);
                return result;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        catch (Exception exp)
        {
            await Console.Out.WriteLineAsync(exp.StackTrace);
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
