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
public sealed class PlainQueryableMissingAsExpressiveAnalyzerTests : GeneratorTestBase
{
    [TestMethod]
    public async Task PlainQueryable_WithExpressiveProperty_ReportsEXP0028()
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0028"),
            "Expected EXP0028 when plain IQueryable lambda references an [Expressive] member");
    }

    [TestMethod]
    public async Task ExpressiveQueryable_NoEXP0028()
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
                    void M(IExpressiveQueryable<User> users)
                    {
                        users.Where(u => u.IsAdult);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0028"),
            "EXP0028 should not fire when the receiver is already IExpressiveQueryable");
    }

    [TestMethod]
    public async Task PlainQueryable_NonExpressiveMember_NoEXP0028()
    {
        const string source = """
            using System.Linq;
            namespace Test
            {
                class User
                {
                    public int Age { get; set; }
                }

                class C
                {
                    void M(IQueryable<User> users)
                    {
                        users.Where(u => u.Age >= 18);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0028"),
            "EXP0028 should not fire when no [Expressive] member is referenced");
    }

    [TestMethod]
    public async Task PlainEnumerable_NoEXP0028()
    {
        const string source = """
            using System.Collections.Generic;
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
                    void M(IEnumerable<User> users)
                    {
                        users.Where(u => u.IsAdult);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0028"),
            "EXP0028 should not fire on IEnumerable (LINQ-to-Objects) chains");
    }

    [TestMethod]
    public async Task PlainQueryable_WithNotExpressiveMember_NoEXP0028()
    {
        const string source = """
            using System.Linq;
            using ExpressiveSharp;
            namespace Test
            {
                class User
                {
                    public int Age { get; set; }

                    [NotExpressive]
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0028"),
            "[NotExpressive] should suppress EXP0028");
    }

    [TestMethod]
    public async Task PlainQueryable_AfterAsExpressive_NoEXP0028()
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
                        users.AsExpressive().Where(u => u.IsAdult);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0028"),
            "EXP0028 should not fire when the chain is already wrapped with .AsExpressive()");
    }

    [TestMethod]
    public async Task PlainQueryable_ExpressiveMethodInLambda_ReportsEXP0028()
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
                    public bool IsOver(int min) => Age >= min;
                }

                class C
                {
                    void M(IQueryable<User> users)
                    {
                        users.Where(u => u.IsOver(21));
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0028"),
            "Expected EXP0028 when plain IQueryable lambda references an [Expressive] method");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

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

        var analyzer = new PlainQueryableMissingAsExpressiveAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }
}
