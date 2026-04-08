using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class SetOperationTests : GeneratorTestBase
{
    [TestMethod]
    public Task ExceptBy_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query, System.Collections.Generic.IEnumerable<string> excluded)
                    {
                        query.AsExpressive()
                             .ExceptBy(excluded, o => o.Tag)
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
    public Task IntersectBy_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query, System.Collections.Generic.IEnumerable<string> included)
                    {
                        query.AsExpressive()
                             .IntersectBy(included, o => o.Tag)
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
    public Task UnionBy_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query, System.Collections.Generic.IEnumerable<string> extra)
                    {
                        query.AsExpressive()
                             .UnionBy(extra, o => o.Tag)
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
