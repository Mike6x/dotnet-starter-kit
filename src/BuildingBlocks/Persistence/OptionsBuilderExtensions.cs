using FSH.Framework.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FSH.Framework.Persistence;

public static class OptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ConfigureHeroDatabase(
        this DbContextOptionsBuilder builder,
        string dbProvider,
        string connectionString,
        string migrationsAssembly)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(dbProvider);

        builder.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
        return dbProvider.ToUpperInvariant() switch
        {
            DbProviders.PostgreSQL =>
                builder.UseNpgsql(
                        connectionString,
                        e =>
                        {
                            e.MigrationsAssembly(migrationsAssembly);
                        })
                    .EnableSensitiveDataLogging(),
            DbProviders.MSSQL =>
                builder.UseSqlServer(
                        connectionString,
                        e =>
                        {
                            e.MigrationsAssembly(migrationsAssembly);
                            e.EnableRetryOnFailure();
                        })
                    .EnableSensitiveDataLogging(),
            _ => throw new InvalidOperationException($"Database Provider {dbProvider} is not supported."),
        };
    }

}
