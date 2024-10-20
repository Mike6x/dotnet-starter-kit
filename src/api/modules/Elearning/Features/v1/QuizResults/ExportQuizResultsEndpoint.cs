using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public static class ExportQuizResultsEndpoint
{
    internal static RouteHandlerBuilder MapExportQuizResultsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/export", async Task<byte[]> (ISender mediator, [FromBody] ExportQuizResultsRequest command) =>
            {
                var response = await mediator.Send(command);

                return response;
            })
            .WithName(nameof(ExportQuizResultsEndpoint))
            .WithSummary("Exports a list of QuizResults")
            .WithDescription("Exports a list of QuizResults with filtering support")
            .Produces <byte[]>()
            .RequirePermission("Permissions.QuizResults.Export")
            .MapToApiVersion(1);
    }
}

