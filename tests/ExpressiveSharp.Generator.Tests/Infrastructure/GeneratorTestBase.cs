using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

namespace ExpressiveSharp.Generator.Tests.Infrastructure;

[TestClass]
[UsesVerify]
public abstract partial class GeneratorTestBase
{

    /// <summary>
    /// Wraps <see cref="GeneratorDriverRunResult"/> filtering out <c>ExpressionRegistry.g.cs</c>
    /// from <see cref="GeneratedTrees"/> so per-member tests are unaffected by the registry file.
    /// </summary>
    protected sealed class TestGeneratorRunResult
    {
        private readonly GeneratorDriverRunResult _inner;

        public TestGeneratorRunResult(GeneratorDriverRunResult inner) => _inner = inner;

        public ImmutableArray<Diagnostic> Diagnostics => _inner.Diagnostics;

        /// <summary>Generated trees excluding <c>ExpressionRegistry.g.cs</c> and <c>*.Attributes.g.cs</c>.</summary>
        public ImmutableArray<SyntaxTree> GeneratedTrees =>
            _inner.GeneratedTrees
                .Where(t => !t.FilePath.EndsWith("ExpressionRegistry.g.cs", StringComparison.Ordinal)
                    && !t.FilePath.EndsWith(".Attributes.g.cs", StringComparison.Ordinal))
                .ToImmutableArray();

        /// <summary>All generated trees including <c>ExpressionRegistry.g.cs</c>.</summary>
        public ImmutableArray<SyntaxTree> AllGeneratedTrees => _inner.GeneratedTrees;

        /// <summary>The registry tree, or <c>null</c> if none was generated.</summary>
        public SyntaxTree? RegistryTree =>
            _inner.GeneratedTrees
                .FirstOrDefault(t => t.FilePath.EndsWith("ExpressionRegistry.g.cs", StringComparison.Ordinal));
    }

    // ── Reference helpers ────────────────────────────────────────────────────

    protected IReadOnlyList<MetadataReference> GetDefaultReferences()
    {
        var references = Basic.Reference.Assemblies.
#if NET10_0_OR_GREATER
            Net100
#elif NET9_0
            Net90
#else
            Net80
#endif
            .References.All.ToList<MetadataReference>();

        references.Add(MetadataReference.CreateFromFile(typeof(ExpressiveAttribute).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(IExpressiveQueryable<>).Assembly.Location));
        return references;
    }

    private static CSharpParseOptions CreateParseOptions(LanguageVersion languageVersion = LanguageVersion.Latest)
    {
        var interceptorFeature = new KeyValuePair<string, string>(
#if NET10_0_OR_GREATER
            "InterceptorsNamespaces",
#else
            "InterceptorsPreviewNamespaces",
#endif
            "ExpressiveSharp.Generated.Interceptors");

        return CSharpParseOptions.Default
            .WithLanguageVersion(languageVersion)
            .WithFeatures([interceptorFeature]);
    }

    // ── Compilation factory ──────────────────────────────────────────────────

    private const string GlobalUsings = """
        global using System;
        global using System.Collections.Generic;
        global using System.Linq;
        global using ExpressiveSharp;
        """;

