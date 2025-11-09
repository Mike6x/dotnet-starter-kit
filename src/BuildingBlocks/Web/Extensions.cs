using FSH.Framework.Caching;
using FSH.Framework.Jobs;
using FSH.Framework.Mailing;
using FSH.Framework.Persistence;
using FSH.Framework.Web.Cors;
using FSH.Framework.Web.Exceptions;
using FSH.Framework.Web.Mediator.Behaviors;
using FSH.Framework.Web.Modules;
using FSH.Framework.Web.Observability.Logging.Serilog;
using FSH.Framework.Web.OpenApi;
using FSH.Framework.Web.Origin;
using FSH.Framework.Web.Versioning;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace FSH.Framework.Web;

public static class Extensions
{
    public static IHostApplicationBuilder UseFullStackHero(this IHostApplicationBuilder builder, params Assembly[] moduleAssemblies)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.AddHeroLogging();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDatabaseOptions(builder.Configuration);
        builder.Services.EnableCors(builder.Configuration);
        builder.Services.AddHeroVersioning();
        builder.Services.EnableApiDocs(builder.Configuration);
        builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
        builder.Services.AddFshJobs();
        builder.Services.AddHeroMailing();
        builder.Services.AddHeroCaching(builder.Configuration);
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.AddProblemDetails();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));
        builder.AddModules(moduleAssemblies);
        return builder;
    }

    public static WebApplication ConfigureFullStackHero(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.ExposeCors();
        app.UseRouting();
        app.ExposeApiDocs();
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
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapModules();
        return app;
    }
}