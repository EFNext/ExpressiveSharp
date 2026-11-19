using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class PolyfillTests : GeneratorTestBase
{
    [TestMethod]
    public Task Polyfill_SimpleLambda_GeneratesExpression()
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
                        Expression<Func<Order, bool>> expr = ExpressionPolyfill.Create<Func<Order, bool>>(
                            o => o.Tag == "urgent");
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Polyfill_InferredTypeArgument_GeneratesExpression()
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
                        var expr = ExpressionPolyfill.Create((Order o) => o.Tag == "urgent");
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Polyfill_CapturedVariable_GeneratesClosureAccess()
    {
        var source =
            """
            using System.Linq.Expressions;

            namespace TestNs
            {
                class Order { public double Price { get; set; } }
                class TestClass
                {
                    public void Run(double threshold)
                    {
                        var expr = ExpressionPolyfill.Create((Order o) => o.Price > threshold);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Polyfill_NullConditional_RewritesOperator()
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
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

#if NET10_0_OR_GREATER
    [TestMethod]
    public Task Polyfill_ExtensionProperty_CallsImplementationGetter()
    {
        var source =
            """
            using System.Linq.Expressions;

            namespace TestNs
            {
                class Order { public int Quantity { get; set; } }
                static class OrderExtensions
                {
                    extension(Order o)
                    {
                        public bool IsBig => o.Quantity > 100;
                    }
                }
                class TestClass
                {
                    public void Run()
                    {
                        var expr = ExpressionPolyfill.Create((Order o) => o.IsBig);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Polyfill_ExtensionMethod_CallsImplementationMethod()
    {
        var source =
            """
            using System.Linq.Expressions;

            namespace TestNs
            {
                class Order { public int Quantity { get; set; } }
                static class OrderExtensions
                {
                    extension(Order o)
                    {
                        public int Scaled(int factor) => o.Quantity * factor;
                    }

                    extension(Order)
                    {
                        public static int Threshold(int value) => value * 2;
                    }
                }
                class TestClass
                {
                    public void Run()
                    {
                        var expr = ExpressionPolyfill.Create((Order o) => o.Scaled(3) > Order.Threshold(50));
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
#endif
}
