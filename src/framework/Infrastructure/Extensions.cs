using FSH.Framework.Core;
using FSH.Framework.Core.Origin;
using FSH.Framework.Infrastructure.Caching;
using FSH.Framework.Infrastructure.Exceptions;
using FSH.Framework.Infrastructure.Identity;
using FSH.Framework.Infrastructure.Jobs;
using FSH.Framework.Infrastructure.Mailing;
using FSH.Framework.Infrastructure.Multitenancy;
using FSH.Framework.Infrastructure.Persistence.Extensions;
using FSH.Framework.Infrastructure.Storage;
using FSH.Framework.Web;
using FSH.Framework.Web.Cors;
using FSH.Framework.Web.Identity;
using FSH.Framework.Web.Mediator;
using FSH.Framework.Web.MultiTenancy;
using FSH.Framework.Web.Observability.Logging.Serilog;
using FSH.Framework.Web.OpenApi;
using FSH.Web.Endpoints.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace FSH.Framework.Infrastructure;
public static class Extensions
{
    public static WebApplicationBuilder UseFullStackHero(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddHttpContextAccessor();
        builder.AddHeroLogging();
        builder.AddDatabaseOption();
        builder.Services.EnableCors(builder.Configuration);
        builder.Services.AddLocalFileStorage();
        builder.Services.EnableApiDocs(builder.Configuration);
        builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
        builder.Services.AddFshJobs();
        builder.Services.AddHeroMailing();
        builder.Services.AddHeroCaching(builder.Configuration);
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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
        app.UseExceptionHandler();
        app.ExposeCors();
        app.ExposeApiDocs();
        app.UseJobDashboard(app.Configuration);
        app.UseRouting();
        app.UseStaticFiles();
        var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        if (!Directory.Exists(assetsPath))
        {
            Directory.CreateDirectory(assetsPath);
        }
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            RequestPath = new PathString("/wwwroot"),
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.ConfigureMultitenancy();
        app.MapIdentityEndpoints();
        app.MapMultitenancyEndpoints();
        app.MapHealthCheckEndpoints();
        return app;
    }
}
