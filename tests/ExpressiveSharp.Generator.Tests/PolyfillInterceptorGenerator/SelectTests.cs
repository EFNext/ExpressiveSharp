using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class SelectTests : GeneratorTestBase
{
    [TestMethod]
    public Task Select_AnonymousType_GeneratesGenericInterceptor()
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
                        query.WithExpressionRewrite().Select(o => new { o.Id, o.Name }).ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for anonymous type projection");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Select_AnonymousType_WithComputedExpression_GeneratesCorrectly()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Id { get; set; } public decimal Price { get; set; } public int Qty { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite().Select(o => new { o.Id, Total = o.Price * o.Qty }).ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.AreEqual(0, result.Diagnostics.Length, "No diagnostics expected for anonymous type projection");

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Select_NamedType_GeneratesConcreteInterceptor()
    {
        var source = 
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite().Select(o => o.Name).ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
