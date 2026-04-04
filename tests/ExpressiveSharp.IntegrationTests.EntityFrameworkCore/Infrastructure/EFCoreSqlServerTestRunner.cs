#if TEST_SQLSERVER
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;

public sealed class EFCoreSqlServerTestRunner : EFCoreRelationalTestRunnerBase
{
    public EFCoreSqlServerTestRunner(string baseConnectionString, Action<string>? logSql = null)
        : base(CreateOptions(baseConnectionString, logSql))
    {
    }

    private static DbContextOptions<IntegrationTestDbContext> CreateOptions(
        string baseConnectionString, Action<string>? logSql)
    {
        // Each runner instance gets a unique database for test isolation
        var dbName = $"test_{Guid.NewGuid():N}";
        var connStr = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnectionString)
        {
            InitialCatalog = dbName
        }.ConnectionString;

        var builder = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseSqlServer(connStr)
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
