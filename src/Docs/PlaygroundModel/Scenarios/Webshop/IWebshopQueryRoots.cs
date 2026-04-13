using ExpressiveSharp;

namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

/// <summary>
/// Multi-root query context that sample snippets receive as their sole argument.
/// Each render target (EF Core SQLite/Postgres/SqlServer/Cosmos, MongoDB, in-memory
/// playground) supplies its own implementation wrapping its underlying queryables.
/// Snippets read as <c>db.Customers.Where(...)</c>, <c>db.Orders.SelectMany(...)</c>,
/// etc. — naturally rooted at whichever entity set the example needs.
/// </summary>
public interface IWebshopQueryRoots
{
    IExpressiveQueryable<Customer> Customers { get; }
    IExpressiveQueryable<Order> Orders { get; }
    IExpressiveQueryable<Product> Products { get; }
    IExpressiveQueryable<LineItem> LineItems { get; }
}
