using FSH.Framework.Web.Identity.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Web.Identity;
public static class Endpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app, string routePrefix = "/api/identity")
    {
        var group = app.MapGroup(routePrefix)
                       .WithTags("Identity")
                       .WithOpenApi();

        group.MapGenerateTokenEndpoint();

        return app;
    }
}
