using System.Net.Http.Json;

namespace ConfTool.Client.Conferences
{
    public class CountriesService
    {
        private HttpClient _httpClient;
        private string _baseApiUrl;

        public CountriesService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _baseApiUrl = $"api/{config["CountriesBaseUrl"]}";
        }

        public async Task<List<string>> ListCountriesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<string>>(_baseApiUrl);

            return result;
        }
    }

}
