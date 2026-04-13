using System.Reflection;
using ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;
using ExpressiveSharp.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Docs.Playground.Core.Services;

public sealed class SnippetCompiler
{
    public const string SnippetFilePath = "/snippet/__Snippet.cs";
    public const string SnippetTypeFullName = "ExpressiveSharp.Docs.Playground.Snippet.__Snippet";

    internal const string SnippetPlaceholder = "/*__SNIPPET__*/";
    internal const string SetupPlaceholder = "/*__SETUP__*/";

    private readonly IPlaygroundReferences _references;

    public SnippetCompiler(IPlaygroundReferences references)
    {
        _references = references;
    }

    public CompileResult Compile(string snippetExpression, string? setupCode, IPlaygroundScenario scenario)
    {
        if (!_references.IsLoaded)
            throw new InvalidOperationException(
                "PlaygroundReferences must be loaded before Compile is called.");

        var wrap = SnippetWrap.Build(scenario.WrapperTemplate, snippetExpression, setupCode);

        var parseOptions = CSharpParseOptions.Default
            .WithLanguageVersion(LanguageVersion.CSharp13)
            .WithFeatures(new[]
            {
                new KeyValuePair<string, string>("InterceptorsNamespaces", "ExpressiveSharp.Generated.Interceptors"),
            });

        var snippetTree = CSharpSyntaxTree.ParseText(
            wrap.Source,
            parseOptions,
            path: SnippetFilePath);

        var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: false)
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>
            {
                ["CS1702"] = ReportDiagnostic.Suppress,
            });

        var compilation = CSharpCompilation.Create(
            assemblyName: "ExpressiveSharp.Docs.Playground.Snippet_" + Guid.NewGuid().ToString("N"),
            syntaxTrees: new[] { snippetTree },
            references: _references.References,
            options: compilationOptions);

        var driver = CSharpGeneratorDriver
            .Create(new ExpressiveGenerator(), new PolyfillInterceptorGenerator())
            .WithUpdatedParseOptions(parseOptions);

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var updatedCompilation,
            out var generatorDiagnostics);

        var generatedSources = new List<GeneratedSource>();
        foreach (var runResult in driver.GetRunResult().Results)
            foreach (var generatedSource in runResult.GeneratedSources)
                generatedSources.Add(new GeneratedSource(
                    generatedSource.HintName,
                    generatedSource.SourceText.ToString()));

        using var peStream = new MemoryStream();
        var emitResult = updatedCompilation.Emit(peStream);

        var diagnostics = generatorDiagnostics
            .Concat(emitResult.Diagnostics)
            .Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
            .Select(SnippetDiagnostic.From)
            .ToList();

        if (!emitResult.Success)
        {
            return new CompileResult(false, null, generatedSources, diagnostics, wrap);
        }

        peStream.Position = 0;
        var assembly = Assembly.Load(peStream.ToArray());
        return new CompileResult(true, assembly, generatedSources, diagnostics, wrap);
    }
}

public sealed class SnippetWrap
{
    public string Source { get; }
    public LinePosition SnippetOrigin { get; }
    public LinePosition SnippetEnd { get; }
    public LinePosition? SetupOrigin { get; }
    public LinePosition? SetupEnd { get; }

    private SnippetWrap(string source, LinePosition snippetOrigin, LinePosition snippetEnd, LinePosition? setupOrigin, LinePosition? setupEnd)
    {
        Source = source;
        SnippetOrigin = snippetOrigin;
        SnippetEnd = snippetEnd;
        SetupOrigin = setupOrigin;
        SetupEnd = setupEnd;
    }

