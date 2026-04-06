using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class DiagnosticTests : GeneratorTestBase
{
    // ── EXP0001: RequiresBodyDefinition ─────────────────────────────────────

    [TestMethod]
    public void AutoProperty_NoBody_ReportsEXP0001()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Value { get; set; }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0001"),
            "Expected EXP0001 for auto-property without body");
    }

    [TestMethod]
    public void AbstractMethod_NoBody_ReportsEXP0001()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                abstract class C {
                    [Expressive]
                    public abstract int Compute(int x);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0001"),
            "Expected EXP0001 for abstract method without body");
    }

    // ── EXP0003: NoSourceAvailableForDelegatedConstructor ───────────────────

    [TestMethod]
    public void DelegatedConstructor_BaseInBCL_ReportsEXP0003()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class MyException : System.Exception {
                    public int Code { get; set; }

                    public MyException() { }

                    [Expressive]
                    public MyException(int code) : base("error")
                    {
                        Code = code;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0003"),
            "Expected EXP0003 for delegated constructor with no source available");
    }

    // ── EXP0007: UnsupportedInitializer ─────────────────────────────────────

    [TestMethod]
    public void NestedObjectInitializer_ReportsEXP0007()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Inner {
                    public int Value { get; set; }
                }

                class Outer {
                    public int Id { get; set; }
                    public Inner Child { get; set; } = new Inner();
                }

                class C {
                    [Expressive]
                    public Outer Create() => new Outer { Id = 1, Child = { Value = 42 } };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0007"),
            "Expected EXP0007 for nested member initializer");
    }

    // ── EXP0018: IgnoredOperation ─────────────────────────────────────────

    [TestMethod]
    public void StringInterpolation_AlignmentSpecifier_ReportsEXP0018()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public string Name { get; set; }

                    [Expressive]
                    public string Aligned => $"{Name,20}";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0018"),
            "Expected EXP0018 for alignment specifier in string interpolation");
        Assert.IsTrue(result.GeneratedTrees.Length > 0,
            "Generator should still produce output (interpolation without alignment)");
    }

    // ── EXP0009: UnsupportedOperator ────────────────────────────────────────

    [TestMethod]
    public void UnsignedRightShift_ReportsEXP0009()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Value { get; set; }

                    [Expressive]
                    public int Shifted => Value >>> 2;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0009"),
            "Expected EXP0009 for unsigned right shift operator (>>>)");
    }

    // ── EXP0011: UnresolvablePatternMember ──────────────────────────────────

    [TestMethod]
    public void PositionalPattern_NoMatchingProperty_ReportsEXP0011()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Point {
                    public int X { get; set; }
                    public int Y { get; set; }
                    public void Deconstruct(out int a, out int b) { a = X; b = Y; }
                }

                class C {
                    [Expressive]
                    public bool Check(Point p) => p is (1, 2);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0011"),
            "Expected EXP0011 for positional pattern with unresolvable member names");
    }

    // ── EXP0012: FactoryMethodShouldBeConstructor ───────────────────────────

    [TestMethod]
    public void FactoryMethod_ReturningNewContainingType_ReportsEXP0012()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string Name { get; set; }
                    public int Age { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public static PersonDto Create(string name, int age) =>
                        new PersonDto { Name = name, Age = age };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        var diag = result.Diagnostics.FirstOrDefault(d => d.Id == "EXP0012");
        Assert.IsNotNull(diag, "Expected EXP0012 for factory method returning new ContainingType");
        Assert.AreEqual(DiagnosticSeverity.Info, diag.Severity);
        Assert.IsTrue(result.GeneratedTrees.Length > 0,
            "Generator should still produce output alongside Info diagnostic");
    }

    // ── EXP0014: ExpressiveForTargetTypeNotFound ────────────────────────────

    [TestMethod]
    public void ExpressiveFor_NonExistentTargetType_ReportsEXP0014()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                static class Mappings {
                    [ExpressiveFor(typeof(NonExistentType), "Method")]
                    static int Stub(int x) => x;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0014"),
            "Expected EXP0014 for unresolvable target type in [ExpressiveFor]");
    }

    // NOTE: EXP0010 (InterceptorEmissionFailed) is intentionally not tested.
    // It is a catch-all for unhandled exceptions during interceptor generation
    // in PolyfillInterceptorGenerator. No natural user input triggers it — it
    // fires only when an internal bug causes an exception. Testing it would
    // require fault injection, which would change production code for test-only
    // purposes.
}
