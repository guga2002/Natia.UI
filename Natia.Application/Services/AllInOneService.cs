using Microsoft.Extensions.Logging;
using Natia.Application.Contracts;
using Natia.Core.Entities;
using Natia.Core.Interfaces;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Natia.Application.Services
{
    public class AllInOneService : IAllinOneService
    {
        private readonly IDesclamblerRepository desclambler;
        private readonly IEmr60InfoRepository emr60Info;
        private readonly IRecieverRepository recieverInterface;
        private readonly ILogger<AllInOneService> _logger;

        public AllInOneService(
            IDesclamblerRepository desclambler,
            IEmr60InfoRepository emr60Info,
            IRecieverRepository recieverInterface,
            ILogger<AllInOneService> logger)
        {
            this.desclambler = desclambler;
            this.recieverInterface = recieverInterface;
            this.emr60Info = emr60Info;
            _logger = logger;
        }

        public async Task<Desclamblers?> GetDesclamblerInfoByChanellId(int id)
        {
            try
            {
                _logger.LogInformation("Fetching Desclambler info for Channel ID={ID}", id);
                var res = await desclambler.GetDesclamblerInfoById(id);
                if (res == null)
                {
                    _logger.LogWarning("No Desclambler info found for Channel ID={ID}", id);
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Desclambler info for Channel ID={ID}", id);
                throw;
            }
        }

        public async Task<Emr60Info?> GetInfoByChanellName(string Name)
        {
            try
            {
                _logger.LogInformation("Fetching EMR60 info for Channel Name={Name}", Name);
                var res = await emr60Info.GetEmrInfoByCHanellName(Name);
                if (res == null)
                {
                    _logger.LogWarning("No EMR60 info found for Channel Name={Name}", Name);
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching EMR60 info for Channel Name={Name}", Name);
                throw;
            }
        }

        public Task<Reciever?> GetRecieverInfoByChanellId(int id)
        {
            _logger.LogInformation("Fetching Receiver info for Channel ID={ID}", id);
            return recieverInterface.GetRecieverInfoById(id);
        }

        public async Task<string> GetPort(string Name)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve port info for Channel={Name} via HTTP request", Name);

                HttpClient client = new HttpClient();
                string username = "dima";
                string password = "dima123";
                string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = await client.GetAsync("http://192.168.20.60/en/muxset.asp");
                var lis = new List<string>();

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    lis.AddRange(responseBody.Split(new char[] { '\n' }).Select(io => io.ToLower()));

                    var res = lis.FirstOrDefault(io => io.Contains(Name.ToLower()) && io.Contains("card7->phy1"));
                    if (res is null)
                    {
                        _logger.LogWarning("No matching port entry found for Channel={Name}", Name);
                        return string.Empty;
                    }

                    string portInfo = Regex.Match(res, @"port(\d+)").Groups[1].Value;
                    _logger.LogInformation("Port info found for Channel={Name}: Port={Port}", Name, portInfo);
                    return portInfo;
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve data for Channel={Name}. HTTP Status={Status}", Name, response.StatusCode);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving port info for Channel={Name}", Name);
                throw;
            }
        }
    }
}
