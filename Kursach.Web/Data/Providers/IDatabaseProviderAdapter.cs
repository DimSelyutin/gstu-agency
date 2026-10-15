using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Data.Providers;

/// <summary>
/// Адаптер СУБД: настраивает EF Core под конкретный провайдер.
/// </summary>
public interface IDatabaseProviderAdapter
{
    /// <summary>
    /// Имя провайдера (<see cref="DatabaseProviderNames"/>).
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Конфигурирует <see cref="DbContextOptionsBuilder"/> для выбранной СУБД.
    /// </summary>
    /// <param name="options">Строитель параметров контекста.</param>
    /// <param name="connectionString">Строка подключения.</param>
    void Configure(DbContextOptionsBuilder options, string connectionString);

    /// <summary>
    /// Применяет схему БД при старте (миграции или EnsureCreated).
    /// </summary>
    /// <param name="db">Контекст.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task EnsureSchemaAsync(AgencyDbContext db, CancellationToken cancellationToken = default);
}
