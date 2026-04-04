#if TEST_POSTGRES
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;

public sealed class EFCorePostgresTestRunner : EFCoreRelationalTestRunnerBase
{
    public EFCorePostgresTestRunner(string baseConnectionString, Action<string>? logSql = null)
        : base(CreateOptions(baseConnectionString, logSql), logSql)
    {
    }

    private static DbContextOptions<IntegrationTestDbContext> CreateOptions(
        string baseConnectionString, Action<string>? logSql)
    {
        var dbName = $"test_{Guid.NewGuid():N}";
        var connStrBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Database = dbName
        };

        // Create the database using the default connection
        using (var adminConn = new NpgsqlConnection(baseConnectionString))
        {
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\"";
            cmd.ExecuteNonQuery();
        }

        var builder = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseNpgsql(connStrBuilder.ConnectionString)
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
    }
}
#endif
