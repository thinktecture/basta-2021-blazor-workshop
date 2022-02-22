using ConfTool.Client.Services;
using ConfTool.Shared.DTO;
using ConfTool.Shared.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using ProtoBuf.Grpc.Client;

namespace ConfTool.Client.Conferences
{
    public partial class ConferenceDetails
    {
        [Parameter] public Guid Id { get; set; }
        [Parameter] public string Mode { get; set; }
        [Inject] private GrpcChannel _channel { get; set; }
        [Inject] private CountriesService _countriesService { get; set; }
        [Inject] private DialogService _dialogService { get; set; }
        [Inject] private NavigationManager _navigationManager { get; set; }

        private IConferencesService _client;
        private ConfTool.Shared.DTO.ConferenceDetails _conferenceDetails;
        private List<string> _countries;
        private bool _isShow => Mode == ConferenceDetailsMode.Show;

        protected override async Task OnInitializedAsync()
        {
            _client = _channel.CreateGrpcService<IConferencesService>();
            _countries = await _countriesService.ListCountriesAsync();
            if (Mode == ConferenceDetailsMode.Show || Mode == ConferenceDetailsMode.Edit)
            {
                _conferenceDetails = await _client.GetConferenceDetailsAsync(new ConferenceDetailsRequest { ID = Id });
            }
            else
            {
                _conferenceDetails = new ConfTool.Shared.DTO.ConferenceDetails
                {
                    DateFrom = DateTime.UtcNow,
                    DateTo = DateTime.UtcNow
                };
            }

            
            await base.OnInitializedAsync();
        }

        private async Task<IEnumerable<string>> SearchCountry(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _countries;

            return _countries.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task SaveConference()
        {
            
            if (!await _dialogService.ConfirmAsync("Do you want to save this new entry?"))
            {
                Console.WriteLine("### User declined to save conference!");
                return;
            }

            if (Mode == ConferenceDetailsMode.New)
            {
                await _client.AddNewConferenceAsync(_conferenceDetails);
                Console.WriteLine("NEW Conference added...");
            }
            _navigationManager.NavigateTo("/conferences");
        }

        private void Cancel()
        {
            _navigationManager.NavigateTo("/conferences");
        }
    }
}