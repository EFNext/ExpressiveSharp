using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class JoinTests : GeneratorTestBase
{
    [TestMethod]
    public Task Join_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int CustomerId { get; set; } public string Product { get; set; } }
                class Customer { public int Id { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> orders, System.Collections.Generic.IEnumerable<Customer> customers)
                    {
                        orders.AsExpressive()
                              .Join(customers,
                                    o => o.CustomerId,
                                    c => c.Id,
                                    (o, c) => o.Product + " - " + c.Name)
                              .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Join_AnonymousResultSelector_GeneratesGenericInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int CustomerId { get; set; } public string Product { get; set; } }
                class Customer { public int Id { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> orders, System.Collections.Generic.IEnumerable<Customer> customers)
                    {
                        orders.AsExpressive()
                              .Join(customers,
                                    o => o.CustomerId,
                                    c => c.Id,
                                    (o, c) => new { o.Product, CustomerName = c.Name })
                              .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for anonymous result in Join");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task GroupJoin_AnonymousResultSelector_GeneratesGenericInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int CustomerId { get; set; } }
                class Customer { public int Id { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> orders, System.Collections.Generic.IEnumerable<Customer> customers)
                    {
                        orders.AsExpressive()
                              .GroupJoin(customers,
                                         o => o.CustomerId,
                                         c => c.Id,
                                         (o, cs) => new { o.CustomerId, Count = cs.Count() })
                              .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for anonymous result in GroupJoin");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task GroupJoin_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int CustomerId { get; set; } }
                class Customer { public int Id { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> orders, System.Collections.Generic.IEnumerable<Customer> customers)
                    {
                        orders.AsExpressive()
                              .GroupJoin(customers,
                                         o => o.CustomerId,
                                         c => c.Id,
                                         (o, cs) => o.CustomerId)
                              .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
