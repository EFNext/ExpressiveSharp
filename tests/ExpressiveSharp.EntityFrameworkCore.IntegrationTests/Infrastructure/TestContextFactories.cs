using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

#if TEST_POSTGRES
using Npgsql;
#endif

#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
using MySqlConnector;
#endif

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Per-provider context factories used by test bases that need direct
/// <see cref="DbContext"/> access (tests that bypass <c>IIntegrationTestRunner</c>).
///
/// Each factory creates a fresh, uniquely-named database inside the shared
/// container (or an in-memory SQLite connection) and returns a disposable
/// handle that the caller disposes in test cleanup.
/// </summary>
internal static class TestContextFactories
{
    // ── SQLite ──────────────────────────────────────────────────────────

    public static SqliteContextHandle<IntegrationTestDbContext> CreateSqlite()
        => CreateSqlite<IntegrationTestDbContext>(opt => new IntegrationTestDbContext(opt));

    public static SqliteContextHandle<QueryFilterTestDbContext> CreateSqliteQueryFilter()
        => CreateSqlite<QueryFilterTestDbContext>(opt => new QueryFilterTestDbContext(opt));

    public static SqliteContextHandle<TContext> CreateSqlite<TContext>(
        Func<DbContextOptions<TContext>, TContext> factory) where TContext : DbContext
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(connection)
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;

        return new SqliteContextHandle<TContext>(factory(options), connection);
    }

    public sealed class SqliteContextHandle<TContext>(TContext context, SqliteConnection connection) : IAsyncDisposable
        where TContext : DbContext
    {
        public TContext Context { get; } = context;
        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await connection.DisposeAsync();
        }
    }

    // ── SQL Server ──────────────────────────────────────────────────────

#if TEST_SQLSERVER
    public static SqlServerContextHandle<IntegrationTestDbContext> CreateSqlServer()
        => CreateSqlServer<IntegrationTestDbContext>(opt => new IntegrationTestDbContext(opt));

    public static SqlServerContextHandle<QueryFilterTestDbContext> CreateSqlServerQueryFilter()
        => CreateSqlServer<QueryFilterTestDbContext>(opt => new QueryFilterTestDbContext(opt));

    public static SqlServerContextHandle<TContext> CreateSqlServer<TContext>(
        Func<DbContextOptions<TContext>, TContext> factory) where TContext : DbContext
    {
        var dbName = $"test_{Guid.NewGuid():N}";
        var connStr = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(
            ContainerFixture.SqlServerConnectionString!)
        {
            InitialCatalog = dbName
        }.ConnectionString;

        var options = new DbContextOptionsBuilder<TContext>()
            .UseSqlServer(connStr)
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;

        return new SqlServerContextHandle<TContext>(factory(options));
    }

    public sealed class SqlServerContextHandle<TContext>(TContext context) : IAsyncDisposable
        where TContext : DbContext
    {
        public TContext Context { get; } = context;
        public async ValueTask DisposeAsync()
        {
            try { await Context.Database.EnsureDeletedAsync(); }
            finally { await Context.DisposeAsync(); }
        }
    }
#endif

    // ── PostgreSQL ──────────────────────────────────────────────────────

#if TEST_POSTGRES
    public static PostgresContextHandle<IntegrationTestDbContext> CreatePostgres()
        => CreatePostgres<IntegrationTestDbContext>(opt => new IntegrationTestDbContext(opt));

    public static PostgresContextHandle<QueryFilterTestDbContext> CreatePostgresQueryFilter()
        => CreatePostgres<QueryFilterTestDbContext>(opt => new QueryFilterTestDbContext(opt));

    public static PostgresContextHandle<TContext> CreatePostgres<TContext>(
        Func<DbContextOptions<TContext>, TContext> factory) where TContext : DbContext
    {
        var dbName = $"test_{Guid.NewGuid():N}";
        var baseConnStr = ContainerFixture.PostgresConnectionString!;
        var connStrBuilder = new NpgsqlConnectionStringBuilder(baseConnStr)
        {
            Database = dbName
        };

        using (var adminConn = new NpgsqlConnection(baseConnStr))
        {
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\"";
            cmd.ExecuteNonQuery();
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connStrBuilder.ConnectionString);
        dataSourceBuilder.EnableRecordsAsTuples();
        var dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(dataSource)
            .UseExpressives(o => o.UseRelationalExtensions())
            .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
            .Options;

        return new PostgresContextHandle<TContext>(factory(options), dataSource);
    }

    public sealed class PostgresContextHandle<TContext>(TContext context, NpgsqlDataSource dataSource) : IAsyncDisposable
        where TContext : DbContext
    {
        public TContext Context { get; } = context;
        public async ValueTask DisposeAsync()
        {
            try { await Context.Database.EnsureDeletedAsync(); }
            finally
            {
                await Context.DisposeAsync();
                await dataSource.DisposeAsync();
            }
        }
    }
#endif

    // ── MySQL (Pomelo) ──────────────────────────────────────────────────

#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
    public static PomeloMySqlContextHandle<IntegrationTestDbContext> CreatePomeloMySql()
        => CreatePomeloMySql<IntegrationTestDbContext>(opt => new IntegrationTestDbContext(opt));

    public static PomeloMySqlContextHandle<QueryFilterTestDbContext> CreatePomeloMySqlQueryFilter()
        => CreatePomeloMySql<QueryFilterTestDbContext>(opt => new QueryFilterTestDbContext(opt));

    public static PomeloMySqlContextHandle<TContext> CreatePomeloMySql<TContext>(
        Func<DbContextOptions<TContext>, TContext> factory) where TContext : DbContext
    {
        var dbName = $"test_{Guid.NewGuid():N}";
        var baseConnStr = ContainerFixture.MySqlConnectionString!;
        var connStrBuilder = new MySqlConnectionStringBuilder(baseConnStr)
        {
            Database = dbName
        };

        using (var adminConn = new MySqlConnection(baseConnStr))
        {
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE `{dbName}`";
            cmd.ExecuteNonQuery();
        }

        var serverVersion = ServerVersion.AutoDetect(connStrBuilder.ConnectionString);
        var options = new DbContextOptionsBuilder<TContext>()
            .UseMySql(connStrBuilder.ConnectionString, serverVersion)
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;

        return new PomeloMySqlContextHandle<TContext>(factory(options));
    }

    public sealed class PomeloMySqlContextHandle<TContext>(TContext context) : IAsyncDisposable
        where TContext : DbContext
    {
        public TContext Context { get; } = context;
        public async ValueTask DisposeAsync()
        {
            try { await Context.Database.EnsureDeletedAsync(); }
            finally { await Context.DisposeAsync(); }
        }
    }
#endif

    // ── Cosmos DB ───────────────────────────────────────────────────────

#if TEST_COSMOS
    public static CosmosContextHandle CreateCosmos()
    {
        var dbName = $"test_{Guid.NewGuid():N}";
        var options = new DbContextOptionsBuilder<CosmosIntegrationTestDbContext>()
            .UseCosmos(ContainerFixture.CosmosConnectionString!, dbName)
            .UseExpressives()
            .Options;

        return new CosmosContextHandle(new CosmosIntegrationTestDbContext(options));
    }

    public sealed class CosmosContextHandle(CosmosIntegrationTestDbContext context) : IAsyncDisposable
    {
        public CosmosIntegrationTestDbContext Context { get; } = context;
        public async ValueTask DisposeAsync()
        {
            try { await Context.Database.EnsureDeletedAsync(); }
            finally { await Context.DisposeAsync(); }
        }
    }
#endif
}
