using Npgsql;

namespace Paper.Infrastructure.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string? ConnectionString { get; set; }

    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 5432;

    public string Database { get; set; } = "moderationdb";

    public string Username { get; set; } = "postgres";

    public string Password { get; set; } = "postgres";

    public int MaxPoolSize { get; set; } = 10;

    public int MinPoolSize { get; set; }

    public int ConnectionTimeoutSeconds { get; set; } = 5;

    public int CommandTimeoutSeconds { get; set; } = 30;

    public string SslMode { get; set; } = "Prefer";

    public bool CreateSchemaOnStartup { get; set; } = true;

    public bool SeedData { get; set; } = true;

    public void Validate()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
            return;

        var problems = new List<string>();

        if (string.IsNullOrWhiteSpace(Host))
            problems.Add("Host пуст.");

        if (string.IsNullOrWhiteSpace(Database))
            problems.Add("Database пустое.");

        if (string.IsNullOrWhiteSpace(Username))
            problems.Add("Username пуст.");

        if (string.IsNullOrWhiteSpace(Password))
            problems.Add("Password пуст.");

        if (Port is <= 0 or > 65535)
            problems.Add($"Port ({Port}) вне диапазона 1..65535.");

        if (MaxPoolSize <= 0)
            problems.Add($"MaxPoolSize ({MaxPoolSize}) должен быть больше нуля.");

        if (MinPoolSize < 0)
            problems.Add($"MinPoolSize ({MinPoolSize}) не может быть отрицательным.");

        if (MinPoolSize > MaxPoolSize)
            problems.Add($"MinPoolSize ({MinPoolSize}) больше MaxPoolSize ({MaxPoolSize}).");

        if (!Enum.TryParse<SslMode>(SslMode, ignoreCase: true, out _))
        {
            problems.Add(
                $"SslMode ('{SslMode}') недопустим. Ожидается одно из: {string.Join(", ", Enum.GetNames<SslMode>())}.");
        }

        if (problems.Count > 0)
        {
            throw new InvalidOperationException(
                $"Секция \"{SectionName}\" в конфигурации заполнена некорректно:" +
                Environment.NewLine + "  - " + string.Join(Environment.NewLine + "  - ", problems));
        }
    }

    public string BuildConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
            return ConnectionString;

        return new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password,
            MaxPoolSize = MaxPoolSize,
            MinPoolSize = MinPoolSize,
            Timeout = ConnectionTimeoutSeconds,
            CommandTimeout = CommandTimeoutSeconds,
            SslMode = Enum.Parse<SslMode>(SslMode, ignoreCase: true),
        }.ConnectionString;
    }

    public string BuildRedactedDescription()
    {
        var builder = new NpgsqlConnectionStringBuilder(BuildConnectionString());
        builder.Password = string.IsNullOrEmpty(builder.Password) ? builder.Password : "***";

        return $"{builder.Host}:{builder.Port}/{builder.Database} (user={builder.Username}, pool={builder.MinPoolSize}..{builder.MaxPoolSize})";
    }
}
