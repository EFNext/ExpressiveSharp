using Microsoft.VisualStudio.TestTools.UnitTesting;

#if TEST_SQLSERVER
using Testcontainers.MsSql;
#endif
#if TEST_POSTGRES
using Testcontainers.PostgreSql;
#endif
#if TEST_COSMOS
using Testcontainers.CosmosDb;
#endif
#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
using Testcontainers.MySql;
#endif

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

#if TEST_SQLSERVER || TEST_POSTGRES || TEST_COSMOS || (TEST_POMELO_MYSQL && !NET10_0_OR_GREATER)
[TestClass]
public static class ContainerFixture
{
    public static bool IsDockerAvailable { get; private set; }

#if TEST_SQLSERVER
    private static MsSqlContainer? _sqlServer;
    public static string? SqlServerConnectionString { get; private set; }
#endif

#if TEST_POSTGRES
    private static PostgreSqlContainer? _postgres;
    public static string? PostgresConnectionString { get; private set; }
#endif

#if TEST_COSMOS
    private static CosmosDbContainer? _cosmos;
    public static string? CosmosConnectionString { get; private set; }
#endif

#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
    private static MySqlContainer? _mysql;
    public static string? MySqlConnectionString { get; private set; }
#endif

    [AssemblyInitialize]
    public static async Task InitializeAsync(TestContext _)
    {
        if (!DetectDocker())
        {
            IsDockerAvailable = false;
            return;
        }

        IsDockerAvailable = true;

        var tasks = new List<Task>();

#if TEST_SQLSERVER
        _sqlServer = new MsSqlBuilder().Build();
        tasks.Add(StartSqlServerAsync());
#endif
#if TEST_POSTGRES
        _postgres = new PostgreSqlBuilder().Build();
        tasks.Add(StartPostgresAsync());
#endif
#if TEST_COSMOS
        _cosmos = new CosmosDbBuilder().Build();
        tasks.Add(StartCosmosAsync());
#endif
#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
        _mysql = new MySqlBuilder()
            .WithUsername("root")
            .WithPassword("test")
            .Build();
        tasks.Add(StartMySqlAsync());
#endif

        await Task.WhenAll(tasks);
    }

    [AssemblyCleanup]
    public static async Task CleanupAsync()
    {
        var tasks = new List<Task>();

#if TEST_SQLSERVER
        if (_sqlServer is not null) tasks.Add(_sqlServer.DisposeAsync().AsTask());
#endif
#if TEST_POSTGRES
        if (_postgres is not null) tasks.Add(_postgres.DisposeAsync().AsTask());
#endif
#if TEST_COSMOS
        if (_cosmos is not null) tasks.Add(_cosmos.DisposeAsync().AsTask());
#endif
#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
        if (_mysql is not null) tasks.Add(_mysql.DisposeAsync().AsTask());
#endif

        await Task.WhenAll(tasks);
    }

    private static bool DetectDocker()
    {
        try
        {
            using var client = new Docker.DotNet.DockerClientConfiguration().CreateClient();
            client.System.PingAsync().GetAwaiter().GetResult();
            return true;
        }
        catch
        {
            return false;
        }
    }

#if TEST_SQLSERVER
    private static async Task StartSqlServerAsync()
    {
        await _sqlServer!.StartAsync();
        SqlServerConnectionString = _sqlServer.GetConnectionString();
    }
#endif

#if TEST_POSTGRES
    private static async Task StartPostgresAsync()
    {
        await _postgres!.StartAsync();
        PostgresConnectionString = _postgres.GetConnectionString();
    }
#endif

#if TEST_COSMOS
    private static async Task StartCosmosAsync()
    {
        await _cosmos!.StartAsync();
        CosmosConnectionString = _cosmos.GetConnectionString();
    }
#endif

#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
    private static async Task StartMySqlAsync()
    {
        await _mysql!.StartAsync();
        MySqlConnectionString = _mysql.GetConnectionString();
    }
#endif
}
#endif
