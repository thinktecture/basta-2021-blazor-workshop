using ConfTool.Shared.DTO;
using System.Net.Http.Json;

namespace ConfTool.Client.Conferences
{
    public class ConferencesService
    {
        private HttpClient _httpClient;
        private string _conferencesUrl = string.Empty;

        public ConferencesService(IConfiguration configuration, HttpClient httpClient)
        {
            _conferencesUrl = $"api/{configuration.GetValue<string>("ConferencesBaseUrl")}";
            _httpClient = httpClient;
        }

        public async Task<List<ConferenceOverview>> GetConferencesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ConferenceOverview>>(_conferencesUrl);

            return result;
        }

        public async Task<ConfTool.Shared.DTO.ConferenceDetails> GetConferenceDetailsAsync(Guid id)
        {
            var result = await _httpClient.GetFromJsonAsync<ConfTool.Shared.DTO.ConferenceDetails>(_conferencesUrl + id);

            return result;
        }

        public async Task<ConferenceDetails> AddConferenceAsync(ConfTool.Shared.DTO.ConferenceDetails details)
        {
            try
            {
                var httpResult = await _httpClient.PostAsJsonAsync(_conferencesUrl, details);
                return await httpResult.Content.ReadFromJsonAsync<ConferenceDetails>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
