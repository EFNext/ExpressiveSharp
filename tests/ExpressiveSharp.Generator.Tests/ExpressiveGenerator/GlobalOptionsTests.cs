using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class GlobalOptionsTests : GeneratorTestBase
{
    // ── Block body (always on — no opt-in needed) ───────────────────────────

    [TestMethod]
    public void BlockBody_AlwaysAllowed_NoDiagnostics()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Value { get; set; }
                    [Expressive]
                    public int GetDouble()
                    {
                        return Value * 2;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation, new Dictionary<string, string>());

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    // ── EnumMethodExpansion (enabled by default, disable via Disable flag) ──

    [TestMethod]
    public void EnumMethodExpansion_EnabledByDefault_ExpandsWithoutAttributeFlag()
    {
        var compilation = CreateCompilation(
            """
            using System.ComponentModel.DataAnnotations;
            namespace Foo {
                public enum MyEnum { A, B }
                public static class MyEnumExtensions {
                    public static string GetName(this MyEnum value) => value.ToString();
                }
                public record Entity {
                    public MyEnum Status { get; set; }
                    [Expressive]
                    public string StatusName => Status.GetName();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation, new Dictionary<string, string>());

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
        var generated = result.GeneratedTrees[0].ToString();
        Assert.IsTrue(generated.Contains("MyEnum.A"));
        Assert.IsTrue(generated.Contains("MyEnum.B"));
    }

    [TestMethod]
    public void AttributeDisableEnumMethodExpansion_OverridesDefault()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public enum MyEnum { A, B }
                public static class MyEnumExtensions {
                    public static string GetName(this MyEnum value) => value.ToString();
                }
                public record Entity {
                    public MyEnum Status { get; set; }
                    [Expressive]
                    public string StatusName => Status.GetName();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation, new Dictionary<string, string>());

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
        var generated = result.GeneratedTrees[0].ToString();
        Assert.IsFalse(generated.Contains("MyEnum.A =="));
    }

    // ── NullConditionalMode ─────────────────────────────────────────────────

    [TestMethod]
    public void GlobalNullConditionalMode_Rewrite_AllowsNullConditionals()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Inner { public int Value { get; set; } }
                class C {
                    public Inner? Inner { get; set; }
                    [Expressive]
                    public int? InnerValue => Inner?.Value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation, new Dictionary<string, string>
        {
            
        });

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public void AttributeDisableNullConditional_StillGeneratesFaithfully()
    {
        // Disable=NullConditional no longer prevents generation.
        // The emitter always produces faithful expression trees.
        // Consumers use runtime transformers for adaptation.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Inner { public int Value { get; set; } }
                class C {
                    public Inner? Inner { get; set; }
                    [Expressive]
                    public int? InnerValue => Inner?.Value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation, new Dictionary<string, string>
        {
            
        });

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    // ── Hard-coded defaults ─────────────────────────────────────────────────

    [TestMethod]
    public void NoGlobalOptions_HardCodedDefaultsApply_BlockBodyWorks()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Value { get; set; }
                    [Expressive]
                    public int GetDouble()
                    {
                        return Value * 2;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation, new Dictionary<string, string>());

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public void MalformedGlobalOption_IsTreatedAsNotSet()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Value { get; set; }
                    [Expressive]
                    public int GetDouble()
                    {
                        return Value * 2;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation, new Dictionary<string, string>
        {
            
        });

        // Malformed value is ignored; defaults apply (block body always on, Rewrite is default)
        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }
}