    public static SnippetWrap Build(string template, string snippetExpression, string? setupCode)
    {
        var afterSetup = template.Replace(SnippetCompiler.SetupPlaceholder, setupCode ?? "");
        var setupRange = ComputeSubstitutionRange(template, SnippetCompiler.SetupPlaceholder, setupCode ?? "");

        var afterBoth = afterSetup.Replace(SnippetCompiler.SnippetPlaceholder, snippetExpression);
        var snippetRange = ComputeSubstitutionRange(afterSetup, SnippetCompiler.SnippetPlaceholder, snippetExpression);

        return new SnippetWrap(
            source: afterBoth,
            snippetOrigin: snippetRange.start,
            snippetEnd: snippetRange.end,
            setupOrigin: string.IsNullOrEmpty(setupCode) ? null : setupRange.start,
            setupEnd: string.IsNullOrEmpty(setupCode) ? null : setupRange.end);
    }

    private static (LinePosition start, LinePosition end) ComputeSubstitutionRange(
        string haystack,
        string placeholder,
        string replacement)
    {
        var idx = haystack.IndexOf(placeholder, StringComparison.Ordinal);
        if (idx < 0)
        {
            return (new LinePosition(int.MaxValue, 0), new LinePosition(int.MaxValue, 0));
        }
        var start = OffsetToLinePosition(haystack, idx);
        var end = OffsetToLinePosition(replacement, replacement.Length, baseLine: start.Line, baseColumn: start.Character);
        return (start, end);
    }

    private static LinePosition OffsetToLinePosition(string text, int offset, int baseLine = 0, int baseColumn = 0)
    {
        var line = baseLine;
        var col = baseColumn;
        for (var i = 0; i < offset && i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                line++;
                col = 0;
            }
            else if (text[i] != '\r')
            {
                col++;
            }
        }
        return new LinePosition(line, col);
    }

    public bool IsInSnippet(LinePosition position) =>
        IsBetween(position, SnippetOrigin, SnippetEnd);

    public bool IsInSetup(LinePosition position) =>
        SetupOrigin is { } start && SetupEnd is { } end && IsBetween(position, start, end);

    private static bool IsBetween(LinePosition position, LinePosition start, LinePosition end)
    {
        if (position.Line < start.Line || position.Line > end.Line) return false;
        if (position.Line == start.Line && position.Character < start.Character) return false;
        if (position.Line == end.Line && position.Character > end.Character) return false;
        return true;
    }

    public LinePosition ToSnippetRelative(LinePosition wrapped)
    {
        var line = wrapped.Line - SnippetOrigin.Line;
        var col = wrapped.Line == SnippetOrigin.Line
            ? wrapped.Character - SnippetOrigin.Character
            : wrapped.Character;
        return new LinePosition(Math.Max(0, line), Math.Max(0, col));
    }

    public LinePosition ToWrapped(LinePosition snippetRelative)
    {
        var line = snippetRelative.Line + SnippetOrigin.Line;
        var col = snippetRelative.Line == 0
            ? snippetRelative.Character + SnippetOrigin.Character
            : snippetRelative.Character;
        return new LinePosition(line, col);
    }
}

public sealed record CompileResult(
    bool Success,
    Assembly? Assembly,
    IReadOnlyList<GeneratedSource> GeneratedSources,
    IReadOnlyList<SnippetDiagnostic> Diagnostics,
    SnippetWrap Wrap);

public sealed record GeneratedSource(string HintName, string Source);

public sealed record SnippetDiagnostic(
    DiagnosticSeverity Severity,
    string Id,
    string Message,
    LinePositionSpan? Span,
    bool IsInSource)
{
    public static SnippetDiagnostic From(Diagnostic d) => new(
        d.Severity,
        d.Id,
        d.GetMessage(),
        d.Location.IsInSource ? d.Location.GetLineSpan().Span : null,
        d.Location.IsInSource);

    public override string ToString()
    {
        var loc = Span is { } s
            ? $" @ ({s.Start.Line + 1},{s.Start.Character + 1})-({s.End.Line + 1},{s.End.Character + 1})"
            : "";
        return $"{Severity} {Id}: {Message}{loc}";
    }
}
