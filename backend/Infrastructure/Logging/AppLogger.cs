using Microsoft.Extensions.Logging;
using Paper.Domain.Abstractions;

namespace Paper.Infrastructure.Logging;

public sealed class AppLogger : IAppLogger
{
    private readonly ILogger<AppLogger> _logger;

    public AppLogger(ILogger<AppLogger> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object?[] args)
        => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object?[] args)
        => _logger.LogWarning(message, args);

    public void LogError(Exception exception, string message, params object?[] args)
        => _logger.LogError(exception, message, args);
}
