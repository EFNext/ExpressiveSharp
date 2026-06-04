using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

// Guards that a cross-file edit doesn't serve stale generated output (and that an unrelated edit
// doesn't churn it). See the comparer history in ExpressiveGenerator for why.
[TestClass]
public class IncrementalCachingTests : GeneratorTestBase
{
    private const string MemberSource = """
        using ExpressiveSharp;
        namespace App
        {
            public static class Q
            {
                [Expressive]
                public static long Get(Thing t) => t.Number;
            }
        }
        """;

    // v1: Number is int  -> body widens int->long, so the emitter inserts an Expression.Convert.
    private const string ModelsIntVersion = """
        namespace App { public class Thing { public int Number { get; set; } } }
        """;

    // v2: Number is long -> body is already long, so NO Expression.Convert is emitted.
    private const string ModelsLongVersion = """
        namespace App { public class Thing { public long Number { get; set; } } }
        """;

    // An unrelated edit to the models file that does not change Get's generated output.
    private const string ModelsIntVersionWithUnrelatedMember = """
        namespace App { public class Thing { public int Number { get; set; } public int Other { get; set; } } }
        """;

    private static GeneratorDriver CreateDriver() => CSharpGeneratorDriver.Create(
        new[] { new global::ExpressiveSharp.Generator.ExpressiveGenerator().AsSourceGenerator() },
        driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

    private CSharpCompilation CreateCompilation(SyntaxTree memberTree, SyntaxTree modelsTree) =>
        CSharpCompilation.Create(
            "compilation",
            new[] { memberTree, modelsTree },
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private static string GetMemberSourceText(GeneratorDriverRunResult run) =>
        run.GeneratedTrees.Single(t => t.FilePath.Contains("Get_P0_App_Thing")).GetText().ToString();

    [TestMethod]
    public void CrossFileTypeChange_IncrementalOutputMatchesFreshRun()
    {
        var memberTree = CSharpSyntaxTree.ParseText(MemberSource, path: "Member.cs");
        var modelsV1 = CSharpSyntaxTree.ParseText(ModelsIntVersion, path: "Models.cs");

        var c1 = CreateCompilation(memberTree, modelsV1);
        var driver = CreateDriver().RunGenerators(c1);

        // Edit ONLY Models.cs (Member.cs's tree reference is preserved, so a naive comparer would
        // treat this member as unchanged).
        var modelsV2 = CSharpSyntaxTree.ParseText(ModelsLongVersion, path: "Models.cs");
        var c2 = c1.ReplaceSyntaxTree(modelsV1, modelsV2);

        driver = driver.RunGenerators(c2);
        var incremental = GetMemberSourceText(driver.GetRunResult());

        // Ground truth: a fresh driver on the final compilation.
        var fresh = GetMemberSourceText(CreateDriver().RunGenerators(c2).GetRunResult());

        TestContext.WriteLine("Incremental:\n" + incremental);
        TestContext.WriteLine("Fresh:\n" + fresh);

        Assert.AreEqual(fresh, incremental,
            "After a cross-file type change, incremental output must match a from-scratch run (no stale cache).");
        // Sanity: the change really did alter the output (int->long removes the Convert), so the test
        // is actually exercising staleness rather than comparing two identical strings.
        StringAssert.Contains(incremental, "long", "Expected the long-typed property to be reflected.");
        Assert.IsFalse(incremental.Contains("Convert"),
            "With Number typed as long, no widening Convert should remain in the generated tree.");
    }

    [TestMethod]
    public void UnrelatedCrossFileEdit_DoesNotInvalidateGeneratedSources()
    {
        var memberTree = CSharpSyntaxTree.ParseText(MemberSource, path: "Member.cs");
        var modelsV1 = CSharpSyntaxTree.ParseText(ModelsIntVersion, path: "Models.cs");

        var c1 = CreateCompilation(memberTree, modelsV1);
        var driver = CreateDriver().RunGenerators(c1);
        var text1 = GetMemberSourceText(driver.GetRunResult());

        // Add an unrelated member to Thing — Get's generated output is unaffected.
        var modelsV1b = CSharpSyntaxTree.ParseText(ModelsIntVersionWithUnrelatedMember, path: "Models.cs");
        var c2 = c1.ReplaceSyntaxTree(modelsV1, modelsV1b);

        driver = driver.RunGenerators(c2);
        var run2 = driver.GetRunResult();

        Assert.AreEqual(text1, GetMemberSourceText(run2),
            "Precondition: the unrelated edit should not change Get's generated text.");

        // Value unchanged => step reason Unchanged => downstream AddSource isn't re-run (no churn).
        var sourceSteps = run2.Results
            .Single()
            .TrackedSteps[global::ExpressiveSharp.Generator.ExpressiveGenerator.ExpressiveSourcesTrackingName];

        foreach (var step in sourceSteps)
        {
            foreach (var output in step.Outputs)
            {
                Assert.AreEqual(IncrementalStepRunReason.Unchanged, output.Reason,
                    "Value-gating should mark the generated source 'Unchanged' for an unrelated cross-file edit.");
            }
        }
    }
}
