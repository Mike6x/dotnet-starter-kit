namespace FSH.Modules.Identity.Contracts.v1.Users.ResetPassword;

public class ResetPasswordCommand
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string Token { get; set; } = default!;
}