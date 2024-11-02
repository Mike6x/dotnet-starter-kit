using FSH.Framework.Core.Exceptions;
using Shared.Authorization;

namespace FSH.Framework.Infrastructure.Identity.Users.Services;

internal partial class UserService
{

#region My Customize
    public async Task DeleteAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId) 
            ?? throw new NotFoundException($"User with Id: {userId} Not Found.");

        if (user.Email == TenantConstants.Root.EmailAddress)
        {
            throw new ConflictException($"Admin user: {user.Email} can not be deleted !");
        }

        if (user.ImageUrl != null)
        {
            storageService.Remove(user.ImageUrl);
            user.ImageUrl = null;
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var result = await userManager.RemoveFromRolesAsync(user, userRoles);

        if (!result.Succeeded)
        {
            throw new FshException("Remove role(s) failed.");
        }

        await signInManager.RefreshSignInAsync(user);
        result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            List<string> errors = result.Errors.Select(error => error.Description).ToList();
            throw new FshException("Delete profile failed", errors);
        }
    }
    #endregion

}
