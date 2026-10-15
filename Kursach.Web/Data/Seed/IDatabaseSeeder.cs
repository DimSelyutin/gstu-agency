using Kursach.Web.Data;

namespace Kursach.Web.Data.Seed;

/// <summary>
/// Контракт инициализации базы данных демонстрационными данными.
/// </summary>
public interface IDatabaseSeeder
{
    /// <summary>
    /// Заполняет пустую базу данных тестовыми записями.
    /// Операция идемпотентна: повторный вызов не добавляет данные, если они уже есть.
    /// </summary>
    Task SeedAsync(AgencyDbContext db, CancellationToken cancellationToken = default);
}
