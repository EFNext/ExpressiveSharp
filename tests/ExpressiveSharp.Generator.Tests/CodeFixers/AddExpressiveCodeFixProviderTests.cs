using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressiveSharp.CodeFixers;
using ExpressiveSharp.Generator.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Generator.Tests.CodeFixers;

[TestClass]
public sealed class AddExpressiveCodeFixProviderTests : GeneratorTestBase
{
    [TestMethod]
    public async Task AddsExpressiveAttribute_ToMethodDeclaration()
    {
        const string source = """
            using ExpressiveSharp;
            namespace Foo
            {
                class C
                {
                    public static int Helper(int x) => x * x;

                    [Expressive]
                    public int Computed => Helper(42);
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        TestContext.WriteLine("ACTUAL OUTPUT:");
        TestContext.WriteLine(fixedSource);
        TestContext.WriteLine("END OUTPUT");

        // The code fix should add [Expressive] to the Helper method declaration
        var lines = fixedSource.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();
        var helperLine = System.Array.FindIndex(lines, l => l.Contains("public static int Helper"));
        Assert.IsTrue(helperLine > 0, "Should find Helper method in output");
        Assert.IsTrue(lines[helperLine - 1].Trim() == "[Expressive]",
            $"Expected [Expressive] on line before Helper, but got: '{lines[helperLine - 1].Trim()}'");
    }

    [TestMethod]
    public async Task AddsExpressiveAttribute_ToPropertyDeclaration()
    {
        const string source = """
            using ExpressiveSharp;
            namespace Foo
            {
                class C
                {
                    public int Value => 42;

                    [Expressive]
                    public int Doubled => Value * 2;
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        var lines = fixedSource.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();
        var valueLine = System.Array.FindIndex(lines, l => l.Contains("public int Value"));
        Assert.IsTrue(valueLine > 0, "Should find Value property in output");
        Assert.IsTrue(lines[valueLine - 1].Trim() == "[Expressive]",
            $"Expected [Expressive] on line before Value, but got: '{lines[valueLine - 1].Trim()}'");
    }

    [TestMethod]
    public async Task AddsUsingDirective_WhenMissing()
    {
        const string declSource = """
            namespace Foo
            {
                class C
                {
                    public int Helper(int x) => x * x;
                }
            }
            """;

        const string callerSource = """
            using ExpressiveSharp;
            namespace Bar
            {
                class D
                {
                    [Expressive]
                    public int Computed(Foo.C c) => c.Helper(42);
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(declSource, callerSource, "Helper.cs", "Caller.cs");

        Assert.IsTrue(fixedSource.Contains("using ExpressiveSharp;"),
            "Expected using ExpressiveSharp to be added");
        Assert.IsTrue(fixedSource.Contains("[Expressive]"),
            "Expected [Expressive] attribute to be added");
    }

    [TestMethod]
    public async Task DoesNotAddDuplicateUsing_WhenAlreadyPresent()
    {
        const string source = """
            using ExpressiveSharp;
            namespace Foo
            {
                class C
                {
                    public static int Helper(int x) => x * x;

                    [Expressive]
                    public int Computed => Helper(42);
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        var usingCount = fixedSource.Split("using ExpressiveSharp;").Length - 1;
        Assert.AreEqual(1, usingCount,
            "Expected exactly one 'using ExpressiveSharp;' directive");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<string> ApplyCodeFixAsync(
        string declSource,
        string? callerSource = null,
        string declFileName = "TestFile.cs",
        string callerFileName = "Caller.cs")
    {
        // 1. Build workspace with documents
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

        // Add global usings
        var globalUsingsDoc = workspace.AddDocument(project.Id, "GlobalUsings.cs",
            SourceText.From("""
                global using System;
                global using System.Collections.Generic;
                global using System.Linq;
                global using ExpressiveSharp;
                """));
        project = globalUsingsDoc.Project;

        // Add declaration source
        var declDoc = workspace.AddDocument(project.Id, declFileName, SourceText.From(declSource));
        project = declDoc.Project;

        // Add caller source if provided (for cross-file tests)
        if (callerSource is not null)
        {
            var callerDoc = workspace.AddDocument(project.Id, callerFileName, SourceText.From(callerSource));
            project = callerDoc.Project;
        }

        // 2. Get compilation and run generator
        var compilation = await project.GetCompilationAsync()
            ?? throw new System.Exception("Failed to get compilation");

        var generator = new global::ExpressiveSharp.Generator.ExpressiveGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generator)
            .WithUpdatedParseOptions(parseOptions);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);
        var result = driver.GetRunResult();

        var diagnostic = result.Diagnostics.FirstOrDefault(d => d.Id == "EXP0013");
        Assert.IsNotNull(diagnostic, "Expected EXP0013 diagnostic to be emitted");
        Assert.IsTrue(diagnostic.AdditionalLocations.Count > 0,
            "Expected additional location (declaration) on EXP0013");

        // 3. Find the document containing the diagnostic usage
        var usageTree = diagnostic.Location.SourceTree;
        Assert.IsNotNull(usageTree, "Diagnostic should have a source tree");

        var usageDoc = project.Solution.GetDocument(usageTree);
        Assert.IsNotNull(usageDoc, "Should find workspace document for diagnostic location");

        // 4. Apply the code fix
        var codeFix = new AddExpressiveCodeFixProvider();
        var actions = new System.Collections.Generic.List<CodeAction>();
        var context = new CodeFixContext(
            usageDoc,
            diagnostic,
            (action, _) => actions.Add(action),
            CancellationToken.None);

        await codeFix.RegisterCodeFixesAsync(context);
        Assert.IsTrue(actions.Count > 0, "Expected at least one code fix action");

        var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
        var applyOperation = operations.OfType<ApplyChangesOperation>().First();
        var fixedSolution = applyOperation.ChangedSolution;

        // 5. Get the fixed declaration document
        var declTree = diagnostic.AdditionalLocations[0].SourceTree;
        Assert.IsNotNull(declTree, "Declaration should have a source tree");

        var declDocInSolution = project.Solution.GetDocument(declTree);
        Assert.IsNotNull(declDocInSolution, "Should find declaration document in original solution");

        var fixedDeclDoc = fixedSolution.GetDocument(declDocInSolution.Id);
        Assert.IsNotNull(fixedDeclDoc, "Should find fixed declaration document");

        return (await fixedDeclDoc.GetTextAsync()).ToString();
    }
}
