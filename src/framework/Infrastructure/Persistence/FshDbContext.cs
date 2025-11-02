using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Multitenancy;
using FSH.Framework.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Persistence;
public class FshDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor,
    DbContextOptions options,
    IOptions<DatabaseOptions> settings)
    : MultiTenantDbContext(multiTenantContextAccessor, options)
{
    private readonly DatabaseOptions _settings = settings.Value;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.AppendGlobalQueryFilter<ISoftDeletable>(s => !s.IsDeleted);
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        optionsBuilder.EnableSensitiveDataLogging();

        if (!string.IsNullOrWhiteSpace(multiTenantContextAccessor?.MultiTenantContext.TenantInfo?.ConnectionString))
        {
            optionsBuilder.ConfigureDatabase(_settings.Provider, multiTenantContextAccessor.MultiTenantContext.TenantInfo.ConnectionString!, _settings.MigrationsAssembly);
        }
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TenantNotSetMode = TenantNotSetMode.Overwrite;
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }
}