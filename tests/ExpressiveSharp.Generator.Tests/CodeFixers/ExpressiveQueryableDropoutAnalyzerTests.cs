using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressiveSharp.CodeFixers;
using ExpressiveSharp.Generator.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Generator.Tests.CodeFixers;

[TestClass]
public sealed class ExpressiveQueryableDropoutAnalyzerTests : GeneratorTestBase
{
    /// <summary>
    /// Stubs for the EF Core types and the ExpressiveSharp.EntityFrameworkCore shadow extensions —
    /// just enough surface for the analyzer's symbol checks to bind without pulling in the real
    /// packages as references.
    /// </summary>
    private const string Stubs = """
        namespace Microsoft.EntityFrameworkCore
        {
            public interface IIncludableQueryable<out TEntity, out TProperty>
                : System.Linq.IQueryable<TEntity> { }

            public static class EntityFrameworkQueryableExtensions
            {
                public static IIncludableQueryable<TEntity, TProperty> Include<TEntity, TProperty>(
                    this System.Linq.IQueryable<TEntity> source,
                    System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> path)
                    where TEntity : class
                    => null!;
            }
        }

        namespace ExpressiveSharp.EntityFrameworkCore
        {
            public static class ExpressiveQueryableEfCoreExtensions
            {
                public static ExpressiveSharp.IExpressiveQueryable<TEntity> AsNoTracking<TEntity>(
                    this ExpressiveSharp.IExpressiveQueryable<TEntity> source)
                    where TEntity : class
                    => source;
            }
        }

        namespace Test
        {
            public static class UserHelpers
            {
                // Drops the chain: takes IQueryable<T>, returns IQueryable<T>.
                public static System.Linq.IQueryable<T> Filter<T>(this System.Linq.IQueryable<T> q)
                    => q;

                [ExpressiveSharp.NotExpressive]
                public static System.Linq.IQueryable<T> Sanitize<T>(this System.Linq.IQueryable<T> q)
                    => q;
            }
        }
        """;

