using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class SpanMethodTests : GeneratorTestBase
{
    [TestMethod]
    public Task ArrayContains_EmitsEnumerableContains()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    private static readonly int[] Featured = [1, 3];
                    public int Id { get; set; }

                    [Expressive]
                    public bool IsFeatured => Featured.Contains(Id);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ArrayContains_EnumElement_EmitsEnumerableContains()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                enum Status { Draft, Active, Done }

                class C {
                    private static readonly Status[] Open = [Status.Draft, Status.Active];
                    public Status Status { get; set; }

                    [Expressive]
                    public bool IsOpen => Open.Contains(Status);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ArrayContains_ExplicitComparer_KeepsComparer()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    private static readonly string[] Names = ["a", "b"];
                    public string Name { get; set; } = "";

                    [Expressive]
                    public bool IsKnown => Names.Contains(Name, StringComparer.OrdinalIgnoreCase);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ArraySequenceEqual_EmitsEnumerableSequenceEqual()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    private static readonly int[] Expected = [1, 2, 3];
                    public int[] Values { get; set; } = [];

                    [Expressive]
                    public bool Matches => Values.SequenceEqual(Expected);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
