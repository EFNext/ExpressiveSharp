using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for EF Core integration tests that require a relational SQL
/// provider (SQLite, SQL Server, PostgreSQL, MySQL, Oracle, ...) and a
/// strongly-typed context. Feature test bases using relational-only
/// constructs — window functions (<c>ROW_NUMBER</c>, <c>RANK</c>, ...),
/// bulk <c>ExecuteUpdate</c>, typed DbSet access — derive from this class
/// rather than <see cref="EFCoreTestBase"/> so that Cosmos concrete classes
/// can ignore them.
///
/// Generic over <typeparamref name="TContext"/> so test suites with their
/// own specialized DbContext (e.g. query-filter tests) can reuse the same
/// lifecycle infrastructure.
/// </summary>
public abstract class EFCoreRelationalTestBase<TContext> : EFCoreTestBase
    where TContext : DbContext
{
    protected new TContext Context => (TContext)base.Context;
}

/// <summary>
/// Convenience alias for the common case: a relational test using
/// <see cref="IntegrationTestDbContext"/>.
/// </summary>
public abstract class EFCoreRelationalTestBase : EFCoreRelationalTestBase<IntegrationTestDbContext>
{
}
