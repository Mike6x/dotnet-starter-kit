using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Web.MultiTenancy;
public static class Endpoints
{
    public static IEndpointRouteBuilder MapMultitenancyEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1))
                            .ReportApiVersions()
                            .Build();

        var group = app.MapGroup("api/v{version:apiVersion}/tenants")
                       .WithTags("Tenants")
                       .WithOpenApi()
                       .WithApiVersionSet(versionSet);

        return group;
    }
}
