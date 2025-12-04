using Microsoft.Extensions.DependencyInjection;
using FSH.Playground.Blazor.Services.Api;
using FSH.Playground.Blazor.Services.Api.Audits;

namespace FSH.Playground.Blazor;

public static class ApiClientRegistration
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        services.AddTransient<ProfileClient>();
        services.AddTransient<AuditClient>();
        services.AddTransient<FSH.Playground.Blazor.Services.Api.Dashboard.DashboardClient>();
        return services;
    }
}
