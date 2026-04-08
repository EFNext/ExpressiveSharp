#if NET9_0_OR_GREATER
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class AggregateByTests : GeneratorTestBase
{
    [TestMethod]
    public Task AggregateBy_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } public decimal Price { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .AggregateBy(
                                 o => o.Tag,
                                 0m,
                                 (acc, o) => acc + o.Price)
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
    public Task AggregateBy_WithSeedSelector_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } public decimal Price { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .AggregateBy(
                                 o => o.Tag,
                                 k => 0m,
                                 (acc, o) => acc + o.Price)
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
#endif
