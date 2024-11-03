using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;

public static class SearchQuizsEndpoint
{
    internal static RouteHandlerBuilder MapSearchQuizsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", async (ISender mediator, [FromBody] SearchQuizsRequest command) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);
        })
        .WithName(nameof(SearchQuizsEndpoint))
        .WithSummary("Gets a list of Quiz items with paging support")
        .WithDescription("Gets a list of Quiz items with paging support")
        .Produces<PagedList<QuizDto>>()
        .RequirePermission("Permissions.Quizs.Search")
        .MapToApiVersion(1);
    }
}
