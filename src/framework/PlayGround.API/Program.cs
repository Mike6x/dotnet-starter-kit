using FSH.Framework.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.UseFullStackHero();

var app = builder.Build();

app.ConfigureFullStackHero();
app.MapGet("/", () => "hello world!").WithTags("PlayGround");

await app.RunAsync();
