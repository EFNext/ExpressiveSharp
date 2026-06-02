using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressiveSharp.EntityFrameworkCore.CodeFixers;
using ExpressiveSharp.Generator.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Generator.Tests.CodeFixers;

[TestClass]
public sealed class WindowFunctionLiteralArgsAnalyzerTests : GeneratorTestBase
{
    private const string WindowFunctionStub = """
        namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions
        {
            public sealed class OrderedWindowDefinition { }
            public static class Window
            {
                public static OrderedWindowDefinition OrderBy<TKey>(TKey key) => null!;
            }
            public static class WindowFunction
            {
                public static long Ntile(int buckets, OrderedWindowDefinition window) => 0;
                public static T Lag<T>(T expression, OrderedWindowDefinition window) => default!;
                public static T Lag<T>(T expression, int offset, OrderedWindowDefinition window) => default!;
                public static T Lead<T>(T expression, OrderedWindowDefinition window) => default!;
                public static T Lead<T>(T expression, int offset, OrderedWindowDefinition window) => default!;
            }
        }
        """;

    [TestMethod]
    public async Task NtileWithZeroBuckets_ReportsEXP0030()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                long M(int x) => WindowFunction.Ntile(0, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0030"),
            "Expected EXP0030 for Ntile(0)");
    }

    [TestMethod]
    public async Task NtileWithNegativeBuckets_ReportsEXP0030()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                long M(int x) => WindowFunction.Ntile(-3, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0030"));
    }

    [TestMethod]
    public async Task NtileWithPositiveLiteral_NoDiagnostic()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                long M(int x) => WindowFunction.Ntile(4, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0030"));
    }

    [TestMethod]
    public async Task NtileWithVariable_NoDiagnostic()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                long M(int x, int n) => WindowFunction.Ntile(n, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0030"));
    }

    [TestMethod]
    public async Task LagWithNegativeOffset_ReportsEXP0031()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                int M(int x) => WindowFunction.Lag(x, -1, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0031"));
    }

    [TestMethod]
    public async Task LeadWithNegativeOffset_ReportsEXP0031()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                int M(int x) => WindowFunction.Lead(x, -2, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0031"));
    }

    [TestMethod]
    public async Task LagWithPositiveOffset_NoDiagnostic()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                int M(int x) => WindowFunction.Lag(x, 1, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0031"));
    }

    [TestMethod]
    public async Task LagWithoutOffsetOverload_NoDiagnostic()
    {
        const string source = """
            using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
            class C
            {
                int M(int x) => WindowFunction.Lag(x, Window.OrderBy(x));
            }
            """;

        var diagnostics = await GetWindowDiagnosticsAsync(source);
        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0031"));
    }

    private async Task<ImmutableArray<Diagnostic>> GetWindowDiagnosticsAsync(string source)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        var trees = new[]
        {
            CSharpSyntaxTree.ParseText(WindowFunctionStub, parseOptions, "Stubs.cs"),
            CSharpSyntaxTree.ParseText(source, parseOptions, "TestFile.cs"),
        };

        var compilation = CSharpCompilation.Create(
            "TestProject",
            trees,
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new WindowFunctionLiteralArgsAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }
}
