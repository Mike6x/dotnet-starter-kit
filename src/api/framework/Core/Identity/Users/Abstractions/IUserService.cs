using System.Security.Claims;
using FSH.Framework.Core.Identity.Users.Dtos;
using FSH.Framework.Core.Identity.Users.Features.AssignUserRole;
using FSH.Framework.Core.Identity.Users.Features.ChangePassword;
using FSH.Framework.Core.Identity.Users.Features.ExportUsers;
using FSH.Framework.Core.Identity.Users.Features.ForgotPassword;
using FSH.Framework.Core.Identity.Users.Features.RegisterUser;
using FSH.Framework.Core.Identity.Users.Features.ResetPassword;
using FSH.Framework.Core.Identity.Users.Features.ToggleUserStatus;
using FSH.Framework.Core.Identity.Users.Features.UpdateUser;
using FSH.Framework.Core.Paging;

namespace FSH.Framework.Core.Identity.Users.Abstractions;
public interface IUserService
{
    Task<bool> ExistsWithNameAsync(string name);
    
    Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null);
    
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null);
    
    Task<List<UserDetail>> GetListAsync(CancellationToken cancellationToken);
    
    Task<int> GetCountAsync(CancellationToken cancellationToken);
    
    Task<UserDetail> GetAsync(string userId, CancellationToken cancellationToken);
    
    Task ToggleStatusAsync(ToggleUserStatusCommand request, CancellationToken cancellationToken);
    
    Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal);
    
    Task<RegisterUserResponse> RegisterAsync(RegisterUserCommand request, string origin, CancellationToken cancellationToken);
    
    Task UpdateAsync(UpdateUserCommand request, string userId, string origin);
    Task DisableAsync(string userId);
    
    Task<string> ConfirmEmailAsync(string userId, string code, string tenant, CancellationToken cancellationToken);
    Task<string> ConfirmPhoneNumberAsync(string userId, string code);

    // passwords
    Task ForgotPasswordAsync(ForgotPasswordCommand request, string origin, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordCommand request, CancellationToken cancellationToken);
    Task ChangePasswordAsync(ChangePasswordCommand request, string userId);
    
    // permissions
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task<List<string>?> GetPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<string> AssignRolesAsync(string userId, AssignUserRoleCommand request, CancellationToken cancellationToken);
    Task<List<UserRoleDetail>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);
    
    
    #region My Customize
    Task<PagedList<UserDetail>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken);
    Task<UserDetail> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<UserDetail> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserDetail> GetByPhoneAsync(string phone, CancellationToken cancellationToken);
    
    Task<string> DeleteAsync(string userId);
    Task<byte[]> ExportAsync(UserListFilter filter, CancellationToken cancellationToken);

    Task SendVerificationEmailAsync(string userId, string origin, CancellationToken cancellationToken);
    #endregion
}
