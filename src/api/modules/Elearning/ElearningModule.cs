using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
using FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
using FSH.Starter.WebApi.Elearning.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning;

public static class ElearningModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("elearning") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var quizGroup = app.MapGroup("quizs").WithTags("quizs");
            quizGroup.MapCreateQuizEndpoint();
            quizGroup.MapGetQuizEndpoint();
            quizGroup.MapGetQuizsEndpoint();
            quizGroup.MapSearchQuizsEndpoint();
            quizGroup.MapUpdateQuizEndpoint();
            quizGroup.MapDeleteQuizEndpoint();
            quizGroup.MapExportQuizsEndpoint();
            quizGroup.MapImportQuizsEndpoint();
            
            var quizResultGroup = app.MapGroup("quizresults").WithTags("quizresults");
            // quizResultGroup.MapCreateQuizResultEndpoint();
            // quizResultGroup.MapGetQuizResultEndpoint();
            // quizResultGroup.MapGetQuizResultsEndpoint();
            // quizResultGroup.MapSearchQuizResultEndpoint();
            // quizResultGroup.MapUpdateQuizResultEndpoint();
            quizResultGroup.MapDeleteQuizResultEndpoint();
            // quizResultGroup.MapExportQuizResultsEndpoint();
            // quizResultGroup.MapImportQuizResultsEndpoint();
            //
        }
    }
    
    public static WebApplicationBuilder RegisterElearningServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<ElearningDbContext>();
        builder.Services.AddScoped<IDbInitializer, ElearningDbInitializer>();
        
        builder.Services.AddKeyedScoped<IRepository<Quiz>, ElearningRepository<Quiz>>("elearning:quizs");
        builder.Services.AddKeyedScoped<IReadRepository<Quiz>, ElearningRepository<Quiz>>("elearning:quizs");
        
        builder.Services.AddKeyedScoped<IRepository<QuizResult>, ElearningRepository<QuizResult>>("elearning:quizresults");
        builder.Services.AddKeyedScoped<IReadRepository<QuizResult>, ElearningRepository<QuizResult>>("elearning:quizresults");
        
        return builder;
    }
    
    public static WebApplication UseElearningModule(this WebApplication app)
    {
        return app;
    }
}
