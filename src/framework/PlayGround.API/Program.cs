using FSH.Framework.Web.Cors;
using FSH.Framework.Web.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.EnableApiDocs(builder.Configuration);
builder.Services.EnableCors(builder.Configuration);
var app = builder.Build();
app.ExposeApiDocs();
app.ExposeCors();

app.UseHttpsRedirection();

app.MapGet("/", () => "hello world!");

await app.RunAsync();
