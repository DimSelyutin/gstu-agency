using FluentAssertions;
using Kursach.Web.Data.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kursach.Tests.Data;

/// <summary>
/// Тесты выбора адаптера СУБД.
/// </summary>
public class DatabaseProviderAdapterTests
{
    [Theory]
    [InlineData("SqlServer", typeof(SqlServerDatabaseAdapter))]
    [InlineData("sqlserver", typeof(SqlServerDatabaseAdapter))]
    [InlineData("MSSQL", typeof(SqlServerDatabaseAdapter))]
    [InlineData("MySql", typeof(MySqlDatabaseAdapter))]
    [InlineData("mysql", typeof(MySqlDatabaseAdapter))]
    [InlineData("MariaDB", typeof(MySqlDatabaseAdapter))]
    public void ResolveAdapter_ReturnsExpectedType(string provider, Type expectedType)
    {
        var services = new ServiceCollection();
        services.AddSingleton<SqlServerDatabaseAdapter>();
        services.AddSingleton<MySqlDatabaseAdapter>();
        var sp = services.BuildServiceProvider();

        var adapter = DatabaseServiceCollectionExtensions.ResolveAdapter(sp, provider);

        adapter.Should().BeOfType(expectedType);
        adapter.ProviderName.Should().BeOneOf(DatabaseProviderNames.SqlServer, DatabaseProviderNames.MySql);
    }

    [Fact]
    public void ResolveAdapter_UnknownProvider_Throws()
    {
        var services = new ServiceCollection();
        services.AddSingleton<SqlServerDatabaseAdapter>();
        services.AddSingleton<MySqlDatabaseAdapter>();
        var sp = services.BuildServiceProvider();

        var act = () => DatabaseServiceCollectionExtensions.ResolveAdapter(sp, "Oracle");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Oracle*");
    }

    [Fact]
    public void ResolveConnectionString_PrefersNamedConnection()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MySql"] = "Server=mysql;Database=agency;",
                ["ConnectionStrings:DefaultConnection"] = "Server=sql;Database=agency;"
            })
            .Build();

        var options = new DatabaseOptions
        {
            Provider = DatabaseProviderNames.MySql,
            ConnectionStringName = "MySql"
        };

        var cs = DatabaseServiceCollectionExtensions.ResolveConnectionString(config, options);

        cs.Should().Contain("Server=mysql");
    }

    [Fact]
    public void ResolveConnectionString_FallsBackToDefault()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=fallback;Database=agency;"
            })
            .Build();

        var options = new DatabaseOptions { Provider = DatabaseProviderNames.SqlServer };

        var cs = DatabaseServiceCollectionExtensions.ResolveConnectionString(config, options);

        cs.Should().Contain("Server=fallback");
    }

    [Fact]
    public void SqlServerAdapter_ProviderName_IsSqlServer()
    {
        new SqlServerDatabaseAdapter().ProviderName.Should().Be(DatabaseProviderNames.SqlServer);
    }

    [Fact]
    public void MySqlAdapter_ProviderName_IsMySql()
    {
        new MySqlDatabaseAdapter().ProviderName.Should().Be(DatabaseProviderNames.MySql);
    }
}
