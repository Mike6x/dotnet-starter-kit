using FSH.Framework.Shared.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Persistence;

internal sealed class DatabaseOptionsLogger : IPostConfigureOptions<DatabaseOptions>
{
    private readonly ILogger<DatabaseOptionsLogger> _logger;
    public DatabaseOptionsLogger(ILogger<DatabaseOptionsLogger> logger) => _logger = logger;

    public void PostConfigure(string? name, DatabaseOptions options)
    {
        _logger.LogInformation("current db provider: {Provider}", options.Provider);
        _logger.LogInformation("for docs: https://www.fullstackhero.net");
        _logger.LogInformation("sponsor: https://opencollective.com/fullstackhero");
    }
}