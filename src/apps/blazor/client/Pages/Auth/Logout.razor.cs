using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Auth
{
    public partial class Logout()
    {

        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;

        [Inject]
        protected IApiClient ApiClient { get; set; } = default!;
         protected override async Task OnInitializedAsync()
        {
            await AuthService.LogoutAsync();
            Toast.Add("Logged out", Severity.Error);
        }


    }
}
