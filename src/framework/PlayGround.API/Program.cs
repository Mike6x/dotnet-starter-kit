using FSH.Framework.Web.Cors;
using FSH.Framework.Web.Identity;
using FSH.Framework.Web.OpenApi;
using FSH.Web.Endpoints.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.EnableApiDocs(builder.Configuration);
builder.Services.EnableCors(builder.Configuration);

builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

var app = builder.Build();
app.ExposeApiDocs();
app.ExposeCors();

app.UseHttpsRedirection();

app.MapGet("/", () => "hello world!").WithTags("PlayGround");

app.MapIdentityEndpoints();
app.MapHealthCheckEndpoints();
await app.RunAsync();
