using System.Net.Http;
using System.Net.Sockets;
using Docker.DotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testcontainers.MongoDb;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;

[TestClass]
public static class MongoContainerFixture
{
    public static bool IsDockerAvailable { get; private set; }
    public static string? ConnectionString { get; private set; }

    private static MongoDbContainer? _container;

    [AssemblyInitialize]
    public static async Task InitializeAsync(TestContext _)
    {
        if (!DetectDocker())
        {
            IsDockerAvailable = false;
            return;
        }

        IsDockerAvailable = true;
        _container = new MongoDbBuilder().Build();
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
    }

    [AssemblyCleanup]
    public static async Task CleanupAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }

    private static bool DetectDocker()
    {
        // Probe the Docker daemon — only expected unavailability paths are caught
        // (Docker API errors, HTTP/socket failures, timeouts, invalid endpoint).
        // Any other exception signals a real bug and is allowed to propagate.
        try
        {
            using var config = new DockerClientConfiguration();
            using var client = config.CreateClient();
            client.System.PingAsync().GetAwaiter().GetResult();
            return true;
        }
        catch (DockerApiException)
        {
            return false;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (SocketException)
        {
            return false;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
