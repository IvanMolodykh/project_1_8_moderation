using System.Globalization;
using Npgsql;
using Paper.Domain.Abstractions;
using Paper.Domain.Entities;
using Paper.Infrastructure.Abstractions;

namespace Paper.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private const string TableName = "users";

    private readonly IDbConnectionFactory _connections;

    public UserRepository(IDbConnectionFactory connections)
    {
        _connections = connections;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            $"""
             SELECT id, name, email, created_at
             FROM {TableName}
             ORDER BY id;
             """,
            connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var users = new List<User>();
        while (await reader.ReadAsync(cancellationToken))
            users.Add(Map(reader));

        return users;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            $"""
             SELECT id, name, email, created_at
             FROM {TableName}
             WHERE id = @id;
             """,
            connection);

        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<User> AddAsync(string name, string email, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            $"""
             INSERT INTO {TableName} (name, email)
             VALUES (@name, @email)
             RETURNING id, name, email, created_at;
             """,
            connection);

        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            throw new InvalidOperationException("INSERT ... RETURNING не вернул строку.");

        return Map(reader);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connections.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            $"""
             SELECT COUNT(*) FROM {TableName};
             """,
            connection);

        var result = await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(result, CultureInfo.InvariantCulture);
    }

    private static User Map(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1),
        Email = reader.GetString(2),
        CreatedAt = reader.GetFieldValue<DateTimeOffset>(3),
    };
}
