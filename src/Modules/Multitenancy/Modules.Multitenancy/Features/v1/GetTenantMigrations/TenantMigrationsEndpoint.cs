using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Contracts.Dtos;
using FSH.Modules.Multitenancy.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Modules.Multitenancy.Features.v1.GetTenantMigrations;

public static class TenantMigrationsEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet(
                "/migrations",
                async (IServiceProvider serviceProvider, CancellationToken cancellationToken) =>
                {
                    using IServiceScope scope = serviceProvider.CreateScope();
                    var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
                    var tenants = await tenantStore.GetAllAsync().ConfigureAwait(false);

                    var tenantMigrationStatuses = new List<TenantMigrationStatusDto>();

                    foreach (var tenant in tenants)
                    {
                        var tenantStatus = new TenantMigrationStatusDto
                        {
                            TenantId = tenant.Id,
                            Name = tenant.Name,
                            IsActive = tenant.IsActive,
                            ValidUpto = tenant.ValidUpto
                        };

                        try
                        {
                            using IServiceScope tenantScope = scope.ServiceProvider.CreateScope();

                            tenantScope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>()
                                .MultiTenantContext = new MultiTenantContext<AppTenantInfo>
                                {
                                    TenantInfo = tenant
                                };

                            var dbContext = tenantScope.ServiceProvider.GetRequiredService<TenantDbContext>();

                            var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)
                                .ConfigureAwait(false);
                            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)
                                .ConfigureAwait(false);

                            tenantStatus.Provider = dbContext.Database.ProviderName;
                            tenantStatus.LastAppliedMigration = appliedMigrations.LastOrDefault();
                            tenantStatus.PendingMigrations = pendingMigrations.ToArray();
                            tenantStatus.HasPendingMigrations = tenantStatus.PendingMigrations.Count > 0;
                        }
                        catch (Exception ex)
                        {
                            tenantStatus.Error = ex.Message;
                        }

                        tenantMigrationStatuses.Add(tenantStatus);
                    }

                    return Results.Ok(tenantMigrationStatuses);
                })
            .WithName("GetTenantMigrations")
            .RequirePermission(MultitenancyConstants.Permissions.View)
            .WithSummary("Get per-tenant migration status")
            .WithDescription("Retrieve migration status for each tenant, including pending migrations and provider information.")
            .Produces<IReadOnlyCollection<TenantMigrationStatusDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}