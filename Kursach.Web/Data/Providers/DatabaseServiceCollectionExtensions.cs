using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kursach.Web.Data.Providers;

/// <summary>
/// Регистрация адаптеров СУБД и DbContext в DI.
/// </summary>
public static class DatabaseServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет выбранный по конфигурации адаптер БД и <see cref="AgencyDbContext"/>.
    /// </summary>
    public static IServiceCollection AddAgencyDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddSingleton<SqlServerDatabaseAdapter>();
        services.AddSingleton<MySqlDatabaseAdapter>();
        services.AddSingleton<IDatabaseProviderAdapter>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            return ResolveAdapter(sp, options.Provider);
        });

        services.AddDbContext<AgencyDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var adapter = sp.GetRequiredService<IDatabaseProviderAdapter>();
            var connectionString = ResolveConnectionString(configuration, dbOptions);
            adapter.Configure(options, connectionString);
        });

        return services;
    }

    /// <summary>
    /// Выбирает адаптер по имени провайдера.
    /// </summary>
    public static IDatabaseProviderAdapter ResolveAdapter(IServiceProvider sp, string? providerName)
    {
        var normalized = (providerName ?? DatabaseProviderNames.SqlServer).Trim();

        if (normalized.Equals(DatabaseProviderNames.MySql, StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("MySQL", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("MariaDB", StringComparison.OrdinalIgnoreCase))
        {
            return sp.GetRequiredService<MySqlDatabaseAdapter>();
        }

        if (normalized.Equals(DatabaseProviderNames.SqlServer, StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("MSSQL", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("SQLServer", StringComparison.OrdinalIgnoreCase))
        {
            return sp.GetRequiredService<SqlServerDatabaseAdapter>();
        }

        throw new InvalidOperationException(
            $"Неизвестный Database:Provider '{providerName}'. Допустимо: SqlServer, MySql.");
    }

    /// <summary>
    /// Возвращает строку подключения для выбранного провайдера.
    /// </summary>
    public static string ResolveConnectionString(IConfiguration configuration, DatabaseOptions options)
    {
        var names = new List<string>();
        if (!string.IsNullOrWhiteSpace(options.ConnectionStringName))
        {
            names.Add(options.ConnectionStringName);
        }

        names.Add(options.Provider);
        names.Add("DefaultConnection");

        foreach (var name in names.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var value = configuration.GetConnectionString(name);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        throw new InvalidOperationException(
            $"Не найдена строка подключения для провайдера '{options.Provider}'. " +
            "Укажите ConnectionStrings:SqlServer / ConnectionStrings:MySql или ConnectionStrings:DefaultConnection.");
    }
}
