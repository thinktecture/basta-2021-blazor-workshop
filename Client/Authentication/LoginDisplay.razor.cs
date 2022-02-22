using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ConfTool.Client.Authentication
{
    public partial class LoginDisplay
    {
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private SignOutSessionStateManager _signOutSessionStateManager { get; set; }
        private async Task BeginSignOut(MouseEventArgs args)
        {
            await _signOutSessionStateManager.SetSignOutState();
            _navigationManager.NavigateTo("authentication/logout");
        }
    }
}