using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class IndexRangeTests : GeneratorTestBase
{
    [TestMethod]
    public Task IndexFromEnd_WorksAsExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }

                    [Expressive]
                    public int Last => Items[^1];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task Range_WorksAsExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Start { get; set; }
                    public int End { get; set; }

                    [Expressive]
                    public Range Slice => Start..End;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
