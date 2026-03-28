using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class ComparerOverloadTests : GeneratorTestBase
{
    [TestMethod]
    public Task OrderBy_WithComparer_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite()
                             .OrderBy(o => o.Tag, System.StringComparer.OrdinalIgnoreCase)
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
    public Task GroupBy_WithComparer_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite()
                             .GroupBy(o => o.Tag, System.StringComparer.OrdinalIgnoreCase)
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
    public Task DistinctBy_WithComparer_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite()
                             .DistinctBy(o => o.Tag, System.StringComparer.OrdinalIgnoreCase)
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
