using Kursach.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Tests;

/// <summary>
/// Вспомогательные методы для создания InMemory-контекста в тестах.
/// </summary>
internal static class TestDbFactory
{
    public static AgencyDbContext CreateContext(string? name = null)
    {
        var options = new DbContextOptionsBuilder<AgencyDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;
        var db = new AgencyDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }
}
