#if TEST_POSTGRES
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;

public sealed class EFCorePostgresTestRunner : EFCoreRelationalTestRunnerBase
{
    private readonly NpgsqlDataSource _dataSource;

    public EFCorePostgresTestRunner(string baseConnectionString, Action<string>? logSql = null)
        : base(CreateOptions(baseConnectionString, out var dataSource, logSql), logSql)
    {
        _dataSource = dataSource;
    }

    private static DbContextOptions<IntegrationTestDbContext> CreateOptions(
        string baseConnectionString, out NpgsqlDataSource dataSource, Action<string>? logSql)
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

        // Build a data source with tuple support enabled
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connStrBuilder.ConnectionString);
        dataSourceBuilder.EnableRecordsAsTuples();
        dataSource = dataSourceBuilder.Build();

        var builder = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseNpgsql(dataSource)
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
        await _dataSource.DisposeAsync();
    }
}
#endif
