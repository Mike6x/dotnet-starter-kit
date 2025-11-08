using FSH.Framework.Core.Mailing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Infrastructure.Mailing;
internal static class Extensions
{
    internal static IServiceCollection AddHeroMailing(this IServiceCollection services)
    {
        services.AddTransient<IMailService, SmtpMailService>();
        services.AddOptions<MailOptions>().BindConfiguration(nameof(MailOptions));
        return services;
    }
}