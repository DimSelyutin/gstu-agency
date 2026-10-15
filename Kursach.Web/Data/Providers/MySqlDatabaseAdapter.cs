using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Data.Providers;

/// <summary>
/// Адаптер EF Core для MySQL (Oracle MySql.EntityFrameworkCore).
/// </summary>
/// <remarks>
/// Миграции SQL Server несовместимы с MySQL один в один, поэтому схема
/// создаётся через <c>EnsureCreated</c>.
/// </remarks>
public sealed class MySqlDatabaseAdapter : IDatabaseProviderAdapter
{
    /// <inheritdoc />
    public string ProviderName => DatabaseProviderNames.MySql;

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseMySQL(connectionString);
    }

    /// <inheritdoc />
    public async Task EnsureSchemaAsync(AgencyDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);
    }
}
