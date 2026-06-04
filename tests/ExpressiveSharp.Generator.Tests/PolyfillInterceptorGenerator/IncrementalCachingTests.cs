using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

// Guards that a cross-file edit doesn't serve stale interceptors (and that an unrelated edit
// doesn't churn them). An interceptor is built by binding a file's call sites, which depend on
// the whole compilation — not just that file's syntax.
[TestClass]
public class IncrementalCachingTests : GeneratorTestBase
{
    private const string QuerySource = """
        using System;
        using System.Linq.Expressions;
        using ExpressiveSharp;
        namespace TestNs
        {
            class Q
            {
                public void Run()
                {
                    Expression<Func<Order, long>> e = ExpressionPolyfill.Create<Func<Order, long>>(o => o.Value);
                }
            }
        }
        """;

    private const string ModelsInt = "namespace TestNs { class Order { public int Value { get; set; } } }";
    private const string ModelsLong = "namespace TestNs { class Order { public long Value { get; set; } } }";
    private const string ModelsIntWithUnrelated =
        "namespace TestNs { class Order { public int Value { get; set; } public int Other { get; set; } } }";

    private static GeneratorDriver CreateDriver(CSharpParseOptions parseOptions) => CSharpGeneratorDriver
        .Create(
            new[] { new global::ExpressiveSharp.Generator.PolyfillInterceptorGenerator().AsSourceGenerator() },
            driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true))
        .WithUpdatedParseOptions(parseOptions);

    private static string InterceptorText(GeneratorDriverRunResult run) =>
        string.Join("\n", run.GeneratedTrees.Select(t => t.GetText().ToString()));

    [TestMethod]
    public void CrossFileTypeChange_IncrementalOutputMatchesFreshRun()
    {
        var c1 = CreateCompilation(new[] { QuerySource, ModelsInt });
        var parseOptions = (CSharpParseOptions)c1.SyntaxTrees.First().Options;
        var modelsV1 = c1.SyntaxTrees.First(t => t.ToString().Contains("class Order"));
        var modelsV2 = CSharpSyntaxTree.ParseText(ModelsLong, parseOptions, modelsV1.FilePath);
        var c2 = c1.ReplaceSyntaxTree(modelsV1, modelsV2);

        var driver = CreateDriver(parseOptions).RunGenerators(c1).RunGenerators(c2);
        var incremental = InterceptorText(driver.GetRunResult());

        var fresh = InterceptorText(CreateDriver(parseOptions).RunGenerators(c2).GetRunResult());

        Assert.AreEqual(fresh, incremental,
            "After a cross-file type change, interceptor output must match a from-scratch run.");
        Assert.IsFalse(incremental.Contains("Convert"),
            "With Value typed as long, no widening Convert should remain in the interceptor.");
    }

    [TestMethod]
    public void UnrelatedCrossFileEdit_DoesNotInvalidateInterceptor()
    {
        var c1 = CreateCompilation(new[] { QuerySource, ModelsInt });
        var parseOptions = (CSharpParseOptions)c1.SyntaxTrees.First().Options;
        var modelsV1 = c1.SyntaxTrees.First(t => t.ToString().Contains("class Order"));
        var modelsV1b = CSharpSyntaxTree.ParseText(ModelsIntWithUnrelated, parseOptions, modelsV1.FilePath);
        var c2 = c1.ReplaceSyntaxTree(modelsV1, modelsV1b);

        var driver = CreateDriver(parseOptions).RunGenerators(c1);
        var text1 = InterceptorText(driver.GetRunResult());

        driver = driver.RunGenerators(c2);
        var run2 = driver.GetRunResult();

        Assert.AreEqual(text1, InterceptorText(run2),
            "Precondition: the unrelated edit should not change the interceptor text.");

        var steps = run2.Results
            .Single()
            .TrackedSteps[global::ExpressiveSharp.Generator.PolyfillInterceptorGenerator.InterceptorSourcesTrackingName];

        // Cached (input unchanged) or Unchanged (re-ran, equal value) both mean "not re-emitted";
        // only New/Modified would cause downstream churn.
        foreach (var step in steps)
        {
            foreach (var output in step.Outputs)
            {
                Assert.IsTrue(
                    output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged,
                    $"Expected the interceptor source to be gated (Cached/Unchanged) but was {output.Reason}.");
            }
        }
    }
}
