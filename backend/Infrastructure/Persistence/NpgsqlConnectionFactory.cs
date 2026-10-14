using Microsoft.Extensions.Logging;
using Npgsql;
using Paper.Infrastructure.Abstractions;

namespace Paper.Infrastructure.Persistence;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<NpgsqlConnectionFactory> _logger;

    public NpgsqlConnectionFactory(NpgsqlDataSource dataSource, ILogger<NpgsqlConnectionFactory> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    public async Task<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        _logger.LogDebug("Взято соединение из пула ({State})", connection.State);

        return connection;
    }
}
