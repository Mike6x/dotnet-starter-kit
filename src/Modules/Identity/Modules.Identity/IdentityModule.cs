using Asp.Versioning;
using FSH.Framework.Core.Context;
using FSH.Framework.Identity.v1.Tokens.TokenGeneration;
using FSH.Framework.Infrastructure.Identity.Users.Endpoints;
using FSH.Framework.Infrastructure.Identity.Users.Services;
using FSH.Framework.Persistence;
using FSH.Framework.Storage.Local;
using FSH.Framework.Storage.Services;
using FSH.Framework.Web.Modules;
using FSH.Modules.Identity.Authorization;
using FSH.Modules.Identity.Authorization.Jwt;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Data;
using FSH.Modules.Identity.Features.v1.Roles;
using FSH.Modules.Identity.Features.v1.Roles.DeleteRole;
using FSH.Modules.Identity.Features.v1.Roles.GetRole;
using FSH.Modules.Identity.Features.v1.Roles.GetRoles;
using FSH.Modules.Identity.Features.v1.Roles.GetRoleWithPermissions;
using FSH.Modules.Identity.Features.v1.Roles.UpdateRolePermissions;
using FSH.Modules.Identity.Features.v1.Roles.UpsertRole;
using FSH.Modules.Identity.Features.v1.Users;
using FSH.Modules.Identity.Features.v1.Users.AssignUserRoles;
using FSH.Modules.Identity.Features.v1.Users.ChangePassword;
using FSH.Modules.Identity.Features.v1.Users.ConfirmEmail;
using FSH.Modules.Identity.Features.v1.Users.DeleteUser;
using FSH.Modules.Identity.Features.v1.Users.GetUser;
using FSH.Modules.Identity.Features.v1.Users.GetUserPermissions;
using FSH.Modules.Identity.Features.v1.Users.GetUserProfile;
using FSH.Modules.Identity.Features.v1.Users.GetUserRoles;
using FSH.Modules.Identity.Features.v1.Users.GetUsers;
using FSH.Modules.Identity.Features.v1.Users.RegisterUser;
using FSH.Modules.Identity.Features.v1.Users.ResetPassword;
using FSH.Modules.Identity.Features.v1.Users.ToggleUserStatus;
using FSH.Modules.Identity.Features.v1.Users.UpdateUser;
using FSH.Modules.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace FSH.Modules.Identity;

public class IdentityModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var services = builder.Services;
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, PathAwareAuthorizationHandler>();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IStorageService, LocalStorageService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddHeroDbContext<IdentityDbContext>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<IdentityDbContext>(
                name: "db:identity",
                failureStatus: HealthStatus.Unhealthy);
        services.AddScoped<IDbInitializer, IdentityDbInitializer>();
        services.AddIdentity<FshUser, FshRole>(options =>
        {
            options.Password.RequiredLength = IdentityModuleConstants.PasswordLength;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        })
           .AddEntityFrameworkStores<IdentityDbContext>()
           .AddDefaultTokenProviders();
        services.ConfigureJwtAuth();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiVersionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}/identity")
            .WithTags("Identity")
            .WithApiVersionSet(apiVersionSet);

        // tokens
        group.MapGenerateTokenEndpoint().AllowAnonymous().RequireRateLimiting("auth");

        // roles
        group.MapGetRolesEndpoint();
        group.MapGetRoleByIdEndpoint();
        group.MapDeleteRoleEndpoint();
        group.MapGetRolePermissionsEndpoint();
        group.MapUpdateRolePermissionsEndpoint();
        group.MapCreateOrUpdateRoleEndpoint();

        // users
        group.MapAssignUserRolesEndpoint();
        group.MapChangePasswordEndpoint();
        group.MapConfirmEmailEndpoint();
        group.MapDeleteUserEndpoint();
        group.MapGetUserEndpoint();
        group.MapGetCurrentUserPermissionsEndpoint();
        group.MapGetMeEndpoint();
        group.MapGetUserRolesEndpoint();
        group.MapGetUsersListEndpoint();
        group.MapRegisterUserEndpoint();
        group.MapResetPasswordEndpoint();
        group.MapSelfRegisterUserEndpoint();
        group.ToggleUserStatusEndpointEndpoint();
        group.MapUpdateUserEndpoint();
    }
}
