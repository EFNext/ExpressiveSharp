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
public sealed class OverrideMissingExpressiveAnalyzerTests : GeneratorTestBase
{
    [TestMethod]
    public async Task OverrideProperty_MissingExpressive_WarnsEXP0032()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class Animal {
                    public string Name { get; set; }
                    [Expressive] public virtual string Description => "Animal: " + Name;
                }
                class Dog : Animal {
                    public override string Description => "Dog: " + Name;
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0032"),
            "Expected EXP0032 for an override of an [Expressive] property that is missing [Expressive]");
    }

    [TestMethod]
    public async Task OverrideMethod_MissingExpressive_WarnsEXP0032()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class Animal {
                    public string Name { get; set; }
                    [Expressive] public virtual string Describe() => "Animal: " + Name;
                }
                class Dog : Animal {
                    public override string Describe() => "Dog: " + Name;
                }
            }
            """);

        Assert.IsTrue(diagnostics.Any(d => d.Id == "EXP0032"),
            "Expected EXP0032 for an override of an [Expressive] method that is missing [Expressive]");
    }

    [TestMethod]
    public async Task Override_WithExpressive_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class Animal {
                    public string Name { get; set; }
                    [Expressive] public virtual string Description => "Animal: " + Name;
                }
                class Dog : Animal {
                    [Expressive] public override string Description => "Dog: " + Name;
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0032"),
            "An override that is itself [Expressive] participates in dispatch and must not warn");
    }

    [TestMethod]
    public async Task Override_OfNonExpressiveBase_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class Animal {
                    public string Name { get; set; }
                    public virtual string Description => "Animal: " + Name;
                }
                class Dog : Animal {
                    public override string Description => "Dog: " + Name;
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0032"),
            "Overriding a non-[Expressive] member is unrelated to expansion and must not warn");
    }

    [TestMethod]
    public async Task Override_WithNotExpressive_NoWarning()
    {
        var diagnostics = await RunAnalyzerAsync(
            """
            namespace Foo {
                class Animal {
                    public string Name { get; set; }
                    [Expressive] public virtual string Description => "Animal: " + Name;
                }
                class Dog : Animal {
                    [NotExpressive] public override string Description => "Dog: " + Name;
                }
            }
            """);

        Assert.IsFalse(diagnostics.Any(d => d.Id == "EXP0032"),
            "[NotExpressive] is the explicit opt-out and must silence EXP0032");
    }

    [TestMethod]
    public async Task CodeFix_AddsExpressive_ToOverride()
    {
        const string source = """
            using ExpressiveSharp;
            namespace Foo
            {
                class Animal
                {
                    public string Name { get; set; }
                    [Expressive] public virtual string Description => "Animal: " + Name;
                }

                class Dog : Animal
                {
                    public override string Description => "Dog: " + Name;
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        var lines = fixedSource.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();
        var overrideLine = System.Array.FindIndex(lines, l => l.Contains("public override string Description"));
        Assert.IsTrue(overrideLine > 0, "Should find the overriding property in output");
        Assert.IsTrue(lines[overrideLine - 1].Trim() == "[Expressive]",
            $"Expected [Expressive] on the line before the override, but got: '{lines[overrideLine - 1].Trim()}'");
    }

    private async Task<ImmutableArray<Diagnostic>> RunAnalyzerAsync(string source)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        var compilation = CSharpCompilation.Create(
            "TestCompilation",
            new[]
            {
                CSharpSyntaxTree.ParseText(
                    """
                    global using System;
                    global using ExpressiveSharp;
                    """, parseOptions, "GlobalUsings.cs"),
                CSharpSyntaxTree.ParseText(source, parseOptions, "TestFile.cs"),
            },
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new OverrideMissingExpressiveAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }

    private async Task<string> ApplyCodeFixAsync(string source)
    {
        var workspace = new AdhocWorkspace();
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
        var doc = workspace.AddDocument(project.Id, "TestFile.cs", SourceText.From(source));
        project = doc.Project;

        var compilation = await project.GetCompilationAsync()
            ?? throw new System.Exception("Failed to get compilation");

        var analyzer = new OverrideMissingExpressiveAnalyzer();
        var withAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        var diagnostic = diagnostics.FirstOrDefault(d => d.Id == "EXP0032");
        Assert.IsNotNull(diagnostic, "Expected EXP0032 diagnostic to be emitted");

        var fixDoc = project.Solution.GetDocument(diagnostic.Location.SourceTree);
        Assert.IsNotNull(fixDoc, "Should find workspace document for diagnostic location");

        var codeFix = new AddExpressiveCodeFixProvider();
        var actions = new List<CodeAction>();
        var context = new CodeFixContext(fixDoc, diagnostic, (action, _) => actions.Add(action), CancellationToken.None);
        await codeFix.RegisterCodeFixesAsync(context);
        Assert.IsTrue(actions.Count > 0, "Expected at least one code fix action");

        var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
        var apply = operations.OfType<ApplyChangesOperation>().First();
        var fixedDoc = apply.ChangedSolution.GetDocument(fixDoc.Id);
        Assert.IsNotNull(fixedDoc, "Should find fixed document");

        return (await fixedDoc.GetTextAsync()).ToString();
    }
}
