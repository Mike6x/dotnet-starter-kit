using FluentValidation;
using FSH.Framework.Core.Context;
using FSH.Modules.Identity.Contracts.v1.Users.ChangePassword;
using FSH.Modules.Identity.Features.v1.Users;
using FSH.Modules.Identity.Services;
using Microsoft.AspNetCore.Identity;

namespace FSH.Modules.Identity.Features.v1.Users.ChangePassword;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    private readonly UserManager<FshUser> _userManager;
    private readonly IPasswordHistoryService _passwordHistoryService;
    private readonly ICurrentUser _currentUser;

    public ChangePasswordValidator(
        UserManager<FshUser> userManager,
        IPasswordHistoryService passwordHistoryService,
        ICurrentUser currentUser)
    {
        _userManager = userManager;
        _passwordHistoryService = passwordHistoryService;
        _currentUser = currentUser;

        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(p => p.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .NotEqual(p => p.Password)
            .WithMessage("New password must be different from the current password.")
            .MustAsync(NotBeInPasswordHistoryAsync)
            .WithMessage("This password has been used recently. Please choose a different password.");

        RuleFor(p => p.ConfirmNewPassword)
            .Equal(p => p.NewPassword)
            .WithMessage("Passwords do not match.");
    }

    private async Task<bool> NotBeInPasswordHistoryAsync(string newPassword, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated())
        {
            return true; // Let other validation handle unauthorized access
        }

        var userId = _currentUser.GetUserId().ToString();
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return true; // Let other validation handle user not found
        }

        // Check if password is in history
        var isInHistory = await _passwordHistoryService.IsPasswordInHistoryAsync(user, newPassword, cancellationToken);
        return !isInHistory; // Return true if NOT in history (validation passes)
    }
}