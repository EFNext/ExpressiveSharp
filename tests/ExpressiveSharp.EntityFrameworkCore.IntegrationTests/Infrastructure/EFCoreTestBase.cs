using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

// Provider-agnostic context lifecycle. Context is exposed as base DbContext so
// Cosmos-compatible tests must use Context.Set<T>() instead of typed DbSets.
public abstract class EFCoreTestBase
{
    protected DbContext Context { get; private set; } = null!;

    private IAsyncDisposable? _handle;

    // Returns an async-disposable handle whose Context is the live DbContext.
    // Disposing drops the per-test database (or closes the SQLite connection).
    protected abstract IAsyncDisposable CreateContextHandle(out DbContext context);

    [TestInitialize]
    public async Task InitContext()
    {
        _handle = CreateContextHandle(out var ctx);
        Context = ctx;
        // EnsureCreatedAsync (not EnsureCreated) — Cosmos rejects all sync I/O.
        await Context.Database.EnsureCreatedAsync();
    }

    [TestCleanup]
    public async Task CleanupContext()
    {
        if (_handle is not null)
            await _handle.DisposeAsync();
    }
}
