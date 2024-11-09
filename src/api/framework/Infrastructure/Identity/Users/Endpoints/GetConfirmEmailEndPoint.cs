using Azure.Core;
using Finbuckle.MultiTenant;
using FluentValidation;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Core.Tenant.Dtos;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Tenant;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Authorization;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints
{
    public static class GetConirmEmailEndpoint
    {
        internal static RouteHandlerBuilder MapGetCornfirmEmailEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("/confirm-email", async (
                [FromQuery] string userId,
                [FromQuery] string code,
                [FromQuery] string tenant,
                HttpContext context,
                ITenantService tenantService,
                IUserService userService,
                CancellationToken cancellationToken) =>
            {
                TenantDetail tenantDetail = await tenantService.GetByIdAsync(tenant) ?? throw new NotFoundException($"Tenant: {tenant} not found");
                var tenantInfo = tenantDetail.Adapt<FshTenantInfo>();  

                context.SetTenantInfo(tenantInfo, true);
                // context.Request.Headers.Add("tenant", tenant);

                return Task.FromResult(userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken));
            })
            .WithName(nameof(GetConirmEmailEndpoint))
            .WithSummary("Confirm email")
            .WithDescription("Confirm email address for a user.")
            .RequirePermission("Permissions.Users.Search")
            .AllowAnonymous();
        }
        
    }
}