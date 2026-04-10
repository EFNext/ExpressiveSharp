using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for all EF Core integration tests. Provides context lifecycle
/// (create, ensure-created, cleanup, drop-database) for <b>any</b> EF Core
/// provider, including non-relational ones like Cosmos DB. The
/// <see cref="Context"/> property is exposed as a base <see cref="DbContext"/>
/// so tests that need to work on Cosmos must use <c>Context.Set&lt;T&gt;()</c>
/// rather than strongly-typed DbSet properties.
///
/// Tests that require relational-only features (window functions, bulk
/// <c>ExecuteUpdate</c>, typed access to <see cref="IntegrationTestDbContext"/>
/// DbSets) should derive from <see cref="EFCoreRelationalTestBase"/> instead.
/// </summary>
public abstract class EFCoreTestBase
{
    protected DbContext Context { get; private set; } = null!;

    private IAsyncDisposable? _handle;

    /// <summary>
    /// Implemented by each per-provider concrete class. Returns an async-disposable
    /// handle whose <c>Context</c> is the live DbContext. Disposing the handle
    /// drops the per-test database (or closes the SQLite connection) and disposes
    /// the context.
    /// </summary>
    protected abstract IAsyncDisposable CreateContextHandle(out DbContext context);

    [TestInitialize]
    public async Task InitContext()
    {
        _handle = CreateContextHandle(out var ctx);
        Context = ctx;
        // EnsureCreatedAsync (not EnsureCreated) — Cosmos rejects all sync I/O.
        // Retry for Cosmos emulator transient errors when multiple TFMs run
        // their own emulator containers in parallel during CI.
        await RetryCosmosTransientAsync(() => Context.Database.EnsureCreatedAsync());
    }

    [TestCleanup]
    public async Task CleanupContext()
    {
        if (_handle is not null)
            await _handle.DisposeAsync();
    }

    /// <summary>
    /// Retries an async operation up to <paramref name="maxRetries"/> times when the
    /// Cosmos emulator returns transient errors. After all retries are exhausted the
    /// test is marked <see cref="Assert.Inconclusive"/> so emulator instability does
    /// not cause hard failures in CI. For non-Cosmos providers the operation executes
    /// once with no overhead.
    /// </summary>
    protected static async Task RetryCosmosTransientAsync(Func<Task> action, int maxRetries = 3)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (IsCosmosTransient(ex))
            {
                if (attempt >= maxRetries)
                    Assert.Inconclusive($"Cosmos emulator unavailable after {maxRetries + 1} attempts: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(2 * (attempt + 1)));
            }
        }
    }

    private static bool IsCosmosTransient(Exception ex)
    {
        // Walk the exception chain looking for Cosmos emulator errors
        for (var current = ex; current != null; current = current.InnerException)
        {
            var msg = current.Message;
            if (msg.Contains("Unauthorized (401)") ||
                msg.Contains("ServiceUnavailable (503)") ||
                msg.Contains("MAC signature") ||
                msg.Contains("Request rate is large") ||
                msg.Contains("Connection refused"))
                return true;
        }
        return false;
    }
}
