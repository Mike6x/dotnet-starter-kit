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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace FSH.Framework.Web;

public static class Extensions
{
    public static IHostApplicationBuilder AddFshPlatform(this IHostApplicationBuilder builder, Action<FshPlatformOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new FshPlatformOptions();
        configure?.Invoke(options);

        builder.AddHeroLogging();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDatabaseOptions(builder.Configuration);

        if (options.EnableCors)
        {
            builder.Services.EnableCors(builder.Configuration);
        }

        builder.Services.AddHeroVersioning();

        if (options.EnableOpenApi)
        {
            builder.Services.EnableApiDocs(builder.Configuration);
        }

        builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

        if (options.EnableJobs)
        {
            builder.Services.AddFshJobs();
        }

        if (options.EnableMailing)
        {
            builder.Services.AddHeroMailing();
        }

        if (options.EnableCaching)
        {
            builder.Services.AddHeroCaching(builder.Configuration);
        }

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.AddProblemDetails();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));

        return builder;
    }


    public static WebApplication UseFshPlatform(this WebApplication app, Action<FshPipelineOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = new FshPipelineOptions();
        configure?.Invoke(options);

        app.UseExceptionHandler();
        app.UseHttpsRedirection();

        if (options.UseCors)
        {
            app.ExposeCors();
        }

        app.UseRouting();

        if (options.UseOpenApi)
        {
            app.ExposeApiDocs();
        }

        if (options.ServeStaticFiles)
        {
            var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }

            // Single static files registration is sufficient; default serves from wwwroot
            app.UseStaticFiles();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        if (options.MapModules)
        {
            app.MapModules();
        }

        return app;
    }
}

public sealed class FshPlatformOptions
{
    public bool EnableCors { get; set; } = true;
    public bool EnableOpenApi { get; set; } = true;
    public bool EnableCaching { get; set; } = false;
    public bool EnableJobs { get; set; } = false;
    public bool EnableMailing { get; set; } = false;
}

public sealed class FshPipelineOptions
{
    public bool UseCors { get; set; } = true;
    public bool UseOpenApi { get; set; } = true;
    public bool ServeStaticFiles { get; set; } = true;
    public bool MapModules { get; set; } = true;
}
