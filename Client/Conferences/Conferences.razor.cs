using ConfTool.Shared.DTO;
using ConfTool.Shared.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using ProtoBuf.Grpc.Client;
using System.Net.Http.Json;

namespace ConfTool.Client.Conferences
{
    public partial class Conferences
    {
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private GrpcChannel _channel { get; set; }

        private List<ConferenceOverview>? _conferences;
        private HubConnection _hubConnection;
        private IConferencesService _client;
        private Guid _newId;

        protected override async Task OnInitializedAsync()
        {
            _client = _channel.CreateGrpcService<IConferencesService>();
            var test = await _client.ListConferencesAsync();
            _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/conferencesHub"))
            .Build();

            _hubConnection.On("NewConferenceAdded", async (Guid id) =>
            {
                _newId = id;
                Console.WriteLine($"###SignalR - NEW conference added! ID: {id}");
                _conferences = (await _client.ListConferencesAsync()).ToList();

                StateHasChanged();
            });


            await _hubConnection.StartAsync();

            _conferences = (await _client.ListConferencesAsync()).ToList();
            await base.OnInitializedAsync();
        }

        private void AddConference()
        {
            _navigationManager.NavigateTo($"conferences/new");
        }

        private void OpenDetails(string mode, Guid id)
        {
            _navigationManager.NavigateTo($"conferences/{mode}/{id}");
        }
    }
}