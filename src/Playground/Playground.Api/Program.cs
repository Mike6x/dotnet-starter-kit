using FSH.Framework.Tenant.Features.v1.GetTenantById;
using FSH.Framework.Web;
using FSH.Framework.Web.Modules;
using FSH.Modules.Auditing;
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
    typeof(MultitenancyModule).Assembly,
    typeof(AuditingModule).Assembly
};

builder.AddFshPlatform(o =>
{
    o.EnableCors = true;
    o.EnableOpenApi = true;
    o.EnableCaching = true;
    o.EnableMailing = true;
    o.EnableJobs = true;
});

builder.AddModules(moduleAssemblies);



var app = builder.Build();
app.ConfigureMultiTenantDatabases();
app.UseFshPlatform(p => { p.MapModules = true; });
app.MapGet("/", () => "hello world!").WithTags("PlayGround").AllowAnonymous();
await app.RunAsync();
