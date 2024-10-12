using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;

public static class ExportQuizsEndpoint
{
    internal static RouteHandlerBuilder MapExportQuizsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/export", async Task<byte[]> (ISender mediator, [FromBody] ExportQuizsRequest command) =>
            {
                var response = await mediator.Send(command);

                return response;
            })
            .WithName(nameof(ExportQuizsEndpoint))
            .WithSummary("Exports a list of Quizs")
            .WithDescription("Exports a list of Quizs with filtering support")
            .Produces <byte[]>()
            .RequirePermission("Permissions.Quizs.Export")
            .MapToApiVersion(1);
    }
}

