using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Identity.Permissions;
using FSH.Framework.Identity.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FSH.Framework.Infrastructure.Auth;

public static class RequiredPermissionAuthorizationExtensions
{
    public static AuthorizationPolicyBuilder RequireRequiredPermissions(this AuthorizationPolicyBuilder builder)
    {
        return builder.AddRequirements(new PermissionAuthorizationRequirement());
    }

    public static AuthorizationBuilder AddRequiredPermissionPolicy(this AuthorizationBuilder builder)
    {
        builder.AddPolicy(PermissionConstants.RequiredPermissionPolicyName, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AddAuthenticationSchemes(AuthenticationConstants.AuthenticationScheme);
            policy.RequireRequiredPermissions();
        });

        builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IAuthorizationHandler, RequiredPermissionAuthorizationHandler>());

        return builder;
    }
}