using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
#if !NET10_0_OR_GREATER
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
#endif

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests.Infrastructure;

/// <summary>
/// Creates <see cref="WindowTestDbContext"/> instances configured for different database providers.
/// Used by SQL shape tests to verify generated SQL across SQLite, SQL Server, and PostgreSQL.
/// No real database connection is needed — only <c>ToQueryString()</c> is called.
/// </summary>
internal static class TestProvider
{
    public static WindowTestDbContext CreateSqliteContext()
    {
        // SQLite needs a real in-memory connection for ToQueryString to work
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<WindowTestDbContext>()
            .UseSqlite(connection)
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;
        var ctx = new WindowTestDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    public static WindowTestDbContext CreateSqlServerContext()
    {
        var options = new DbContextOptionsBuilder<WindowTestDbContext>()
            .UseSqlServer("Server=dummy;Database=dummy;Encrypt=false")
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;
        return new WindowTestDbContext(options);
    }

    public static WindowTestDbContext CreateNpgsqlContext()
    {
        var options = new DbContextOptionsBuilder<WindowTestDbContext>()
            .UseNpgsql("Host=dummy;Database=dummy")
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;
        return new WindowTestDbContext(options);
    }

#if !NET10_0_OR_GREATER
    // Pomelo has no EF Core 10 release yet — MySQL tests run on net8.0 only
    public static WindowTestDbContext CreateMySqlContext()
    {
        var options = new DbContextOptionsBuilder<WindowTestDbContext>()
            .UseMySql("Server=dummy;Database=dummy", MySqlServerVersion.LatestSupportedServerVersion)
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;
        return new WindowTestDbContext(options);
    }
#endif

    public static WindowTestDbContext CreateOracleContext()
    {
        var options = new DbContextOptionsBuilder<WindowTestDbContext>()
            .UseOracle("Data Source=dummy;User Id=dummy;Password=dummy")
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;
        return new WindowTestDbContext(options);
    }
}
