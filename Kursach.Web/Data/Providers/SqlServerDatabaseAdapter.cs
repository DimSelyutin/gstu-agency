using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Data.Providers;

/// <summary>
/// Адаптер EF Core для Microsoft SQL Server.
/// </summary>
public sealed class SqlServerDatabaseAdapter : IDatabaseProviderAdapter
{
    /// <inheritdoc />
    public string ProviderName => DatabaseProviderNames.SqlServer;

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseSqlServer(connectionString);
    }

    /// <inheritdoc />
    public async Task EnsureSchemaAsync(AgencyDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);
    }
}
