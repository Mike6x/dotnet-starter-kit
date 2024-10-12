using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public static class GetQuizEndpoint
{
    internal static RouteHandlerBuilder MapGetQuizEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
                        {
                            var response = await mediator.Send(new GetQuizRequest(id));
                            return Results.Ok(response);
                        })
                        .WithName(nameof(GetQuizEndpoint))
                        .WithSummary("gets Quiz item by id")
                        .WithDescription("gets Quiz item by id")
                        .Produces<GetQuizResponse>()
                        .RequirePermission("Permissions.Quizs.View")
                        .MapToApiVersion(1);
    }
}
