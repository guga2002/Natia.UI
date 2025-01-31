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

        public AllInOneService(IDesclamblerRepository desclambler, IEmr60InfoRepository emr60Info, IRecieverRepository recieverInterface)
        {
            this.desclambler = desclambler;
            this.recieverInterface = recieverInterface;
            this.emr60Info = emr60Info;
        }
        public async Task<Desclamblers> GetDesclamblerInfoByChanellId(int id)
        {
            var res = await desclambler.GetDesclamblerInfoById(id);
            return res;
        }

        public async Task<Emr60Info> GetInfoByChanellName(string Name)
        {
            var res = await emr60Info.GetEmrInfoByCHanellName(Name);
            return res;
        }

        public Task<Reciever> GetRecieverInfoByChanellId(int id)
        {
            var res = recieverInterface.GetRecieverInfoById(id);
            return res;
        }

        public async Task<string> GetPort(string Name)
        {
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
                var res = lis.Where(io => io.Contains(Name.ToLower()) && io.Contains("card7->phy1")).FirstOrDefault();
                if (res is null)
                {
                    return null;
                }
                string portInfo = Regex.Match(res, @"port(\d+)").Groups[1].Value;
                return portInfo;
            }
            else
            {
                Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");
                return null;
            }
        }
    }
}
