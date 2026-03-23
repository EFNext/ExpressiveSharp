using System.Linq.Expressions;
using ExpressiveSharp.FunctionalTests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.FunctionalTests.Infrastructure;

public sealed class EFCoreSqliteConsumer : IExpressionConsumer
{
    private readonly SqliteConnection _connection;
    private readonly TestDbContext _context;

    public EFCoreSqliteConsumer()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;

        _context = new TestDbContext(options);
        _context.Database.EnsureCreated();
    }

    public Task AssertWhereTranslatesAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        var query = _context.Set<TEntity>().Where(predicate);
        _ = query.ToQueryString();
        return Task.CompletedTask;
    }

    public Task AssertSelectTranslatesAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class
    {
        var query = _context.Set<TEntity>().Select(selector);
        _ = query.ToQueryString();
        return Task.CompletedTask;
    }

    public Task AssertOrderByTranslatesAsync<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
    {
        var query = _context.Set<TEntity>().OrderBy(keySelector);
        _ = query.ToQueryString();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
