using FSH.Framework.Blazor.UI;
using FSH.Framework.Blazor.UI.Theme;
using FSH.Playground.Blazor;
using FSH.Playground.Blazor.Components;
using FSH.Playground.Blazor.Services;
using FSH.Playground.Blazor.Services.Api;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

var builder = WebApplication.CreateBuilder(args);

// Configure HTTP/3 support (only override in production, respect launchSettings in dev)
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8080, listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
        });
    });
}

builder.Services.AddHeroUI();

// Authentication & Authorization
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

// Cookie Authentication for SSR support
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Distributed Cache (required by theme state factory)
builder.Services.AddDistributedMemoryCache();

// Simple cookie-based authentication
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

// Tenant theme services
builder.Services.AddScoped<ITenantThemeState, TenantThemeState>(); // For Interactive mode
builder.Services.AddScoped<IThemeStateFactory, CachedThemeStateFactory>(); // For SSR mode

// User profile state for syncing across components
builder.Services.AddScoped<IUserProfileState, UserProfileState>();

// Authorization header handler for API calls
builder.Services.AddScoped<AuthorizationHeaderHandler>();

builder.Services.AddHttpClient();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
                 ?? throw new InvalidOperationException("Api:BaseUrl configuration is missing.");

// Configure HttpClient with authorization handler for API calls
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationHeaderHandler>();
    handler.InnerHandler = new HttpClientHandler();

    return new HttpClient(handler)
    {
        BaseAddress = new Uri(apiBaseUrl)
    };
});

builder.Services.AddApiClients(builder.Configuration);

// Response Compression for static assets and API responses
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// Output Caching for static responses
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder
        .With(c => c.HttpContext.Request.Path.StartsWithSegments("/health"))
        .Expire(TimeSpan.FromSeconds(10)));
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Simple health endpoints for ALB/ECS
app.MapGet("/health/ready", () => Results.Ok(new { status = "Healthy" }))
   .AllowAnonymous();

app.MapGet("/health/live", () => Results.Ok(new { status = "Alive" }))
   .AllowAnonymous();

app.UseResponseCompression(); // Must come before UseStaticFiles
app.UseOutputCache();
app.UseHttpsRedirection();
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();
app.UseAntiforgery();

app.MapSimpleBffAuthEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
