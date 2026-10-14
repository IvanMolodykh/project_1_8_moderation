using Paper.Application.DependencyInjection;
using Paper.Infrastructure.Configuration;
using Paper.Infrastructure.DependencyInjection;
using Paper.Infrastructure.Persistence;
using PaperAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseDefaultServiceProvider((context, options) =>
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment());

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var databaseOptions = app.Services.GetRequiredService<DatabaseOptions>();
app.Logger.LogInformation("PostgreSQL: {Connection}", databaseOptions.BuildRedactedDescription());

var initializer = app.Services.GetRequiredService<DatabaseInitializer>();

try
{
    await initializer.InitializeAsync();
}
catch (Exception exception)
{
    app.Logger.LogError(
        exception,
        "Не удалось подготовить схему БД. Приложение продолжает работу. "
        + "Проверьте, что PostgreSQL запущен (docker compose up -d) и настройки верны.");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<DatabaseUnavailableExceptionMiddleware>();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();

public partial class Program;
