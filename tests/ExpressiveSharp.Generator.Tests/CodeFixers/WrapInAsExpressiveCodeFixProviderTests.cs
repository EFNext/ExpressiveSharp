using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressiveSharp.CodeFixers;
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
public sealed class WrapInAsExpressiveCodeFixProviderTests : GeneratorTestBase
{
    [TestMethod]
    public async Task WrapsRoot_OnSingleLinqCall()
    {
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            namespace Test
            {
                class User
                {
                    public int Age { get; set; }

                    [Expressive]
                    public bool IsAdult => Age >= 18;
                }

                class C
                {
                    void M(IQueryable<User> users)
                    {
                        users.Where(u => u.IsAdult);
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        Assert.IsTrue(fixedSource.Contains("users.AsExpressive().Where(u => u.IsAdult)"),
            $"Expected '.AsExpressive()' inserted before '.Where', got:\n{fixedSource}");
    }

    [TestMethod]
    public async Task WrapsRoot_OnChainedLinqCalls()
    {
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            namespace Test
            {
                class User
                {
                    public int Age { get; set; }

                    [Expressive]
                    public bool IsAdult => Age >= 18;
                }

                class C
                {
                    void M(IQueryable<User> users)
                    {
                        users.OrderBy(u => u.Age).Where(u => u.IsAdult);
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        Assert.IsTrue(fixedSource.Contains("users.AsExpressive().OrderBy(u => u.Age).Where(u => u.IsAdult)"),
            $"Expected '.AsExpressive()' inserted at chain root, got:\n{fixedSource}");
    }

    [TestMethod]
    public async Task AddsUsingDirective_WhenMissing()
    {
        const string source = """
            using System.Linq;
            namespace Test
            {
                class User
                {
                    public int Age { get; set; }

                    [ExpressiveSharp.Expressive]
                    public bool IsAdult => Age >= 18;
                }

                class C
                {
                    void M(IQueryable<User> users)
                    {
                        users.Where(u => u.IsAdult);
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        Assert.IsTrue(fixedSource.Contains("using ExpressiveSharp;"),
            $"Expected 'using ExpressiveSharp;' added, got:\n{fixedSource}");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<string> ApplyCodeFixAsync(string source)
    {
        using var workspace = new Microsoft.CodeAnalysis.AdhocWorkspace();
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
        var document = workspace.AddDocument(project.Id, "TestFile.cs", SourceText.From(source));
        project = document.Project;

        var compilation = await project.GetCompilationAsync()
            ?? throw new System.Exception("Failed to get compilation");

        var analyzer = new PlainQueryableMissingAsExpressiveAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        var analyzerDiagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        var diagnostic = analyzerDiagnostics.FirstOrDefault(d => d.Id == "EXP0028");
        Assert.IsNotNull(diagnostic, "Expected EXP0028 diagnostic to be emitted");

        var docInSolution = project.Solution.GetDocument(diagnostic.Location.SourceTree)
            ?? throw new System.Exception("Failed to locate document for diagnostic");

        var codeFix = new WrapInAsExpressiveCodeFixProvider();
        var actions = new List<CodeAction>();
        var context = new CodeFixContext(
            docInSolution,
            diagnostic,
            (action, _) => actions.Add(action),
            CancellationToken.None);

        await codeFix.RegisterCodeFixesAsync(context);
        Assert.IsTrue(actions.Count > 0, "Expected at least one code fix action");

        var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
        var applyOperation = operations.OfType<ApplyChangesOperation>().First();
        var fixedDoc = applyOperation.ChangedSolution.GetDocument(docInSolution.Id)
            ?? throw new System.Exception("Failed to locate fixed document");

        return (await fixedDoc.GetTextAsync()).ToString();
    }
}
