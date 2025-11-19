using FSH.Framework.Blazor.UI;
using FSH.Playground.Blazor.Components;
using FSH.Playground.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHeroUI();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITokenStore, InMemoryTokenStore>();
builder.Services.AddTransient<BffAuthDelegatingHandler>();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7030");
}).AddHttpMessageHandler<BffAuthDelegatingHandler>();

builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7030");
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapBffAuthEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
