using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressiveSharp.CodeFixers;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Generator.Tests.CodeFixers;

/// <summary>
/// Verifies the code fix for EXP0024 rewrites a nullable <c>[Expressive(Projectable = true)]</c>
/// property from the coalesce shape to the ternary + has-value-flag shape, inserting a private
/// <c>_has&lt;PropertyName&gt;</c> bool field alongside.
/// </summary>
[TestClass]
public sealed class ConvertProjectableToTernaryCodeFixProviderTests : GeneratorTestBase
{
    [TestMethod]
    public async Task ConvertsNullableReferenceProperty_FieldKeyword_ExpressionBodied()
    {
        const string source = """
            #nullable enable
            using ExpressiveSharp;
            namespace Foo
            {
                partial class User
                {
                    public string? Name { get; set; }

                    [Expressive(Projectable = true)]
                    public string? UpperName
                    {
                        get => field ?? (Name != null ? Name.ToUpper() : "(UNNAMED)");
                        init => field = value;
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        StringAssert.Contains(fixedSource, "private bool _hasUpperName;",
            "Expected private bool flag field to be inserted");
        StringAssert.Contains(fixedSource,
            "get => _hasUpperName ? field : (Name != null ? Name.ToUpper() : \"(UNNAMED)\");",
            "Expected get accessor to be rewritten to the ternary form");
        StringAssert.Contains(fixedSource, "_hasUpperName = true;",
            "Expected init accessor to set the flag");
        StringAssert.Contains(fixedSource, "field = value;",
            "Expected init accessor to still assign the backing field");
    }

    [TestMethod]
    public async Task ConvertsNullableValueProperty_FieldKeyword_ExpressionBodied()
    {
        const string source = """
            using ExpressiveSharp;
            namespace Foo
            {
                partial class Account
                {
                    public decimal? TotalAmount { get; set; }

                    [Expressive(Projectable = true)]
                    public decimal? Amount
                    {
                        get => field ?? (TotalAmount ?? 0m);
                        init => field = value;
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        StringAssert.Contains(fixedSource, "private bool _hasAmount;");
        StringAssert.Contains(fixedSource, "get => _hasAmount ? field : (TotalAmount ?? 0m);");
        StringAssert.Contains(fixedSource, "_hasAmount = true;");
        StringAssert.Contains(fixedSource, "field = value;");
    }

    [TestMethod]
    public async Task ConvertsNullableProperty_ManualBackingField()
    {
        // Manual backing field: the fixer must reference `_fullName`, not `field`, in the ternary's
        // true-branch, and the setter must still assign to `_fullName`.
        const string source = """
            #nullable enable
            using ExpressiveSharp;
            namespace Foo
            {
                class User
                {
                    public string? FirstName { get; set; }
                    public string? LastName  { get; set; }

                    private string? _fullName;

                    [Expressive(Projectable = true)]
                    public string? FullName
                    {
                        get => _fullName ?? (FirstName + " " + LastName);
                        init => _fullName = value;
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        StringAssert.Contains(fixedSource, "private bool _hasFullName;");
        StringAssert.Contains(fixedSource, "get => _hasFullName ? _fullName : (FirstName + \" \" + LastName);");
        StringAssert.Contains(fixedSource, "_hasFullName = true;");
        StringAssert.Contains(fixedSource, "_fullName = value;");
    }

    [TestMethod]
    public async Task ConvertsNullableProperty_SetAccessor()
    {
        // `set` instead of `init` should be rewritten the same way.
        const string source = """
            #nullable enable
            using ExpressiveSharp;
            namespace Foo
            {
                partial class User
                {
                    public string? Name { get; set; }

                    [Expressive(Projectable = true)]
                    public string? UpperName
                    {
                        get => field ?? (Name != null ? Name.ToUpper() : "");
                        set => field = value;
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        StringAssert.Contains(fixedSource, "private bool _hasUpperName;");
        StringAssert.Contains(fixedSource, "get => _hasUpperName ? field : (Name != null ? Name.ToUpper() : \"\");");
        StringAssert.Contains(fixedSource, "_hasUpperName = true;");
    }

    [TestMethod]
    public async Task PicksUniqueFlagName_WhenHasPropertyNameAlreadyDefined()
    {
        // The containing type already declares `_hasAmount`, so the fixer must pick a
        // non-colliding name (`_hasAmount1`) instead of producing uncompilable code.
        const string source = """
            using ExpressiveSharp;
            namespace Foo
            {
                partial class Account
                {
                    public decimal? TotalAmount { get; set; }
                    private int _hasAmount;

                    [Expressive(Projectable = true)]
                    public decimal? Amount
                    {
                        get => field ?? (TotalAmount ?? 0m);
                        init => field = value;
                    }
                }
            }
            """;

        var fixedSource = await ApplyCodeFixAsync(source);

        StringAssert.Contains(fixedSource, "private bool _hasAmount1;");
        StringAssert.Contains(fixedSource, "get => _hasAmount1 ? field : (TotalAmount ?? 0m);");
        StringAssert.Contains(fixedSource, "_hasAmount1 = true;");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<string> ApplyCodeFixAsync(string source)
    {
        using var workspace = new AdhocWorkspace();
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

        // Run the generator (it's what reports EXP0024).
        var compilation = await document.Project.GetCompilationAsync()
            ?? throw new System.Exception("Failed to get compilation");

        var result = RunExpressiveGenerator(compilation);
        var diagnostic = result.Diagnostics.FirstOrDefault(d => d.Id == "EXP0024");
        Assert.IsNotNull(diagnostic, "Expected EXP0024 to be emitted by the generator");

        var codeFix = new ConvertProjectableToTernaryCodeFixProvider();
        var actions = new System.Collections.Generic.List<CodeAction>();
        var fixContext = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => actions.Add(action),
            CancellationToken.None);

        await codeFix.RegisterCodeFixesAsync(fixContext);
        Assert.IsTrue(actions.Count > 0, "Expected at least one code fix action");

        var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
        var applyOperation = operations.OfType<ApplyChangesOperation>().First();
        var fixedSolution = applyOperation.ChangedSolution;
        var fixedDocument = fixedSolution.GetDocument(document.Id)!;

        return (await fixedDocument.GetTextAsync()).ToString();
    }
}
