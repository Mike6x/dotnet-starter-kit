using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Web.Cors;

public static class Extensions
{
    private const string PolicyName = "FSHCorsPolicy";

    public static IServiceCollection EnableCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var corsSettings = new CorsOptions();
        configuration.GetSection(nameof(CorsOptions)).Bind(corsSettings);

        services.AddSingleton(Options.Create(corsSettings));

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, builder =>
            {
                if (corsSettings.AllowAll)
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    builder
                        .WithOrigins(corsSettings.AllowedOrigins)
                        .WithHeaders(corsSettings.AllowedHeaders)
                        .WithMethods(corsSettings.AllowedMethods)
                        .AllowCredentials();
                }
            });
        });

        return services;
    }

    public static void ExposeCors(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        app.UseCors(PolicyName);
    }
}