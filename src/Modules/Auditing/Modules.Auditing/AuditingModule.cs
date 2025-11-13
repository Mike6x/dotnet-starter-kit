using FSH.Framework.Persistence;
using FSH.Framework.Web.Modules;
using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Auditing.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FSH.Modules.Auditing;

public class AuditingModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        var httpOpts = builder.Configuration.GetSection("Auditing").Get<AuditHttpOptions>() ?? new AuditHttpOptions();
        builder.Services.AddSingleton(httpOpts);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IAuditClient, DefaultAuditClient>();
        builder.Services.AddScoped<ISecurityAudit, SecurityAudit>();
        builder.Services.AddHeroDbContext<AuditDbContext>();
        builder.Services.AddScoped<IDbInitializer, AuditDbInitializer>();
        builder.Services.AddSingleton<IAuditSerializer, SystemTextJsonAuditSerializer>();

        // Enrichers used by Audit.Configure (scoped, run on request thread)
        builder.Services.AddScoped<IAuditMaskingService, JsonMaskingService>();
        builder.Services.AddHostedService<AuditingConfigurator>();
        builder.Services.AddSingleton<IAuditScope, HttpAuditScope>();

        builder.Services.AddSingleton<ChannelAuditPublisher>();
        builder.Services.AddSingleton<IAuditPublisher>(sp => sp.GetRequiredService<ChannelAuditPublisher>());

        builder.Services.AddSingleton<IAuditSink, SqlAuditSink>();
        builder.Services.AddHostedService<AuditBackgroundWorker>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {

    }
}
