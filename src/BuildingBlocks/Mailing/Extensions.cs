using FSH.Framework.Mailing.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Mailing;

internal static class Extensions
{
    internal static IServiceCollection AddHeroMailing(this IServiceCollection services)
    {
        services.AddTransient<IMailService, SmtpMailService>();
        services.AddOptions<MailOptions>().BindConfiguration(nameof(MailOptions));
        return services;
    }
}