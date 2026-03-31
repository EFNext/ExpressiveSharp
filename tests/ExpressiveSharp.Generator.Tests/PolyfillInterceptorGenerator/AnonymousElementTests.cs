using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

/// <summary>
/// Tests for interceptor generation when the element type T of IRewritableQueryable&lt;T&gt;
/// is an anonymous type (i.e., after Select into an anonymous type).
/// Covers the fix for GitHub issue #9.
/// </summary>
[TestClass]
public class AnonymousElementTests : GeneratorTestBase
{
    [TestMethod]
    public Task Where_AfterAnonymousSelect_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Id { get; set; } public decimal Total { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Select(o => new { o.Id, o.Total })
                             .Where(x => x.Total > 100m)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for anonymous element Where");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task OrderByDescending_AfterAnonymousSelect_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Id { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Select(o => new { o.Id, o.Name })
                             .OrderByDescending(x => x.Name)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for anonymous element OrderBy");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Select_AfterAnonymousSelect_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Id { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Select(o => new { o.Id, o.Name })
                             .Select(x => x.Id)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for Select from anonymous element");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task DistinctBy_AfterAnonymousSelect_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Id { get; set; } public string Category { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Select(o => new { o.Id, o.Category })
                             .DistinctBy(x => x.Category)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for DistinctBy on anonymous element");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task GroupBy_AfterAnonymousSelect_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Id { get; set; } public string Category { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Select(o => new { o.Id, o.Category })
                             .GroupBy(x => x.Category)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for GroupBy on anonymous element");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Count_AfterAnonymousSelect_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Id { get; set; } public decimal Total { get; set; } }
                class TestClass
                {
                    public int Run(System.Linq.IQueryable<Order> query)
                    {
                        return query.AsExpressive()
                                    .Select(o => new { o.Id, o.Total })
                                    .Count(x => x.Total > 0m);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for Count on anonymous element");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
