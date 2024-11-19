using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Shared.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Auth;


public partial class ResetPassword
{

    [Inject]
    public IApiClient PersonalClient { get; set; } = default!;

    private readonly ResetPasswordCommand _passwordModel = new();
    private string Tenant { get; set; } = TenantConstants.Root.Id;

    private string ConfirmPassword { get; set; } = default!;

    private FshValidation? _customValidation;
    private bool BusySubmitting { get; set; }

    protected override void OnInitialized()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("email", out var email))
        {
            _passwordModel.Email = email[0] ?? string.Empty;

        }
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Token", out var token))
        {
            _passwordModel.Token = token[0] ?? string.Empty;
        }
        else
        {
            Navigation.NavigateTo("/login");
        }

    }

    private async Task ResetPasswordAsync()
    {
        BusySubmitting = true;

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => PersonalClient.ResetPasswordEndpointAsync(Tenant,_passwordModel),
            Toast,
            _customValidation,
            "Password Reset!"))
        {
            _passwordModel.Email = string.Empty;
            _passwordModel.Token = string.Empty;
            _passwordModel.Password = string.Empty;
            
          //  Toast.Add("Please Login to Continue.", Severity.Success)

        }

        BusySubmitting = false;
    }

    private bool _newPasswordVisibility;
    private InputType _newPasswordInput = InputType.Password;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility()
    {
        if (_newPasswordVisibility)
        {
            _newPasswordVisibility = false;
            _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            _newPasswordInput = InputType.Password;
        }
        else
        {
            _newPasswordVisibility = true;
            _newPasswordInputIcon = Icons.Material.Filled.Visibility;
            _newPasswordInput = InputType.Text;
        }
    }
}
