using FSH.Framework.Infrastructure;
using FSH.Framework.Infrastructure.Multitenancy;

var builder = WebApplication.CreateBuilder(args);
builder.UseFullStackHero();

var app = builder.Build();

app.ConfigureMultiTenantDatabases();
app.ConfigureFullStackHero();
app.MapGet("/", () => "hello world!").WithTags("PlayGround").AllowAnonymous();

await app.RunAsync();
