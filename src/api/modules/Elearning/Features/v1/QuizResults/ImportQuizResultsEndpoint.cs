using FSH.Framework.Core.DataIO;
using FSH.Framework.Core.Storage.File.Features;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public static class ImportQuizResultsEndpoint
{
    internal static RouteHandlerBuilder MapImportQuizResultsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/Import", async (FileUploadCommand uploadFile, bool isUpdate, ISender mediator) =>
            {
                var response = await mediator.Send(new QuizResults.ImportQuizResultsCommand(uploadFile, isUpdate));
                return Results.Ok(response);
             
            })
            .WithName(nameof(ImportQuizResultsEndpoint))
            .WithSummary("Imports a list of QuizResults")
            .WithDescription("Imports a list of entities from excel files")
            .Produces<ImportResponse>()
            .RequirePermission("Permissions.QuizResults.Import")
            .MapToApiVersion(1);
    }
}

