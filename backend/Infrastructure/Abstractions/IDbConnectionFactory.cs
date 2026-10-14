using Npgsql;

namespace Paper.Infrastructure.Abstractions;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken = default);
}
