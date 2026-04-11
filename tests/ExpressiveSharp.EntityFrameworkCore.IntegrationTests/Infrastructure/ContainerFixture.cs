using Microsoft.VisualStudio.TestTools.UnitTesting;

#if TEST_SQLSERVER
using Testcontainers.MsSql;
#endif
#if TEST_POSTGRES
using Testcontainers.PostgreSql;
#endif
#if TEST_COSMOS
using System.Net.Http;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
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
    // Cosmos DB Linux emulator vNext (preview). Microsoft's officially
    // recommended image for CI/CD use; the classic emulator is documented
    // as not running reliably on hosted CI agents.
    // https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux
    private const string CosmosImage = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview";
    private const int CosmosGatewayPort = 8081;

    // Well-known emulator key (same value used by classic and vNext).
    private const string CosmosEmulatorKey =
        "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    private static IContainer? _cosmos;
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
        // The .NET Cosmos SDK reads the gateway's account metadata, which
        // returns the emulator's *internal* endpoint (localhost:8081). The
        // SDK then connects to that endpoint from the host, so we must bind
        // the host port to the same number 8081 as the container port. That
        // means only one emulator can run per host — Cosmos tests for the
        // multi-TFM matrix must be serialized in CI.
        _cosmos = new ContainerBuilder()
            .WithImage(CosmosImage)
            // .NET SDK does not support HTTP mode against the emulator.
            .WithEnvironment("PROTOCOL", "https")
            .WithPortBinding(CosmosGatewayPort, CosmosGatewayPort)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(CosmosGatewayPort))
            .Build();
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
        CosmosConnectionString =
            $"AccountEndpoint=https://localhost:{CosmosGatewayPort}/;AccountKey={CosmosEmulatorKey}";

        // The vNext emulator binds port 8081 long before its backing
        // PostgreSQL + Rust gateway can serve requests. Poll the gateway
        // root endpoint until it returns a 200 — bypass the self-signed
        // cert manually since the built-in HTTP wait strategy does not.
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        })
        {
            Timeout = TimeSpan.FromSeconds(5),
        };

        var url = $"https://localhost:{CosmosGatewayPort}/";
        var deadline = DateTime.UtcNow.AddMinutes(5);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var resp = await httpClient.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                    return;
            }
            catch
            {
                // Gateway not yet ready — keep polling.
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        throw new TimeoutException(
            $"Cosmos vNext emulator at {url} did not become ready within 5 minutes.");
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