    protected Compilation CreateCompilation([StringSyntax("csharp")] string source, string path = "TestFile.cs")
    {
        var parseOptions = CreateParseOptions();
        var compilation = CSharpCompilation.Create(
            "compilation",
            [
                CSharpSyntaxTree.ParseText(GlobalUsings, parseOptions, "GlobalUsings.cs"),
                CSharpSyntaxTree.ParseText(source, parseOptions, path),
            ],
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        LogSourceDiagnostics(compilation);
        return compilation;
    }

    protected Compilation CreateCompilation(params string[] sources)
    {
        var parseOptions = CreateParseOptions();
        var trees = new List<SyntaxTree>
        {
            CSharpSyntaxTree.ParseText(GlobalUsings, parseOptions, "GlobalUsings.cs"),
        };
        trees.AddRange(sources.Select(s => CSharpSyntaxTree.ParseText(s, parseOptions)));
        var compilation = CSharpCompilation.Create(
            "compilation",
            trees,
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        LogSourceDiagnostics(compilation);
        return compilation;
    }

    // ── Generator runners ────────────────────────────────────────────────────

    protected TestGeneratorRunResult RunExpressiveGenerator(
        Compilation compilation,
        IReadOnlyDictionary<string, string>? globalProperties = null)
    {
        TestContext.WriteLine("Running ExpressiveGenerator...");

        var subject = new global::ExpressiveSharp.Generator.ExpressiveGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(subject)
            .WithUpdatedParseOptions((CSharpParseOptions)compilation.SyntaxTrees.First().Options);

        if (globalProperties is not null)
        {
            driver = driver.WithUpdatedAnalyzerConfigOptions(
                new DictionaryAnalyzerConfigOptionsProvider(globalProperties));
        }

        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation, out var outputCompilation, out _);

        var result = new TestGeneratorRunResult(driver.GetRunResult());
        LogGeneratorResult(result, outputCompilation);
        return result;
    }

    protected GeneratorDriverRunResult RunPolyfillInterceptorGenerator(
        Compilation compilation,
        IReadOnlyDictionary<string, string>? globalProperties = null)
    {
        TestContext.WriteLine("Running PolyfillInterceptorGenerator...");

        var subject = new global::ExpressiveSharp.Generator.PolyfillInterceptorGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(subject)
            .WithUpdatedParseOptions((CSharpParseOptions)compilation.SyntaxTrees.First().Options);

        if (globalProperties is not null)
        {
            driver = driver.WithUpdatedAnalyzerConfigOptions(
                new DictionaryAnalyzerConfigOptionsProvider(globalProperties));
        }

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var result = driver.GetRunResult();
        LogPolyfillResult(result, outputCompilation);
        return result;
    }

    // ── Logging helpers ──────────────────────────────────────────────────────

    private void LogSourceDiagnostics(Compilation compilation)
    {
        var diagnostics = compilation.GetDiagnostics();
        if (diagnostics.IsEmpty)
            return;

        TestContext.WriteLine("Source compilation diagnostics:");
        foreach (var d in diagnostics)
            TestContext.WriteLine("  > " + d);

        // Note: source errors are expected in negative test cases; individual tests assert as needed.
    }

    private void LogGeneratorResult(TestGeneratorRunResult result, Compilation outputCompilation)
    {
        if (result.Diagnostics.IsEmpty)
            TestContext.WriteLine("Generator produced no diagnostics.");
        else
        {
            TestContext.WriteLine("Generator diagnostics:");
            foreach (var d in result.Diagnostics)
                TestContext.WriteLine("  > " + d);
        }

        foreach (var tree in result.AllGeneratedTrees)
        {
            TestContext.WriteLine($"Generated: {tree.FilePath}");
            TestContext.WriteLine(tree.GetText().ToString());
        }

        if (!result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error)
            && result.AllGeneratedTrees.Length > 0)
        {
            var errors = outputCompilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToList();

            if (errors.Count > 0)
            {
                TestContext.WriteLine("Compilation errors in generated code:");
                foreach (var e in errors)
                    TestContext.WriteLine("  > " + e);
            }

            Assert.AreEqual(0, errors.Count, "Generated code should compile without errors.");
        }
    }

    private void LogPolyfillResult(GeneratorDriverRunResult result, Compilation outputCompilation)
    {
        foreach (var tree in result.GeneratedTrees)
        {
            TestContext.WriteLine($"Generated: {tree.FilePath}");
            TestContext.WriteLine(tree.GetText().ToString());
        }

        var errors = outputCompilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (errors.Count > 0)
        {
            TestContext.WriteLine("Compilation errors:");
            foreach (var e in errors)
                TestContext.WriteLine("  > " + e);
        }
    }

    // ── MSBuild global property injection ────────────────────────────────────

    private sealed class DictionaryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly IReadOnlyDictionary<string, string> _dict;

        public DictionaryAnalyzerConfigOptions(IReadOnlyDictionary<string, string> dict) => _dict = dict;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
            _dict.TryGetValue(key, out value);
    }

    private sealed class DictionaryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly AnalyzerConfigOptions _empty =
            new DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string>.Empty);

        public DictionaryAnalyzerConfigOptionsProvider(IReadOnlyDictionary<string, string> globalOptions) =>
            GlobalOptions = new DictionaryAnalyzerConfigOptions(globalOptions);

        public override AnalyzerConfigOptions GlobalOptions { get; }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _empty;
    }
}
