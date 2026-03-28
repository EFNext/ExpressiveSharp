using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class ZipTests : GeneratorTestBase
{
    [TestMethod]
    public Task Zip_WithResultSelector_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public string Name { get; set; } }
                class Price { public decimal Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> orders, System.Collections.Generic.IEnumerable<Price> prices)
                    {
                        orders.WithExpressionRewrite()
                              .Zip(prices, (o, p) => o.Name)
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