    [TestMethod]
    public async Task ExpressiveReceiver_PlainHelper_ReportsEXP0036()
    {
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            using Test;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                class C
                {
                    void M(IQueryable<Order> orders)
                    {
                        orders.AsExpressive().Filter();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0036"),
            "Expected EXP0036 when a plain-IQueryable helper is called on an expressive receiver");
    }

    [TestMethod]
    public async Task ExpressiveReceiver_AsQueryable_NoEXP0036()
    {
        // .AsQueryable() is a sanctioned explicit downcast — we treat it as user intent.
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                class C
                {
                    void M(IQueryable<Order> orders)
                    {
                        orders.AsExpressive().AsQueryable();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0036"),
            "EXP0036 should not fire on an explicit .AsQueryable() — it is the sanctioned downcast");
    }

    [TestMethod]
    public async Task ExpressiveReceiver_PlainHelper_FiresOnceEvenWithDownstreamCalls()
    {
        // The whole point of the generalized analyzer: surface the dropout point itself, not every
        // downstream call after it. Even with a long chain after .Filter(), only one diagnostic.
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            using Microsoft.EntityFrameworkCore;
            using Test;
            namespace Test
            {
                public class Order
                {
                    public int Id { get; set; }
                    public Customer? Customer { get; set; }
                }
                public class Customer { public string Name { get; set; } = ""; }

                class C
                {
                    void M(IQueryable<Order> orders)
                    {
                        orders.AsExpressive().Filter().Include(o => o.Customer);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        var hits = diagnostics.Count(d => d.Id == "EXP0036");
        Assert.AreEqual(1, hits,
            "EXP0036 should fire once at the dropout point, not at downstream Include/Where/etc.");
    }

    [TestMethod]
    public async Task ExpressiveReceiver_ExpressiveShadow_NoEXP0036()
    {
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            using ExpressiveSharp.EntityFrameworkCore;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                class C
                {
                    void M(IQueryable<Order> orders)
                    {
                        orders.AsExpressive().AsNoTracking();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0036"),
            "EXP0036 should not fire when the called method preserves IExpressiveQueryable<T>");
    }

    [TestMethod]
    public async Task PlainReceiver_NoEXP0036()
    {
        const string source = """
            using System.Linq;
            using Test;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                class C
                {
                    void M(IQueryable<Order> orders)
                    {
                        orders.Filter();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0036"),
            "EXP0036 should not fire when the chain was never expressive to begin with");
    }

    [TestMethod]
    public async Task ExpressiveReceiver_NotExpressiveMethod_NoEXP0036()
    {
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            using Test;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                class C
                {
                    void M(IQueryable<Order> orders)
                    {
                        orders.AsExpressive().Sanitize();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0036"),
            "[NotExpressive] on the offending method should suppress EXP0036");
    }

    [TestMethod]
    public async Task ExpressiveReceiver_TerminatingCall_NoEXP0036()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;
            using ExpressiveSharp;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                class C
                {
                    void M(IQueryable<Order> orders)
                    {
                        List<Order> result = orders.AsExpressive().ToList();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0036"),
            "EXP0036 should not fire on terminating calls whose result is not IQueryable<T>");
    }

    [TestMethod]
    public async Task ExpressiveReceiver_BuiltInWhereWithInlineLambda_NoEXP0036()
    {
        // Queryable.Where on an IExpressiveQueryable<T> receiver is rewritten by the
        // polyfill interceptor into the IExpressiveQueryable.Where stub at compile time,
        // so the runtime chain is preserved even though the source-level resolved symbol
        // is Queryable.Where. The analyzer should treat sibling-extension cases as exempt.
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                class C
                {
                    void M(IExpressiveQueryable<Order> orders)
                    {
                        var filtered = orders.Where(o => o.Id > 0);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0036"),
            "EXP0036 should not fire on built-in LINQ calls whose IExpressiveQueryable<T> sibling stub exists.");
    }

    [TestMethod]
    public async Task ExpressiveDbSet_BuiltInWhereWithInlineLambda_OnlyEXP0021Fires_NotEXP0036()
    {
        // The user's StoryGrain shape: ExpressiveDbSet<T>-style receiver, no
        // `using ExpressiveSharp;`. EXP0021 owns this scenario (Warning + codefix);
        // EXP0036 stays silent here so the user gets one actionable diagnostic
        // instead of two overlapping ones.
        const string source = """
            using System.Linq;
            namespace Test
            {
                public class Order { public int Id { get; set; } }

                public class StubDbSet<T> : System.Linq.IQueryable<T>
                {
                    public System.Linq.Expressions.Expression Expression => null!;
                    public System.Type ElementType => typeof(T);
                    public System.Linq.IQueryProvider Provider => null!;
                    public System.Collections.Generic.IEnumerator<T> GetEnumerator() => null!;
                    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => null!;
                }

                public class StubExpressiveDbSet<T> : StubDbSet<T>, ExpressiveSharp.IExpressiveQueryable<T> { }

                class C
                {
                    void M(StubExpressiveDbSet<Order> orders)
                    {
                        var filtered = orders.Where(o => o.Id > 0);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source,
            new ExpressiveQueryableDropoutAnalyzer(),
            new MissingExpressiveImportAnalyzer());

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0021"),
            "EXP0021 should own the missing-using scenario.");
        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0036"),
            "EXP0036 should suppress itself when EXP0021 covers the same dropout cause.");
    }

    private async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
        => await GetDiagnosticsAsync(source, new ExpressiveQueryableDropoutAnalyzer());

    private async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source, params DiagnosticAnalyzer[] analyzers)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        var trees = new[]
        {
            CSharpSyntaxTree.ParseText(Stubs, parseOptions, "Stubs.cs"),
            CSharpSyntaxTree.ParseText(source, parseOptions, "TestFile.cs"),
        };

        var compilation = CSharpCompilation.Create(
            "TestProject",
            trees,
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create(analyzers));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }
}
