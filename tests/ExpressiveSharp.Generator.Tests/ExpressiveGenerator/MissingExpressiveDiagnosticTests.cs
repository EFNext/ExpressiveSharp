using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressiveSharp.CodeFixers;
using ExpressiveSharp.Generator.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class MissingExpressiveDiagnosticTests : GeneratorTestBase
{
    // ── Positive: EXP0013 fires ─────────────────────────────────────────────

    [TestMethod]
    public async Task MethodCall_ToSourceMethodWithExpressionBody_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class C {
                    public static int Transform(int x) => x * x;

                    [Expressive]
                    public int Computed => Transform(42);
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for method call to source method without [Expressive]");
    }

    [TestMethod]
    public async Task PropertyAccess_ToSourcePropertyWithExpressionBody_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
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

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for property access to source property without [Expressive]");
    }

    [TestMethod]
    public async Task PropertyAccess_ToSourcePropertyWithBlockGetter_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
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

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for property access to source property with block getter without [Expressive]");
    }

    [TestMethod]
    public async Task MethodCall_ToBlockBodyMethod_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class C {
                    public static int Compute(int x) { return x * x; }

                    [Expressive]
                    public int Result => Compute(5);
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for method call to block-body method without [Expressive]");
    }

    [TestMethod]
    public async Task ExtensionMethod_OnNonEnumReceiver_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                public static class StringExtensions {
                    public static string Shout(this string value) => value.ToUpper() + "!";
                }

                class C {
                    public string Name { get; set; } = "";

                    [Expressive]
                    public string Loud => Name.Shout();
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for non-enum extension method without [Expressive]");
    }

    [TestMethod]
    public async Task PropertyWithoutExpressive_ReferencedInExpressive_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class Order {
                    public double Price { get; set; }
                    public int Quantity { get; set; }
                    public double Total => Price * Quantity;

                    [Expressive]
                    public string? Label => Total >= 0 ? "Positive" : "Negative";
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for property without [Expressive] referenced in [Expressive] member");
    }

    [TestMethod]
    public async Task EXP0013_HasAdditionalLocation_PointingToDeclaration()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class C {
                    public static int Transform(int x) => x * x;

                    [Expressive]
                    public int Computed => Transform(42);
                }
            }
            """);

        var diag = diagnostics.First(d => d.Id == "EXP0013");
        Assert.AreEqual(1, diag.AdditionalLocations.Count,
            "EXP0013 should include the declaration as an additional location");
    }

    // ── Negative: no EXP0013 ────────────────────────────────────────────────

    [TestMethod]
    public async Task MethodCall_ToMethodWithExpressive_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
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

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn when referenced method already has [Expressive]");
    }

    [TestMethod]
    public async Task PropertyAccess_ToAutoProperty_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo => Bar;
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for auto-property access");
    }

    [TestMethod]
    public async Task MethodCall_ToBclMethod_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class C {
                    public string Name { get; set; }

                    [Expressive]
                    public string Upper => Name.ToUpper();
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for BCL method call");
    }

    [TestMethod]
    public async Task PropertyAccess_ToExpressiveProperty_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
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

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn when referenced property already has [Expressive]");
    }

    [TestMethod]
    public async Task ExtensionMethod_OnEnumReceiver_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                public enum OrderStatus { Pending, Approved, Rejected }

                public static class OrderStatusExtensions {
                    public static string GetDescription(this OrderStatus value) => value switch {
                        OrderStatus.Pending => "Awaiting processing",
                        OrderStatus.Approved => "Order approved",
                        _ => value.ToString(),
                    };
                }

                class Order {
                    public OrderStatus Status { get; set; }

                    [Expressive]
                    public string StatusDescription => Status.GetDescription();
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for enum extension method — generator expands these via TryEmitEnumMethodExpansion");
    }

    // ── Prong 2: IRewritableQueryable LINQ lambdas ────────────────────────

    [TestMethod]
    public async Task RewritableQueryable_Select_WithNonExpressiveExtensionMethod_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                class Todo {
                    public string Name { get; set; } = "";
                }
                class TodoItem {
                    public string Name { get; set; } = "";
                }

                static class TodoExtensions {
                    public static TodoItem AsTodoItem(this Todo t)
                        => new TodoItem { Name = t.Name };
                }

                class C {
                    void Run(IRewritableQueryable<Todo> source) {
                        source.Select(t => t.AsTodoItem());
                    }
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for non-[Expressive] extension method in IRewritableQueryable Select lambda");
    }

    [TestMethod]
    public async Task RewritableQueryable_Where_WithNonExpressiveInstanceMethod_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                class Todo {
                    public string Name { get; set; } = "";
                    public bool IsImportant() => Name.Length > 10;
                }

                class C {
                    void Run(IRewritableQueryable<Todo> source) {
                        source.Where(t => t.IsImportant());
                    }
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for non-[Expressive] instance method in IRewritableQueryable Where lambda");
    }

    [TestMethod]
    public async Task RewritableQueryable_Select_WithNonExpressiveProperty_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                class Todo {
                    public string FirstName { get; set; } = "";
                    public string LastName { get; set; } = "";
                    public string FullName => FirstName + " " + LastName;
                }

                class C {
                    void Run(IRewritableQueryable<Todo> source) {
                        source.Select(t => t.FullName);
                    }
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for non-[Expressive] property in IRewritableQueryable Select lambda");
    }

    [TestMethod]
    public async Task RewritableQueryable_MethodGroup_WithNonExpressiveMethod_WarnsEXP0013()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                class Todo {
                    public string Name { get; set; } = "";
                }
                class TodoItem {
                    public string Name { get; set; } = "";
                }

                static class TodoExtensions {
                    public static TodoItem AsTodoItem(this Todo t)
                        => new TodoItem { Name = t.Name };
                }

                class C {
                    static TodoItem Convert(Todo t) => new TodoItem { Name = t.Name };

                    void Run(IRewritableQueryable<Todo> source) {
                        source.Select(Convert);
                    }
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "Expected EXP0013 for non-[Expressive] method group in IRewritableQueryable Select");
    }

    // ── Prong 2 Negative: no EXP0013 in LINQ lambdas ───────────────────────

    [TestMethod]
    public async Task RewritableQueryable_WithExpressiveMethod_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                class Todo {
                    public string Name { get; set; } = "";
                }
                class TodoItem {
                    public string Name { get; set; } = "";
                }

                static class TodoExtensions {
                    [Expressive]
                    public static TodoItem AsTodoItem(this Todo t)
                        => new TodoItem { Name = t.Name };
                }

                class C {
                    void Run(IRewritableQueryable<Todo> source) {
                        source.Select(t => t.AsTodoItem());
                    }
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn when referenced method already has [Expressive] in LINQ lambda");
    }

    [TestMethod]
    public async Task RewritableQueryable_WithBclMethod_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                class Todo {
                    public string Name { get; set; } = "";
                }

                class C {
                    void Run(IRewritableQueryable<Todo> source) {
                        source.Select(t => t.Name.ToUpper());
                    }
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for BCL method call in IRewritableQueryable LINQ lambda");
    }

    [TestMethod]
    public async Task RewritableQueryable_WithAutoProperty_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                class Todo {
                    public string Name { get; set; } = "";
                }

                class C {
                    void Run(IRewritableQueryable<Todo> source) {
                        source.Select(t => t.Name);
                    }
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for auto-property access in IRewritableQueryable LINQ lambda");
    }

    [TestMethod]
    public async Task RewritableQueryable_WithEnumExtensionMethod_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs {
                enum Priority { Low, Medium, High }

                static class PriorityExtensions {
                    public static string Label(this Priority p) => p switch {
                        Priority.Low => "Low",
                        Priority.Medium => "Medium",
                        _ => "High",
                    };
                }

                class Todo {
                    public Priority Priority { get; set; }
                }

                class C {
                    void Run(IRewritableQueryable<Todo> source) {
                        source.Select(t => t.Priority.Label());
                    }
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "Should not warn for enum extension method in IRewritableQueryable LINQ lambda");
    }

    // ── Helper ──────────────────────────────────────────────────────────────

    private async Task<ImmutableArray<Diagnostic>> RunAnalyzerAsync(string source)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        var compilation = CSharpCompilation.Create(
            "TestCompilation",
            new[]
            {
                CSharpSyntaxTree.ParseText(
                    """
                    global using System;
                    global using System.Collections.Generic;
                    global using System.Linq;
                    global using ExpressiveSharp;
                    """, parseOptions, "GlobalUsings.cs"),
                CSharpSyntaxTree.ParseText(source, parseOptions, "TestFile.cs"),
            },
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new MissingExpressiveAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        return diagnostics;
    }
}
