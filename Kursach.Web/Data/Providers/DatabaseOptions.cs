namespace Kursach.Web.Data.Providers;

/// <summary>
/// Настройки выбора СУБД.
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// Секция конфигурации.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// Имя провайдера: <see cref="DatabaseProviderNames.SqlServer"/> или <see cref="DatabaseProviderNames.MySql"/>.
    /// </summary>
    public string Provider { get; set; } = DatabaseProviderNames.SqlServer;

    /// <summary>
    /// Имя connection string в секции ConnectionStrings.
    /// Если пусто — используется имя провайдера (<c>SqlServer</c>/<c>MySql</c>),
    /// либо запасной вариант <c>DefaultConnection</c>.
    /// </summary>
    public string? ConnectionStringName { get; set; }
}
