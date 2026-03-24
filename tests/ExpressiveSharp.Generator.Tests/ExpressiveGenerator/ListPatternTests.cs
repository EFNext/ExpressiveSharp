using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class ListPatternTests : GeneratorTestBase
{
    [TestMethod]
    public Task ListPattern_FixedLength()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public List<int> Items { get; set; }

                    [Expressive]
                    public bool IsOneTwoThree => Items is [1, 2, 3];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ListPattern_WithSlice()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public List<int> Items { get; set; }

                    [Expressive]
                    public bool StartsWithOne => Items is [1, ..];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
