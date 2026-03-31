using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class ScalarMethodTests : GeneratorTestBase
{
    // ── Predicate methods (bool-returning) ───────────────────────────────

    [TestMethod]
    public Task Any_GeneratesScalarInterceptor()
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
                        var result = query.AsExpressive().Any(o => o.Tag == "urgent");
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task All_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().All(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    // ── Counting methods ─────────────────────────────────────────────────

    [TestMethod]
    public Task Count_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Count(o => o.Amount > 100);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task LongCount_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().LongCount(o => o.Amount > 100);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    // ── Element methods ──────────────────────────────────────────────────

    [TestMethod]
    public Task First_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().First(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task FirstOrDefault_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().FirstOrDefault(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Last_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Last(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task LastOrDefault_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().LastOrDefault(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Single_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Single(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task SingleOrDefault_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().SingleOrDefault(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    // ── Min / Max ────────────────────────────────────────────────────────

    [TestMethod]
    public Task Min_GenericSelector_GeneratesScalarInterceptor()
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
                        var result = query.AsExpressive().Min(o => o.Name);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Max_GenericSelector_GeneratesScalarInterceptor()
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
                        var result = query.AsExpressive().Max(o => o.Name);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task MinBy_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().MinBy(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task MaxBy_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().MaxBy(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    // ── Sum ──────────────────────────────────────────────────────────────

    [TestMethod]
    public Task Sum_IntSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_NullableIntSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_LongSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public long Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_NullableLongSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public long? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_FloatSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public float Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_NullableFloatSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public float? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_DoubleSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public double Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_NullableDoubleSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public double? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_DecimalSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public decimal Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Sum_NullableDecimalSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public decimal? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Sum(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    // ── Average ──────────────────────────────────────────────────────────

    [TestMethod]
    public Task Average_IntSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_NullableIntSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_LongSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public long Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_NullableLongSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public long? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_FloatSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public float Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_NullableFloatSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public float? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_DoubleSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public double Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_NullableDoubleSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public double? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_DecimalSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public decimal Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task Average_NullableDecimalSelector_GeneratesScalarInterceptor()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public decimal? Amount { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Average(o => o.Amount);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
