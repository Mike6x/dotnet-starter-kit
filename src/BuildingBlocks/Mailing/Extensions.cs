using FSH.Framework.Mailing.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Mailing;

public static class Extensions
{
    public static IServiceCollection AddHeroMailing(this IServiceCollection services)
    {
        services.AddTransient<IMailService, SmtpMailService>();
        services.AddOptions<MailOptions>().BindConfiguration(nameof(MailOptions));
        return services;
    }
}