using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class NullableTests : GeneratorTestBase
{
    [TestMethod]
    public Task NullableReferenceTypesAreBeingEliminated()
    {
        var compilation = CreateCompilation(
            """
            #nullable enable

            namespace Foo {
                static class C {
                    [Expressive]
                    public static object? NextFoo(this object? unusedArgument, int? nullablePrimitiveArgument) => null;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task GenericNullableReferenceTypesAreBeingEliminated()
    {
        var compilation = CreateCompilation(
            """
            #nullable enable

            namespace Foo {
                static class C {
                    [Expressive]
                    public static List<object?> NextFoo(this List<object?> input, List<int?> nullablePrimitiveArgument) => input;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableReferenceTypeCastOperatorGetsEliminated()
    {
        var compilation = CreateCompilation(
            """
            #nullable enable

            namespace Foo {
                static class C {
                    [Expressive]
                    public static string? NullableReferenceType(object? input) => (string?)input;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableValueCastOperatorsPersist()
    {
        var compilation = CreateCompilation(
            """
            #nullable enable

            namespace Foo {
                static class C {
                    [Expressive]
                    public static int? NullableValueType(object? input) => (int?)input;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableMemberBinding_WithDisableFlag_StillGenerates()
    {
        // The Disable=NullConditional flag no longer prevents generation.
        // The emitter always produces the faithful null-check ternary.
        // Consumers use RemoveNullConditionalPatterns transformer at runtime.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static int? GetLength(this string input) => input?.Length;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public void NullableMemberBinding_DefaultMode_UsesRewrite()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static int? GetLength(this string input) => input?.Length;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public void MultiLevelNullableMemberBinding_DefaultMode_UsesRewrite()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                    public record Address
                    {
                        public int Id { get; set; }
                        public string? Country { get; set; }
                    }

                    public record Party
                    {
                        public int Id { get; set; }

                        public Address? Address { get; set; }
                    }

                    public record Entity
                    {
                        public int Id { get; set; }

                        public Party? Left { get; set; }
                        public Party? Right { get; set; }

                        [Expressive]
                        public bool IsSameCountry => Left?.Address?.Country == Right?.Address?.Country;
                    }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public Task NullableMemberBinding_WithIgnoreSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static int? GetLength(this string input) => input?.Length;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableMemberBinding_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static int? GetLength(this string input) => input?.Length;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableSimpleElementBinding_WithIgnoreSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static char? GetFirst(this string input) => input?[0];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableSimpleElementBinding_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static char? GetFirst(this string input) => input?[0];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BooleanSimpleTernary_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static bool Test(this object? x) => x?.Equals(4) == false;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableElementBinding_WithIgnoreSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static string? GetFirst(this string input) => input?[0].ToString();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableElementBinding_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static string? GetFirst(this string input) => input?[0].ToString();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableElementAndMemberBinding_WithIgnoreSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public static class EntityExtensions
                {
                    public record Entity
                    {
                        public int Id { get; set; }
                        public List<Entity>? RelatedEntities { get; set; }
                    }

                    [Expressive]
                    public static Entity GetFirstRelatedIgnoreNulls(this Entity entity)
                        => entity?.RelatedEntities?[0];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableElementAndMemberBinding_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public static class EntityExtensions
                {
                    public record Entity
                    {
                        public int Id { get; set; }
                        public List<Entity>? RelatedEntities { get; set; }
                    }

                    [Expressive]
                    public static Entity GetFirstRelatedIgnoreNulls(this Entity entity)
                        => entity?.RelatedEntities?[0];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableParameters_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public static class EntityExtensions
                {
                    public record Entity
                    {
                        public int Id { get; set; }
                        public string? FullName { get; set; }
                    }

                    [Expressive]
                    public static string GetFirstName(this Entity entity)
                        => entity.FullName?.Substring(entity.FullName?.IndexOf(' ') ?? 0);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullConditionalNullCoalesceTypeConversion()
    {
        var compilation = CreateCompilation(
            """
            class Foo {
                public int? FancyNumber { get; set; }

                [Expressive]
                public static int SomeNumber(Foo fancyClass) => fancyClass?.FancyNumber ?? 3;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableValueType_MemberAccess_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public struct Point {
                    public double X { get; set; }
                    public double Y { get; set; }
                }

                static class C {
                    [Expressive]
                    public static double? GetX(this Point? point) => point?.X;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableValueType_MemberAccess_WithIgnoreSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public struct Point {
                    public double X { get; set; }
                    public double Y { get; set; }
                }

                static class C {
                    [Expressive]
                    public static double? GetX(this Point? point) => point?.X;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NullableValueType_MemberAccessWithCoalesce_WithRewriteSupport_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public struct Point {
                    public double X { get; set; }
                    public double Y { get; set; }
                }

                static class C {
                    [Expressive]
                    public static double GetX(this Point? point) => point?.X ?? 0.0;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
