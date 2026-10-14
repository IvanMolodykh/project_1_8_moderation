using System.Globalization;
using Npgsql;
using Paper.Domain.Abstractions;
using Paper.Infrastructure.Abstractions;

namespace Paper.Infrastructure.Persistence;

public sealed class DatabaseHealthProbe
{
    private readonly IDbConnectionFactory _connections;
    private readonly IAppLogger _logger;

    public DatabaseHealthProbe(IDbConnectionFactory connections, IAppLogger logger)
    {
        _connections = connections;
        _logger = logger;
    }

    public async Task<DatabaseHealthResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await _connections.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand("SELECT version();", connection);
            var raw = await command.ExecuteScalarAsync(cancellationToken);

            var version = (raw as string ?? "unknown").Split(' ').Take(2).Aggregate((a, b) => $"{a} {b}");

            _logger.LogInformation("Проверка БД пройдена: {Version}", version);
            return DatabaseHealthResult.Healthy(version);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Проверка БД не пройдена");
            return DatabaseHealthResult.Unhealthy(exception.Message);
        }
    }
}
