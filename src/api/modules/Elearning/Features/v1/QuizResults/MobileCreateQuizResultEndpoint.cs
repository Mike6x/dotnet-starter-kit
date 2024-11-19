using Asp.Versioning;
using Finbuckle.MultiTenant;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Tenant;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

/// <summary>
/// Anonymous user creates a new QuizResult.
/// [TenantIdHeader]
/// [Consumes("application/x-www-form-urlencoded")]
/// [OpenApiOperation("Anonymous user creates a new QuizResult.", "")]
/// [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Create))]
/// </summary>
public static class MobileCreateQuizResultEndpoint
{
    internal static RouteHandlerBuilder MapMobileCreateQuizResultEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/mobile-create", async (
            [FromForm] MobileCreateQuizResultCommand request,
            string tenant,
            HttpContext context,
            ITenantService tenantService,
            ISender mediator) =>
                {
                    var tenantDetail = await tenantService.GetByIdAsync(tenant) ?? throw new NotFoundException($"Tenant: {tenant} not found");
                    var tenantInfo = tenantDetail.Adapt<FshTenantInfo>();   
                    context.SetTenantInfo(tenantInfo, true);

                    var response = await mediator.Send(request);
                    //return Results.CreatedAtRoute("MobileCreateQuizResultEndpoint", new { id = response.Id }, response)

                    return Results.Ok(response);
                })
                // .AddEndpointFilter<TenantIdFilter>()
                .WithName(nameof(MobileCreateQuizResultEndpoint))
                .WithSummary("Creates a QuizResult item")
                .WithDescription("Creates a QuizResult item")
                .Produces(StatusCodes.Status200OK)
                .RequirePermission("Permissions.QuizResults.Create")
                .AllowAnonymous()
                .DisableAntiforgery()
                .MapToApiVersion(new ApiVersion(1, 0));

    }
}

public class TenantIdFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;
        if (headers.ContainsKey("tenant"))
        {
            context.HttpContext.Request.Headers["tenant"] = (string?)context.Arguments[1];
        }
        else 
        {
            context.HttpContext.Request.Headers.Append("tenant", (string?)context.Arguments[1]);
        }
        
        return await next(context);
    }
}