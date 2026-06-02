using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

/// <summary>
/// Tests for <c>[ExpressiveProperty]</c> — synthesizes a settable property on the stub's
/// containing partial type. Two shapes: coalesce (non-nullable targets) and ternary+flag
/// (nullable targets, including both ref-nullable and value-nullable).
/// </summary>
[TestClass]
public class ExpressivePropertyTests : GeneratorTestBase
{
    [TestMethod]
    public Task ReferenceTypeTarget_EmitsCoalesceForm()
    {
        // Non-nullable reference target — coalesce shape with a nullable backing field.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [ExpressiveProperty("FullName")]
                    private string FullNameExpression => LastName + ", " + FirstName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public Task NullableReferenceTypeTarget_EmitsTernaryForm()
    {
        // Nullable reference target — ternary+flag shape (coalesce would collide with stored null).
        var compilation = CreateCompilation(
            """
            #nullable enable
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    public string? FirstName { get; set; }
                    public string? LastName  { get; set; }

                    [ExpressiveProperty("FullName")]
                    private string? FullNameExpression =>
                        LastName is null || FirstName is null ? null : (LastName + ", " + FirstName);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public Task NonNullableValueTypeTarget_EmitsCoalesceForm()
    {
        // Non-nullable value target — coalesce shape with Nullable<T> backing field.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    public decimal TotalAmount { get; set; }
                    public decimal Discount    { get; set; }

                    [ExpressiveProperty("Amount")]
                    private decimal AmountExpression => TotalAmount - Discount;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public Task NullableValueTypeTarget_EmitsTernaryForm()
    {
        // Nullable value target — ternary+flag (issue #35 scenario).
        var compilation = CreateCompilation(
            """
            #nullable enable
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; set; }
                    public decimal? Discount    { get; set; }

                    [ExpressiveProperty("Amount")]
                    private decimal? AmountExpression =>
                        TotalAmount != null && Discount != null
                            ? TotalAmount.Value - Discount.Value
                            : (decimal?)null;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public Task PartialRecord_EmitsCorrectly()
    {
        // Target is a partial record — the synthesized file must emit `partial record`.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial record Person {
                    public string FirstName { get; init; } = "";
                    public string LastName  { get; init; } = "";

                    [ExpressiveProperty("FullName")]
                    private string FullNameExpression => LastName + ", " + FirstName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public Task PartialStruct_EmitsCorrectly()
    {
        // Target is a partial struct — the synthesized file must emit `partial struct`.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial struct Point {
                    public double X { get; set; }
                    public double Y { get; set; }

                    [ExpressiveProperty("Magnitude")]
                    private double MagnitudeExpression => System.Math.Sqrt(X * X + Y * Y);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public Task BackingFieldNameCollision_AppendsSuffix()
    {
        // User type already declares `_fullName` — synthesized backing field must avoid the
        // collision (verified by a string contains assertion rather than full snapshot since
        // only the field name choice is the contract being tested).
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    private string? _fullName;
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [ExpressiveProperty("FullName")]
                    private string FullNameExpression => LastName + ", " + FirstName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        var generated = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));
        StringAssert.Contains(generated, "private string? _fullName2;",
            "Backing field should be renamed to avoid collision with the user-declared _fullName field.");
        Assert.IsFalse(generated.Contains("private string? _fullName;\n        public string FullName"),
            "Synthesized backing field must not be the colliding name `_fullName`.");
        return Task.CompletedTask;
    }

    [TestMethod]
    public Task NestedInsideNonClassContainer_EmitsCorrectOuterKeyword()
    {
        // Partial class nested inside a partial struct — outer wrapper must be `partial struct`,
        // not the previous hard-coded `partial class` (which produced uncompilable output).
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public partial struct Outer {
                    public partial class Inner {
                        public string FirstName { get; set; } = "";

                        [ExpressiveProperty("Name")]
                        private string NameExpression => FirstName;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public void TargetAlreadyExists_ReportsEXP0018()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    public string FullName { get; set; } = "";

                    [ExpressiveProperty("FullName")]
                    private string FullNameExpression => "x";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0018"));
    }

    [TestMethod]
    public void NonPartialContainer_ReportsEXP0019()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class Account {
                    public string FirstName { get; set; } = "";

                    [ExpressiveProperty("FullName")]
                    private string FullNameExpression => FirstName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0019"));
    }

    [TestMethod]
    public void AccessorListFormRejected_EXP0020()
    {
        // Accessor-list form (`{ get => expr; }`) is rejected in favor of top-level `=> expr`.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    public string FirstName { get; set; } = "";

                    [ExpressiveProperty("FullName")]
                    private string FullNameExpression { get { return FirstName; } }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0020"));
    }

    [TestMethod]
    public void StaticStubRejected_EXP0021()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                partial class Account {
                    public static string Theme = "dark";

                    [ExpressiveProperty("EffectiveTheme")]
                    private static string EffectiveThemeExpression => Theme;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0021"));
    }

    [TestMethod]
    public void SynthesizedPropertyReferencingAnotherSynthesizedProperty_EmitsCorrectly()
    {
        // Issue #44: when one [ExpressiveProperty] body references another's synthesized target,
        // ExpressiveGenerator must augment its in-memory compilation with the sibling's
        // synthesized partial so the SemanticModel can bind the cross-reference.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public partial class Person {
                    [ExpressiveProperty("FirstName")]
                    private string FirstNameExpr => "Jane";

                    [ExpressiveProperty("FullName")]
                    private string FullNameExpr => FirstName + " Doe";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        // No EXP0008 (unsupported operation) and no compilation errors in the generated code.
        Assert.AreEqual(0, result.Diagnostics.Length,
            "Unexpected diagnostics: " + string.Join(", ", result.Diagnostics.Select(d => d.Id + ": " + d.GetMessage())));

        // Two stubs → 2 expression-tree files + 2 synthesized partial files = 4 generated trees.
        Assert.AreEqual(4, result.GeneratedTrees.Length);

        // The FullName expression tree must reference FirstName as a property access (not a
        // Default fallback that would indicate the binder couldn't resolve the symbol).
        var fullNameTree = result.GeneratedTrees.Single(t => t.FilePath.Contains("FullName") && !t.FilePath.Contains("Synthesized"));
        var fullNameSource = fullNameTree.ToString();
        StringAssert.Contains(fullNameSource, "FirstName");
        Assert.IsFalse(fullNameSource.Contains("Expression.Default"),
            "FullName body fell back to Default — synthesized property binding failed:\n" + fullNameSource);
    }

    [TestMethod]
    public void ExpressiveBodyReferencingSynthesizedProperty_EmitsCorrectly()
    {
        // Same fix benefits [Expressive] members that reference a sibling synthesized property.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp;
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public partial class Person {
                    [ExpressiveProperty("FirstName")]
                    private string FirstNameExpr => "Jane";

                    [Expressive]
                    public string Greeting() => "Hello, " + FirstName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            "Unexpected diagnostics: " + string.Join(", ", result.Diagnostics.Select(d => d.Id + ": " + d.GetMessage())));

        var greetingTree = result.GeneratedTrees.Single(t => t.FilePath.Contains("Greeting"));
        var greetingSource = greetingTree.ToString();
        StringAssert.Contains(greetingSource, "FirstName");
        Assert.IsFalse(greetingSource.Contains("Expression.Default"),
            "Greeting body fell back to Default — synthesized property binding failed:\n" + greetingSource);
    }

    [TestMethod]
    public void ShadowsInheritedMember_ReportsEXP0022()
    {
        // Target name already exists on the base type — silently hiding it would be a footgun.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class Base {
                    public string Name { get; set; } = "";
                }

                partial class Derived : Base {
                    public string Prefix { get; set; } = "";

                    [ExpressiveProperty("Name")]
                    private string NameExpression => Prefix + "/";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0022"));
    }

    [TestMethod]
    public Task GenericReturnTypeWithInnerNullableReference_EmitsCorrectBackingFieldAnnotation()
    {
        // Outer non-nullable, inner ref-nullable — backing field must keep the inner ? to match the property.
        var compilation = CreateCompilation(
            """
            #nullable enable
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public partial class BugA {
                    [ExpressiveProperty("Items")]
                    private IEnumerable<Item?> ItemsExpr => new Item?[] { null };
                }
                public class Item;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            "Unexpected diagnostics: " + string.Join(", ", result.Diagnostics.Select(d => d.Id + ": " + d.GetMessage())));
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        var generated = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));
        StringAssert.Contains(generated, "IEnumerable<global::Foo.Item?>? _items",
            "Backing field must preserve the inner ? on Item, otherwise the synthesized ?? trips CS8619.");

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public void GenericMethodWithClosedInnerTypeArg_PinsByExactEquality()
    {
        // Inner type arg is concrete (List<int>), exercising the else branch in EnsureMethodInfo.
        var compilation = CreateCompilation(
            """
            #nullable enable
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public static class ListExt {
                    public static T Pick<T>(this IEnumerable<T> src, List<int> filter) => default!;
                }
                public partial class ClosedInnerArg {
                    public List<int> Filter { get; init; } = null!;

                    [ExpressiveProperty("Head")]
                    private string HeadExpr => new[] { "x" }.Pick(Filter);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            "Unexpected diagnostics: " + string.Join(", ", result.Diagnostics.Select(d => d.Id + ": " + d.GetMessage())));

        var generated = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));
        StringAssert.Contains(generated, "GetGenericArguments()[0] == typeof(int)",
            "Closed inner type arg must be pinned by exact equality.");
    }

    [TestMethod]
    public void GenericMethodWithArrayOfTypeParam_GeneratesValidCode()
    {
        // Inner type arg is T[] (IArrayTypeSymbol), exercising the array recursion in ContainsTypeParameter.
        var compilation = CreateCompilation(
            """
            #nullable enable
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public static class ArrExt {
                    public static int Total<T>(this IEnumerable<T[]> source) => 0;
                }
                public partial class ArrayOfTypeParam {
                    public IEnumerable<int[]> Source { get; init; } = null!;

                    [ExpressiveProperty("Sum")]
                    private int SumExpr => Source.Total();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            "Unexpected diagnostics: " + string.Join(", ", result.Diagnostics.Select(d => d.Id + ": " + d.GetMessage())));

        var generated = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));
        Assert.IsFalse(generated.Contains("typeof(T)") || generated.Contains("typeof(T["),
            "Array-of-type-parameter slot must not leak the open type parameter into typeof().");
    }

    [TestMethod]
    public Task BodyUsingQueryableWhereOrderBy_EmitsValidExpressionTree()
    {
        // Queryable.Where's predicate-shaped parameter must not leak the open type parameter (TSource → CS0246).
        var compilation = CreateCompilation(
            """
            #nullable enable
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public partial class BugB {
                    public IQueryable<int> Source { get; init; } = null!;

                    [ExpressiveProperty("Sorted")]
                    private IEnumerable<int> SortedExpr => Source.Where(x => x > 0).OrderBy(x => x);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            "Unexpected diagnostics: " + string.Join(", ", result.Diagnostics.Select(d => d.Id + ": " + d.GetMessage())));
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        var generated = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));
        Assert.IsFalse(generated.Contains("TSource"),
            "Generated code must not reference the open type parameter TSource.");

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public Task BodyUsingQueryableLambdaChain_EmitsExpressionQuoteNotConvert()
    {
        // Issue #50: lambda → Expression<TDelegate> must emit Expression.Quote.
        var compilation = CreateCompilation(
            """
            #nullable enable
            using ExpressiveSharp.Mapping;

            namespace Foo {
                public partial class Row {
                    public int Id { get; init; }
                    public int GroupId { get; init; }
                    public DateTime CreatedAt { get; init; }

                    [ExpressiveProperty("Previous")]
                    private Row? PreviousExpr =>
                        QueryContext.Query<Row>()
                            .Where(f => f.GroupId == GroupId && f.CreatedAt < CreatedAt)
                            .OrderByDescending(f => f.CreatedAt)
                            .FirstOrDefault();
                }

                internal static class QueryContext {
                    public static IQueryable<T> Query<T>() => Array.Empty<T>().AsQueryable();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            "Unexpected diagnostics: " + string.Join(", ", result.Diagnostics.Select(d => d.Id + ": " + d.GetMessage())));

        var generated = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));
        Assert.IsFalse(
            generated.Contains("Expression.Convert(") && generated.Contains("typeof(global::System.Linq.Expressions.Expression<"),
            "Lambda → Expression<TDelegate> must emit Expression.Quote, not Expression.Convert.");

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }
}
