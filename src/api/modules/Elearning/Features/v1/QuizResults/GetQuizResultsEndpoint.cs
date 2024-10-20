using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public static class GetQuizResultsEndpoint
{
    internal static RouteHandlerBuilder MapGetQuizResultsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/getlist", async (ISender mediator, [FromBody] GetQuizResultsRequest command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(GetQuizResultsEndpoint))
            .WithSummary("Gets a list of QuizResults")
            .WithDescription("Gets a list of QuizResults with filtering support")
            .Produces<List<QuizResultDto>>()
            .RequirePermission("Permissions.QuizResults.Search")
            .MapToApiVersion(1);
    }
}

