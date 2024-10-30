using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;

public static class GetQuizsEndpoint
{
    internal static RouteHandlerBuilder MapGetQuizsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/getlist", async (ISender mediator, [FromBody] GetQuizsRequest command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(GetQuizsEndpoint))
            .WithSummary("Gets a list of Quizs")
            .WithDescription("Gets a list of Quizs with filtering support")
            .Produces<List<QuizDto>>()
            .RequirePermission("Permissions.Quizs.Search")
            .MapToApiVersion(1);
    }
}

