using FSH.Framework.Tenant.Features.v1.GetTenantById;
using FSH.Framework.Web;
using FSH.Modules.Identity;
using FSH.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using FSH.Modules.Identity.Features.v1.Tokens.TokenGeneration;
using FSH.Modules.Multitenancy;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenantById;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediator(o =>
{
    o.ServiceLifetime = ServiceLifetime.Scoped;
    o.Assemblies = [
        typeof(GenerateTokenCommand),
        typeof(GenerateTokenCommandHandler),
        typeof(GetTenantByIdQuery),
        typeof(GetTenantByIdQueryHandler)];
});

var moduleAssemblies = new Assembly[]
{
    typeof(IdentityModule).Assembly,
    typeof(MultitenancyModule).Assembly
};
builder.UseFullStackHero(moduleAssemblies);



var app = builder.Build();
app.ConfigureMultiTenantDatabases();
app.ConfigureFullStackHero();
app.MapGet("/", () => "hello world!").WithTags("PlayGround").AllowAnonymous();
await app.RunAsync();