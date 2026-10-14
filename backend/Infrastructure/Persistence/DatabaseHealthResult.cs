namespace Paper.Infrastructure.Persistence;

public sealed record DatabaseHealthResult(bool IsHealthy, string? ServerVersion, string? Error)
{
    public static DatabaseHealthResult Healthy(string version) => new(true, version, null);

    public static DatabaseHealthResult Unhealthy(string error) => new(false, null, error);
}
