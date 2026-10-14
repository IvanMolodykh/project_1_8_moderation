using Npgsql;

namespace PaperAPI.Middleware;

public sealed class DatabaseUnavailableExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DatabaseUnavailableExceptionMiddleware> _logger;

    public DatabaseUnavailableExceptionMiddleware(
        RequestDelegate next,
        ILogger<DatabaseUnavailableExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception) when (IsDatabaseFailure(exception))
        {
            _logger.LogError(exception, "PostgreSQL недоступен при обработке {Path}", context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/problem+json";

            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "База данных недоступна",
                Detail = "Не удалось установить соединение с PostgreSQL. "
                         + "Проверьте, что сервис запущен (docker compose up -d) и настройки верны.",
                Instance = context.Request.Path,
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static bool IsDatabaseFailure(Exception exception) => exception switch
    {
        NpgsqlException => true,
        TimeoutException => true,
        System.Net.Sockets.SocketException => true,
        _ => false,
    };
}
