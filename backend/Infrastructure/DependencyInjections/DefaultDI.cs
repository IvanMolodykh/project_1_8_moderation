//using Application.Interfaces;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjections;

public static class DefaultDI
{

    public static IServiceCollection AddInfrastructure (this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddDbContext<AppDbContext>(options =>
        //    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var dbSettings = configuration.GetSection("Database");

        var connectionString = $"Host={dbSettings["Host"]};" +
                               $"Port={dbSettings["Port"]};" +
                               $"Database={dbSettings["Database"]};" +
                               $"Username={dbSettings["Username"]};" +
                               $"Password={dbSettings["Password"]};" +
                               $"Maximum Pool Size={dbSettings["MaxPoolSize"]};" +
                               $"Minimum Pool Size={dbSettings["MinPoolSize"]};" +
                               $"Timeout={dbSettings["ConnectionTimeoutSeconds"]};" +
                               $"CommandTimeout={dbSettings["CommandTimeoutSeconds"]};" +
                               $"SslMode={dbSettings["SslMode"]};";

        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());

        //services.AddScoped<IUserRepository, UserRepository>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}
