using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressiveSharp.EntityFrameworkCore.CodeFixers;
using ExpressiveSharp.Generator.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Generator.Tests.CodeFixers;

[TestClass]
public sealed class MigrationAnalyzerTests : GeneratorTestBase
{
    /// <summary>
    /// Stub that mimics the EntityFrameworkCore.Projectables API surface
    /// so the semantic model can resolve symbols during analysis.
    /// UseProjectables() is defined in the Microsoft.EntityFrameworkCore namespace
    /// (following EF Core's extension method discoverability convention).
    /// </summary>
    private const string ProjectablesStub = """
        namespace EntityFrameworkCore.Projectables
        {
            [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property)]
            public class ProjectableAttribute : System.Attribute { }
        }
        namespace Microsoft.EntityFrameworkCore
        {
            public class DbContextOptionsBuilder { }
            public static class DbContextOptionsExtensions
            {
                public static DbContextOptionsBuilder UseProjectables(
                    this DbContextOptionsBuilder builder) => builder;
            }
        }
        """;

    [TestMethod]
    public async Task DetectsProjectableAttribute()
    {
        const string source = """
            using EntityFrameworkCore.Projectables;
            class C
            {
                [Projectable]
                public int Value => 42;
            }
            """;

        var diagnostics = await GetMigrationDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP1001"),
            "Expected EXP1001 for [Projectable] attribute");
    }

    [TestMethod]
    public async Task DetectsUseProjectablesCall()
    {
        const string source = """
            using Microsoft.EntityFrameworkCore;
            class C
            {
                void Configure(DbContextOptionsBuilder builder)
                {
                    builder.UseProjectables();
                }
            }
            """;

        var diagnostics = await GetMigrationDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP1002"),
            "Expected EXP1002 for UseProjectables() call");
    }

    [TestMethod]
    public async Task DetectsProjectablesUsingDirective()
    {
        const string source = """
            using EntityFrameworkCore.Projectables;
            class C { }
            """;

        var diagnostics = await GetMigrationDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP1003"),
            "Expected EXP1003 for EntityFrameworkCore.Projectables using");
    }

    [TestMethod]
    public async Task DetectsProjectablesExtensionsUsingDirective()
    {
        const string source = """
            using EntityFrameworkCore.Projectables.Extensions;
            class C { }
            """;

        var diagnostics = await GetMigrationDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP1003"),
            "Expected EXP1003 for EntityFrameworkCore.Projectables.Extensions using");
    }

    [TestMethod]
    public async Task CodeFix_ReplacesProjectableWithExpressive()
    {
        const string source = """
            using EntityFrameworkCore.Projectables;
            class C
            {
                [Projectable]
                public int Value => 42;
            }
            """;

        var fixedSource = await ApplyMigrationFixAsync(source, "EXP1001");

        Assert.IsTrue(fixedSource.Contains("[Expressive]"),
            "Expected [Projectable] to be replaced with [Expressive]");
        Assert.IsFalse(fixedSource.Contains("[Projectable]"),
            "Expected [Projectable] to be removed");
    }

    [TestMethod]
    public async Task CodeFix_ReplacesUseProjectablesWithUseExpressives()
    {
        const string source = """
            using Microsoft.EntityFrameworkCore;
            class C
            {
                void Configure(DbContextOptionsBuilder builder)
                {
                    builder.UseProjectables();
                }
            }
            """;

        var fixedSource = await ApplyMigrationFixAsync(source, "EXP1002");

        Assert.IsTrue(fixedSource.Contains("UseExpressives()"),
            "Expected UseProjectables() to be replaced with UseExpressives()");
        Assert.IsFalse(fixedSource.Contains("UseProjectables"),
            "Expected UseProjectables to be removed");
    }

    [TestMethod]
    public async Task CodeFix_ReplacesProjectablesNamespace()
    {
        const string source = """
            using EntityFrameworkCore.Projectables;
            class C { }
            """;

        var fixedSource = await ApplyMigrationFixAsync(source, "EXP1003");

        Assert.IsTrue(fixedSource.Contains("using ExpressiveSharp;"),
            "Expected namespace to be replaced with ExpressiveSharp");
        Assert.IsFalse(fixedSource.Contains("EntityFrameworkCore.Projectables"),
            "Expected old namespace to be removed");
    }

    [TestMethod]
    public async Task CodeFix_ReplacesExtensionsNamespace()
    {
        const string source = """
            using EntityFrameworkCore.Projectables.Extensions;
            class C { }
            """;

        var fixedSource = await ApplyMigrationFixAsync(source, "EXP1003");

        Assert.IsTrue(fixedSource.Contains("using ExpressiveSharp.Extensions;"),
            "Expected namespace to be replaced with ExpressiveSharp.Extensions");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<ImmutableArray<Diagnostic>> GetMigrationDiagnosticsAsync(
        string source, string? customStub = null)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        var trees = new[]
        {
            CSharpSyntaxTree.ParseText(customStub ?? ProjectablesStub, parseOptions, "Stubs.cs"),
            CSharpSyntaxTree.ParseText(source, parseOptions, "TestFile.cs"),
        };

        var compilation = CSharpCompilation.Create(
            "TestProject",
            trees,
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new MigrationAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }

    private async Task<string> ApplyMigrationFixAsync(string source, string expectedDiagnosticId)
    {
        var workspace = new Microsoft.CodeAnalysis.AdhocWorkspace();
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

        var projectId = ProjectId.CreateNewId();
        var projectInfo = ProjectInfo.Create(
            projectId,
            VersionStamp.Create(),
            "TestProject",
            "TestProject",
            LanguageNames.CSharp,
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            parseOptions: parseOptions,
            metadataReferences: GetDefaultReferences());

        var project = workspace.AddProject(projectInfo);

        // Add the Projectables stubs
        var stubDoc = workspace.AddDocument(project.Id, "Stubs.cs",
            SourceText.From(ProjectablesStub));
        project = stubDoc.Project;

        // Add the source under test
        var testDoc = workspace.AddDocument(project.Id, "TestFile.cs",
            SourceText.From(source));
        project = testDoc.Project;

        // Run analyzer
        var compilation = await project.GetCompilationAsync()
            ?? throw new System.Exception("Failed to get compilation");

        var analyzer = new MigrationAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        var diagnostic = diagnostics.FirstOrDefault(d => d.Id == expectedDiagnosticId);
        Assert.IsNotNull(diagnostic, $"Expected {expectedDiagnosticId} diagnostic");

        // Find the document containing the diagnostic
        var diagnosticTree = diagnostic.Location.SourceTree;
        Assert.IsNotNull(diagnosticTree);
        var diagnosticDoc = project.Solution.GetDocument(diagnosticTree);
        Assert.IsNotNull(diagnosticDoc);

        // Apply the code fix
        var codeFix = new MigrationCodeFixProvider();
        var actions = new System.Collections.Generic.List<CodeAction>();
        var context = new CodeFixContext(
            diagnosticDoc,
            diagnostic,
            (action, _) => actions.Add(action),
            CancellationToken.None);

        await codeFix.RegisterCodeFixesAsync(context);
        Assert.IsTrue(actions.Count > 0, "Expected at least one code fix action");

        var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
        var applyOperation = operations.OfType<ApplyChangesOperation>().First();
        var fixedSolution = applyOperation.ChangedSolution;

        var fixedDoc = fixedSolution.GetDocument(diagnosticDoc.Id);
        Assert.IsNotNull(fixedDoc);

        return (await fixedDoc.GetTextAsync()).ToString();
    }
}
