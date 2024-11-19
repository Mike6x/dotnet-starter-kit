using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public static class SearchQuizResultsEndpoint
{
    internal static RouteHandlerBuilder MapSearchQuizResultsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", async (ISender mediator, [FromBody] SearchQuizResultsRequest command) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);
        })
        .WithName(nameof(SearchQuizResultsEndpoint))
        .WithSummary("Gets a list of QuizResult items with paging support")
        .WithDescription("Gets a list of QuizResult items with paging support")
        .Produces<PagedList<QuizResults.QuizResultDto>>()
        .RequirePermission("Permissions.QuizResults.Search")
        .MapToApiVersion(1);
    }
}
