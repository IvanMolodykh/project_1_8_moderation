using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Paper.Domain.Abstractions;
using Paper.Infrastructure.Abstractions;
using Paper.Infrastructure.Configuration;
using Paper.Infrastructure.Logging;
using Paper.Infrastructure.Persistence;

namespace Paper.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? new DatabaseOptions();

        options.Validate();
        services.AddSingleton(options);

        services.AddSingleton(_ => NpgsqlDataSource.Create(options.BuildConnectionString()));

        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IAppLogger, AppLogger>();

        services.AddSingleton<DatabaseInitializer>();
        services.AddSingleton<DatabaseHealthProbe>();

        return services;
    }
}
