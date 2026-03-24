using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore;

public sealed class EFCoreSqliteTestRunner : IIntegrationTestRunner
{
    private readonly SqliteConnection _connection;
    private readonly IntegrationTestDbContext _context;
    private readonly Action<string>? _logSql;
    private bool _loggingEnabled;

    public EFCoreSqliteTestRunner(Action<string>? logSql = null)
    {
        _logSql = logSql;
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var builder = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseSqlite(_connection)
            .UseExpressives();

        if (logSql is not null)
        {
            builder
                .LogTo(message => { if (_loggingEnabled) logSql(message); },
                    new[] { DbLoggerCategory.Database.Command.Name },
                    Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        var options = builder.Options;

        _context = new IntegrationTestDbContext(options);
        _context.Database.EnsureCreated();
    }

    public async Task SeedAsync(
        IReadOnlyList<Address> addresses,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Order> orders,
        IReadOnlyList<LineItem> lineItems)
    {
        _context.Set<Address>().AddRange(addresses);
        await _context.SaveChangesAsync();

        // Add customers without navigation to avoid tracking conflicts
        foreach (var c in customers)
        {
            _context.Customers.Add(new Customer
            {
                Id = c.Id, Name = c.Name, Email = c.Email, AddressId = c.AddressId,
            });
        }
        await _context.SaveChangesAsync();

        // Add orders without navigation to avoid tracking conflicts
        foreach (var order in orders)
        {
            _context.Orders.Add(new Order
            {
                Id = order.Id, Tag = order.Tag, Price = order.Price,
                Quantity = order.Quantity, Status = order.Status, CustomerId = order.CustomerId,
            });
        }
        await _context.SaveChangesAsync();

        _context.Set<LineItem>().AddRange(lineItems);
        await _context.SaveChangesAsync();

        _loggingEnabled = true;
    }

    public async Task<List<TEntity>> WhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
        => await _context.Set<TEntity>().Where(predicate).ToListAsync();

    public async Task<List<TResult>> SelectAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class
        => await _context.Set<TEntity>().Select(selector).ToListAsync();

    public async Task<List<TResult>> OrderBySelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
        => await _context.Set<TEntity>().OrderBy(keySelector).Select(resultSelector).ToListAsync();

    public async Task<List<TResult>> OrderByDescendingSelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
        => await _context.Set<TEntity>().OrderByDescending(keySelector).Select(resultSelector).ToListAsync();

    public async Task<List<TResult>> OrderByThenBySelectAsync<TEntity, TKey1, TKey2, TResult>(
        Expression<Func<TEntity, TKey1>> primaryKeySelector,
        Expression<Func<TEntity, TKey2>> secondaryKeySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
        => await _context.Set<TEntity>().OrderBy(primaryKeySelector).ThenBy(secondaryKeySelector).Select(resultSelector).ToListAsync();

    public async Task<List<KeyValuePair<TKey, int>>> GroupByCountAsync<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
        => await _context.Set<TEntity>().GroupBy(keySelector)
            .Select(g => new KeyValuePair<TKey, int>(g.Key, g.Count()))
            .ToListAsync();

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
