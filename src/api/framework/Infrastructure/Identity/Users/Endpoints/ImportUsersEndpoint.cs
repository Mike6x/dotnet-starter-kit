using FSH.Framework.Core.DataIO;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Storage.File.Features;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints
{
    public static class ImportUsersEndpoint
    {
        internal static RouteHandlerBuilder MapImportUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/import", async(
                FileUploadCommand uploadFile, 
                bool isUpdate,
                IUserService service, 
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";

                var response =await service.ImportAsync(uploadFile, isUpdate, origin, cancellationToken);
                
                return Results.Ok(response);
            })
            .WithName(nameof(ImportUsersEndpoint))
            .WithDescription("Imports a list of entities from excel files")
            .Produces<ImportResponse>()
            .RequirePermission("Permissions.Users.Import")
            .WithDescription("Import a list of users ");
        }
    }
}