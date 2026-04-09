using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class GlobalOptionsTests : GeneratorTestBase
{
    [TestMethod]
    public Task Polyfill_GlobalNullConditionalIgnore_UsesIgnoreMode()
    {
        var source = 
            """
            using System.Linq.Expressions;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run()
                    {
                        Expression<Func<Order, int?>> expr = ExpressionPolyfill.Create<Func<Order, int?>>(
                            o => o.Tag?.Length);
                    }
                }
            }
            """;
        var globalProperties = new Dictionary<string, string>
        {
            
        };
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source), globalProperties);

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Where_GlobalNullConditionalRewrite_AppliesWithoutPerCallOverride()
    {
        var source = 
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Customer { public string Name { get; set; } }
                class Order { public Customer Customer { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Where(o => o.Customer?.Name == "Alice")
                             .ToList();
                    }
                }
            }
            """;
        var globalProperties = new Dictionary<string, string>
        {
            
        };
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source), globalProperties);

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Where_PerCallOverridesGlobal()
    {
        // Global says Ignore, but per-call AsExpressive says Rewrite — per-call wins.
        var source = 
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Customer { public string Name { get; set; } }
                class Order { public Customer Customer { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive(new ExpressionRewriteOptions())
                             .Where(o => o.Customer?.Name == "Alice")
                             .ToList();
                    }
                }
            }
            """;
        var globalProperties = new Dictionary<string, string>
        {
            
        };
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source), globalProperties);

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
