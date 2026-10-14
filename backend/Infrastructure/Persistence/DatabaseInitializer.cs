using Npgsql;
using Paper.Domain.Abstractions;
using Paper.Infrastructure.Abstractions;
using Paper.Infrastructure.Configuration;

namespace Paper.Infrastructure.Persistence;

public sealed class DatabaseInitializer
{
    private const string TableName = "users";

    private const string CreateTableSql =
        """
        CREATE TABLE IF NOT EXISTS users (
            id         integer      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            name       text         NOT NULL,
            email      text         NOT NULL UNIQUE,
            created_at timestamptz  NOT NULL DEFAULT now()
        );
        """;

    private readonly IDbConnectionFactory _connections;
    private readonly DatabaseOptions _options;
    private readonly IAppLogger _logger;

    public DatabaseInitializer(
        IDbConnectionFactory connections,
        DatabaseOptions options,
        IAppLogger logger)
    {
        _connections = connections;
        _options = options;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.CreateSchemaOnStartup)
        {
            _logger.LogWarning("Создание схемы на старте выключено (Database:CreateSchemaOnStartup=false)");
            return;
        }

        await using var connection = await _connections.OpenAsync(cancellationToken);

        await using (var command = new NpgsqlCommand(CreateTableSql, connection))
            await command.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Схема готова: таблица {Table} создана или уже существует", TableName);

        if (!_options.SeedData)
        {
            _logger.LogInformation("Заполнение тестовыми данными выключено (Database:SeedData=false)");
            return;
        }

        await SeedAsync(connection, cancellationToken);
    }

    private async Task SeedAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        await using var countCommand = new NpgsqlCommand($"SELECT COUNT(*) FROM {TableName};", connection);
        var total = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));

        if (total > 0)
        {
            _logger.LogInformation(
                "В таблице уже есть {Count} {Rows}, начальные данные пропущены",
                total,
                Plural(total, "строка", "строки", "строк"));

            return;
        }

        await using var insertCommand = new NpgsqlCommand(
            $"""
             INSERT INTO {TableName} (name, email) VALUES
                 ('Иван Петров',   'ivan@example.com'),
                 ('Анна Смирнова', 'anna@example.com'),
                 ('Пётр Сидоров',  'petr@example.com');
             """,
            connection);

        await insertCommand.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Добавлено 3 тестовых пользователя");
    }

    private static string Plural(int count, string one, string few, string many)
    {
        var mod100 = count % 100;

        if (mod100 is >= 11 and <= 14)
            return many;

        return (count % 10) switch
        {
            1 => one,
            2 or 3 or 4 => few,
            _ => many,
        };
    }
}