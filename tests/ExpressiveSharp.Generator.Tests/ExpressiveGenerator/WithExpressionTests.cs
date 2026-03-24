using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class WithExpressionTests : GeneratorTestBase
{
    [TestMethod]
    public Task WithExpression_OnRecord()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                record Point(int X, int Y);

                class C {
                    public Point P { get; set; }

                    [Expressive]
                    public Point Shifted => P with { X = P.X + 1 };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
