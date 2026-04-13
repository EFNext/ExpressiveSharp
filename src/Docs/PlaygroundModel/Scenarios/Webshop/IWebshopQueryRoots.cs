using ExpressiveSharp;

namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

/// <summary>
/// Query context passed to sample snippets; each render target supplies its own
/// implementation (EF Core DbContext, MongoDB collections, in-memory arrays).
/// </summary>
public interface IWebshopQueryRoots
{
    IExpressiveQueryable<Customer> Customers { get; }
    IExpressiveQueryable<Order> Orders { get; }
    IExpressiveQueryable<Product> Products { get; }
    IExpressiveQueryable<LineItem> LineItems { get; }
}
