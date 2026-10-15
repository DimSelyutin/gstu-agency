using Kursach.Web.Data.Providers;
using Kursach.Web.Data.Seed;

namespace Kursach.Web.Data;

/// <summary>
/// Расширения для применения схемы и сидинга при старте приложения.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Применяет схему через адаптер СУБД и заполняет БД, если она пуста.
    /// </summary>
    public static async Task InitializeAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AgencyDbContext>();
        var adapter = scope.ServiceProvider.GetRequiredService<IDatabaseProviderAdapter>();
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseInitializer");

        logger.LogInformation("Ensuring database schema via {Provider} adapter...", adapter.ProviderName);
        await adapter.EnsureSchemaAsync(db);

        logger.LogInformation("Seeding database if empty...");
        await seeder.SeedAsync(db);
        logger.LogInformation("Database initialization completed ({Provider}).", adapter.ProviderName);
    }
}
