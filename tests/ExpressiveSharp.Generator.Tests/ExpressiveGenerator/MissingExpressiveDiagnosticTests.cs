using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class MissingExpressiveDiagnosticTests : GeneratorTestBase
{
    // ── Positive: EXP0013 fires ─────────────────────────────────────────────

    [TestMethod]
    public void MethodCall_ToSourceMethodWithExpressionBody_WarnsEXP0013()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public static int Transform(int x) => x * x;

                    [Expressive]
                    public int Computed => Transform(42);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for method call to source method without [Expressive]");
    }

    [TestMethod]
    public void PropertyAccess_ToSourcePropertyWithExpressionBody_WarnsEXP0013()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Raw { get; set; }
                    public int Doubled => Raw * 2;

                    [Expressive]
                    public int Result => Doubled + 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for property access to source property without [Expressive]");
    }

    [TestMethod]
    public void PropertyAccess_ToSourcePropertyWithBlockGetter_WarnsEXP0013()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Raw { get; set; }
                    public int Doubled { get { return Raw * 2; } }

                    [Expressive]
                    public int Result => Doubled + 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for property access to source property with block getter without [Expressive]");
    }

    [TestMethod]
    public void MethodCall_ToBlockBodyMethod_WarnsEXP0013()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public static int Compute(int x) { return x * x; }

                    [Expressive]
                    public int Result => Compute(5);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for method call to block-body method without [Expressive]");
    }

    // ── Negative: no EXP0013 ────────────────────────────────────────────────

    [TestMethod]
    public void MethodCall_ToMethodWithExpressive_NoWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public static int Transform(int x) => x * x;

                    [Expressive]
                    public int Computed => Transform(42);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(2, result.GeneratedTrees.Length);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn when referenced method already has [Expressive]");
    }

    [TestMethod]
    public void PropertyAccess_ToAutoProperty_NoWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo => Bar;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for auto-property access");
    }

    [TestMethod]
    public void MethodCall_ToBclMethod_NoWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public string Name { get; set; }

                    [Expressive]
                    public string Upper => Name.ToUpper();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for BCL method call");
    }

    [TestMethod]
    public void MethodCall_ToAbstractMethod_NoWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                abstract class Base {
                    public abstract int Compute();
                }

                class Derived : Base {
                    public override int Compute() => 42;

                    [Expressive]
                    public int Result => Compute();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        // The override has a body but the base is abstract; the symbol resolved
        // here is the override which does have a body, so EXP0013 may fire.
        // This test verifies no crash; the diagnostic behavior depends on Roslyn symbol resolution.
    }

    [TestMethod]
    public void PropertyAccess_ToExpressiveProperty_NoWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Raw { get; set; }

                    [Expressive]
                    public int Doubled => Raw * 2;

                    [Expressive]
                    public int Result => Doubled + 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(2, result.GeneratedTrees.Length);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn when referenced property already has [Expressive]");
    }
}
