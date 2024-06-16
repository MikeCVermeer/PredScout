using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PredScout
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://omeda.city") }; // Replace with the actual base URL of the API
        }

        public async Task<JObject> GetPlayerStatistics(string playerId)
        {
            var response = await _httpClient.GetAsync($"/players/{playerId}/statistics.json");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        public async Task<JObject> GetPlayerHeroStatistics(string playerId, int heroId)
        {
            var response = await _httpClient.GetAsync($"/players/{playerId}/hero_statistics.json?hero_ids=[{heroId}]");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        public async Task<JObject> GetPlayerRank(string playerId)
        {
            var response = await _httpClient.GetAsync($"/players/{playerId}.json");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
    }
}
