using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore;

public sealed class EFCoreSqliteTestRunner : EFCoreRelationalTestRunnerBase
{
    private readonly SqliteConnection _connection;

    public EFCoreSqliteTestRunner(Action<string>? logSql = null)
        : base(CreateOptions(out var connection, logSql))
    {
        _connection = connection;
    }

    private static DbContextOptions<IntegrationTestDbContext> CreateOptions(
        out SqliteConnection connection, Action<string>? logSql)
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseSqlite(connection)
            .UseExpressives();

        if (logSql is not null)
        {
            builder
                .LogTo(message => logSql(message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        return builder.Options;
    }

    public override async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
