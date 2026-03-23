using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class PositionalPatternTests : GeneratorTestBase
{
    [TestMethod]
    public Task IsExpression_RecordPositionalPattern()
    {
        var compilation = CreateCompilation(
            """
            record Point(int X, int Y);

            class Foo {
                [Expressive]
                public bool IsOrigin(Point point) => point is Point(0, 0);
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SwitchExpression_RecordPositionalPattern()
    {
        var compilation = CreateCompilation(
            """
            record Point(int X, int Y);

            static class PointExtensions {
                [Expressive]
                public static string Describe(this Point point) => point switch {
                    Point(0, 0) => "origin",
                    Point(_, 0) => "x-axis",
                    Point(0, _) => "y-axis",
                    _ => "other",
                };
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task IsExpression_PositionalPatternWithRelationalSubPatterns()
    {
        var compilation = CreateCompilation(
            """
            record Point(int X, int Y);

            class Foo {
                [Expressive]
                public bool IsInFirstQuadrant(Point point) => point is Point(> 0, > 0);
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task IsExpression_RecordStructPositionalPattern()
    {
        var compilation = CreateCompilation(
            """
            record struct Point(int X, int Y);

            class Foo {
                [Expressive]
                public bool IsOrigin(Point point) => point is Point(0, 0);
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
