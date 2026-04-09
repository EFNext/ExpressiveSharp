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
        // Retry for Cosmos emulator transient errors (401/503) when multiple
        // TFMs run their own emulator containers in parallel during CI.
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
    /// Cosmos emulator returns transient errors (401 Unauthorized / MAC signature
    /// mismatch, 503 Service Unavailable). These occur when multiple test processes
    /// run emulator containers in parallel during CI. For non-Cosmos providers the
    /// operation executes once with no overhead.
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
            catch (Exception ex) when (attempt < maxRetries && IsCosmosTransient(ex))
            {
                await Task.Delay(TimeSpan.FromSeconds(2 * (attempt + 1)));
            }
        }
    }

    private static bool IsCosmosTransient(Exception ex)
    {
        // Walk the exception chain looking for Cosmos-specific transient errors
        for (var current = ex; current != null; current = current.InnerException)
        {
            var msg = current.Message;
            if (msg.Contains("Unauthorized (401)") ||
                msg.Contains("ServiceUnavailable (503)") ||
                msg.Contains("MAC signature") ||
                msg.Contains("Request rate is large"))
                return true;
        }
        return false;
    }
}
