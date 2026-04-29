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
public sealed class NotExpressiveOptOutTests : GeneratorTestBase
{
    [TestMethod]
    public async Task NotExpressive_OnReferencedMember_SuppressesEXP0013()
    {
        const string source = """
            using ExpressiveSharp;
            namespace Test
            {
                class C
                {
                    [NotExpressive]
                    public static int Helper(int x) => x * x;

                    [Expressive]
                    public int Computed => Helper(42);
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0013"),
            "[NotExpressive] should suppress EXP0013 on the referenced member");
    }

    [TestMethod]
    public async Task NoOptOut_StillReportsEXP0013()
    {
        const string source = """
            using ExpressiveSharp;
            namespace Test
            {
                class C
                {
                    public static int Helper(int x) => x * x;

                    [Expressive]
                    public int Computed => Helper(42);
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0013"),
            "EXP0013 should still fire when [NotExpressive] is absent");
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

        var analyzer = new MissingExpressiveAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }
}
