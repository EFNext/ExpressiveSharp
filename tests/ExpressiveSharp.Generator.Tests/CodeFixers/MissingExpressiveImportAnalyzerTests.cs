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

namespace ExpressiveSharp.Generator.Tests.CodeFixers;

[TestClass]
public sealed class MissingExpressiveImportAnalyzerTests : GeneratorTestBase
{
    [TestMethod]
    public async Task Where_WithoutUsing_ReportsEXP0026()
    {
        const string source = """
            using System;
            using System.Linq;
            namespace Test
            {
                class C
                {
                    void M(ExpressiveSharp.IExpressiveQueryable<int> q)
                    {
                        q.Where(x => x > 0);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0026"),
            "Expected EXP0026 for Where on IExpressiveQueryable without using");
    }

    [TestMethod]
    public async Task Take_WithoutUsing_ReportsEXP0026()
    {
        const string source = """
            using System;
            using System.Linq;
            namespace Test
            {
                class C
                {
                    void M(ExpressiveSharp.IExpressiveQueryable<int> q)
                    {
                        q.Take(10);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0026"),
            "Expected EXP0026 for Take on IExpressiveQueryable without using (chain break)");
    }

    [TestMethod]
    public async Task Where_WithUsing_NoDiagnostic()
    {
        const string source = """
            using System;
            using System.Linq;
            using ExpressiveSharp;
            namespace Test
            {
                class C
                {
                    void M(IExpressiveQueryable<int> q)
                    {
                        q.Where(x => x > 0);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0026"),
            "Should not report EXP0026 when using ExpressiveSharp is present");
    }

    [TestMethod]
    public async Task Where_OnPlainQueryable_NoDiagnostic()
    {
        const string source = """
            using System;
            using System.Linq;
            namespace Test
            {
                class C
                {
                    void M(IQueryable<int> q)
                    {
                        q.Where(x => x > 0);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0026" || d.Id == "EXP0027"),
            "Should not report any diagnostic for plain IQueryable");
    }

    [TestMethod]
    public async Task Contains_WithoutStub_ReportsEXP0027()
    {
        const string source = """
            using System;
            using System.Linq;
            namespace Test
            {
                class C
                {
                    void M(ExpressiveSharp.IExpressiveQueryable<int> q)
                    {
                        q.Contains(42);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0027"),
            "Expected EXP0027 for Contains (no stub exists)");
        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0026"),
            "Should not report EXP0026 when no stub exists");
    }

    [TestMethod]
    public async Task Contains_OnPlainQueryable_NoDiagnostic()
    {
        const string source = """
            using System;
            using System.Linq;
            namespace Test
            {
                class C
                {
                    void M(IQueryable<int> q)
                    {
                        q.Contains(42);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0026" || d.Id == "EXP0027"),
            "Should not report any diagnostic for plain IQueryable");
    }

    [TestMethod]
    public async Task PreBuiltExpressionTree_WithUsingPresent_NoEXP0026()
    {
        const string source = """
            using System;
            using System.Linq;
            using System.Linq.Expressions;
            using ExpressiveSharp;
            namespace Test
            {
                class C
                {
                    void M(IExpressiveQueryable<int> q)
                    {
                        Expression<Func<int, bool>> pred = x => x > 0;
                        q.Where(pred);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0026"), string.Join("; ", diagnostics));
    }

    private async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
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

        var analyzer = new MissingExpressiveImportAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }
}
