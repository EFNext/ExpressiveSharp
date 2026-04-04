#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;

public sealed class EFCorePomeloMySqlTestRunner : EFCoreRelationalTestRunnerBase
{
    public EFCorePomeloMySqlTestRunner(string baseConnectionString, Action<string>? logSql = null)
        : base(CreateOptions(baseConnectionString, logSql))
    {
    }

    private static DbContextOptions<IntegrationTestDbContext> CreateOptions(
        string baseConnectionString, Action<string>? logSql)
    {
        var dbName = $"test_{Guid.NewGuid():N}";
        var connStrBuilder = new MySqlConnectionStringBuilder(baseConnectionString)
        {
            Database = dbName
        };

        // Create the database using the default connection
        using (var adminConn = new MySqlConnection(baseConnectionString))
        {
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE `{dbName}`";
            cmd.ExecuteNonQuery();
        }

        var serverVersion = ServerVersion.AutoDetect(connStrBuilder.ConnectionString);

        var builder = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseMySql(connStrBuilder.ConnectionString, serverVersion)
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
        try
        {
            await Context.Database.EnsureDeletedAsync();
        }
        finally
        {
            await Context.DisposeAsync();
        }
    }
}
#endif
