using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Shared.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Auth
{
    public partial class EmailConfirm
    {
        [Inject]
        public IApiClient PersonalClient { get; set; } = default!;

        private readonly EmailConfirmCommand _command = new();
        private string Tenant { get; set; } = TenantConstants.Root.Id;

        private FshValidation? _customValidation;
        private bool BusySubmitting { get; set; }


        protected override async void OnInitialized()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("userId", out var userId))
            {
                _command.UserId = userId[0] ?? string.Empty;

            }
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Code", out var code))
            {
                _command.Code = code[0] ?? string.Empty;
            }
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Tenant", out var tenant))
            {
                _command.Tenant = tenant[0] ?? string.Empty;

            }

            await ResetPasswordAsync();

            Navigation.NavigateTo("/login");

        }

        private async Task ResetPasswordAsync()
        {
            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => PersonalClient.ConirmEmailEndpointAsync(Tenant,_command),
                Toast,
                _customValidation,
                "Email Confirm!"))
            {
                _command.UserId = string.Empty;
                _command.Code = string.Empty;
                _command.Tenant = string.Empty;

            }

            BusySubmitting = false;
        }
    }
}