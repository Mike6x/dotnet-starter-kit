using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Mail;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization;

namespace FSH.Framework.Infrastructure.Identity.Users.Services;

internal partial class UserService
{
    public async Task SendVerificationEmailAsync(string userId, string origin, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException("User Not Found.");

        bool isAdmin = await userManager.IsInRoleAsync(user, FshRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException("Administrators do not have been verified");
        }

        user.IsActive = true;
        user.EmailConfirmed = false;

        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            var messages = new List<string> { $"User {user.UserName} : " };
            await GenerateVerificationEmail(user, messages, origin);
        }
    }

    // Generate verification email
    private async Task GenerateVerificationEmail(FshUser user, List<string> messages, string origin)
    {
        string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
        var mailRequest = new MailRequest(
                [user.Email ?? string.Empty],
                "Confirm Registration",
                emailVerificationUri);

        jobService.Enqueue("email", () => mailService.SendAsync(mailRequest, CancellationToken.None));

        messages.Add($"Please check {user.Email} to verify your account!");
    }
}