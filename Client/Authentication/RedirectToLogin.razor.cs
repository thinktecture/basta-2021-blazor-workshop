using Microsoft.AspNetCore.Components;

namespace ConfTool.Client.Authentication
{
    public partial class RedirectToLogin
    {
        [Inject] private NavigationManager _navigationManager { get; set; }
        protected override void OnInitialized()
        {
            _navigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}");
        }
    }
}