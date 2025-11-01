using FSH.Framework.Web.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.EnableApiDocs(builder.Configuration);
var app = builder.Build();
app.ExposeApiDocs();
app.UseHttpsRedirection();

app.MapGet("/", () => "hello world!");

await app.RunAsync();
