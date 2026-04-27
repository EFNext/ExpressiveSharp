using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

// Relational-provider base for tests that use SQL-only constructs (window functions,
// ExecuteUpdate, typed DbSet access). Cosmos concrete classes ignore these.
public abstract class EFCoreRelationalTestBase<TContext> : EFCoreTestBase
    where TContext : DbContext
{
    protected new TContext Context => (TContext)base.Context;
}

public abstract class EFCoreRelationalTestBase : EFCoreRelationalTestBase<IntegrationTestDbContext>
{
}
