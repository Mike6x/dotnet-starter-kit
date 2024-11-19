using FSH.Framework.Infrastructure.Identity.Audit.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;
internal static class Extensions
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAssignRolesToUserEndpoint();
        app.MapChangePasswordEndpoint();
        app.MapDeleteUserEndpoint();
        app.MapDisableUserEndpoint();
        app.MapExportUsersEndpoint();

        app.MapForgotPasswordEndpoint();
        app.MapGetCornfirmEmailEndpoint();
        app.MapGetCornfirmPhoneNumberEndpoint();
        app.MapCornfirmEmailEndpoint();

        app.MapGetUserByEmailEndpoint();
        app.MapGetUserByNameEndpoint();
        app.MapGetUserByPhoneNumberEndpoint();
        app.MapGetUserEndpoint();

        app.MapGetCurrentUserPermissionsEndpoint();   
        app.MapGetMeEndpoint(); 
        app.MapGetUserRolesEndpoint();
        app.MapGetUsersListEndpoint();

        app.MapRegisterUserEndpoint();
        app.MapResetPasswordEndpoint();
        app.MapSearchUsersEndpoint();

        app.MapSelfRegisterUserEndpoint();
        app.MapSendVerificationEmailEndPoint();
        app.ToggleUserStatusEndpointEndpoint();
        app.MapUpdateUserEndpoint();
        app.MapUpdateCurrentUserEndpoint();
   
        app.MapGetUserAuditTrailEndpoint();

        app.MapLogoutCurrentUserEndpoint();
        app.MapImportUsersEndpoint();

        app.MapGetOtherUsersEndpoint();
        return app;
    }
}
