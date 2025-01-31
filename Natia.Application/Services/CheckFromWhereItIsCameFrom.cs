using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Natia.Application.Services
{
    public class CheckFromWhereItIsCameFrom
    {
        private readonly HttpClient _httpClient;

        public CheckFromWhereItIsCameFrom(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> checkAsync(string emr, string chanellName)
        {
            string username = "dima";
            string password = "dima123";
            string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            HttpResponseMessage response = await _httpClient.GetAsync($"http://192.168.20.{emr}/mux/mux_config_en.asp");
            var lis = new List<string>();
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                lis.AddRange(responseBody.Split(new char[] { '\n' }).Select(io => io.ToLower()));
                var res = lis.Where(io => io.Contains(chanellName.ToLower()) && io.Contains("card7->phy1")).FirstOrDefault();
                if (res is null)
                {
                    return -1;
                }
                string portInfo = Regex.Match(res, @"port(\d+)").Groups[1].Value;
                return int.Parse(portInfo);
            }
            else
            {
                Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");
                return -1;
            }
        }
    }
}
