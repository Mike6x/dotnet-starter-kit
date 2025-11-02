using FSH.Framework.Core.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Persistence.Extensions;
public static class ServiceExtensions
{
    public static WebApplicationBuilder AddDatabaseOption(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // create a temporary provider just for logging
        using var provider = builder.Services.BuildServiceProvider();
        var logger = provider.GetService<ILoggerFactory>()?.CreateLogger("ServiceExtensions")!;

        builder.Services.AddOptions<DatabaseOptions>()
            .BindConfiguration(nameof(DatabaseOptions))
            .ValidateDataAnnotations()
            .PostConfigure(config =>
            {
                logger.LogInformation("current db provider: {DatabaseProvider}", config.Provider);
                logger.LogInformation("for documentations and guides, visit https://www.fullstackhero.net");
                logger.LogInformation("to sponsor this project, visit https://opencollective.com/fullstackhero");
            });
        return builder;
    }

    public static IServiceCollection BindDbContext<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDbContext<TContext>((sp, options) =>
        {
            var dbConfig = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.ConfigureDatabase(dbConfig.Provider, dbConfig.ConnectionString, dbConfig.MigrationsAssembly);
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        return services;
    }
}
