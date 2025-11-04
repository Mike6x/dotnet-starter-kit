using FSH.Framework.Core;
using FSH.Framework.Core.Origin;
using FSH.Framework.Infrastructure.Identity;
using FSH.Framework.Infrastructure.Multitenancy;
using FSH.Framework.Infrastructure.Persistence.Extensions;
using FSH.Framework.Web;
using FSH.Framework.Web.Cors;
using FSH.Framework.Web.Identity;
using FSH.Framework.Web.Mediator;
using FSH.Framework.Web.MultiTenancy;
using FSH.Framework.Web.OpenApi;
using FSH.Web.Endpoints.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;

namespace FSH.Framework.Infrastructure;
public static class Extensions
{
    public static WebApplicationBuilder UseFullStackHero(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddHttpContextAccessor();
        builder.AddDatabaseOption();
        builder.Services.EnableCors(builder.Configuration);
        builder.Services.EnableApiDocs(builder.Configuration); builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
        builder.Services.AddProblemDetails();
        builder.Services.AddHealthChecks();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));

        // Define framework assemblies
        var assemblies = new Assembly[]
        {
            typeof(IFshCore).Assembly,
            typeof(IFshInfrastructure).Assembly,
            typeof(IFshWeb).Assembly
        };

        builder.Services.EnableMediator(assemblies);
        builder.Services.RegisterMultitenancy(builder.Configuration);
        builder.Services.RegisterIdentity();
        return builder;
    }

    public static WebApplication ConfigureFullStackHero(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.ExposeCors();
        app.ExposeCors();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.ConfigureMultitenancy();
        app.MapIdentityEndpoints();
        app.MapMultitenancyEndpoints();
        app.MapHealthCheckEndpoints();
        return app;
    }
}
