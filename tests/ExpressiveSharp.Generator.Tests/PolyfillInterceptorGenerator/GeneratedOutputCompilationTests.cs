using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class GeneratedOutputCompilationTests : GeneratorTestBase
{
    private (GeneratorDriverRunResult Result, Compilation Output) RunInterceptorGenerator(Compilation compilation)
    {
        var subject = new global::ExpressiveSharp.Generator.PolyfillInterceptorGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(subject)
            .WithUpdatedParseOptions((CSharpParseOptions)compilation.SyntaxTrees.First().Options);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var result = driver.GetRunResult();

        foreach (var tree in result.GeneratedTrees)
        {
            TestContext.WriteLine($"Generated: {tree.FilePath}");
            TestContext.WriteLine(tree.GetText().ToString());
        }

        return (result, outputCompilation);
    }

    private static string FormatErrors(Compilation output) =>
        string.Join("\n", output.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.ToString()));

    [TestMethod]
    public void AnonymousType_NestedTwoGenericLevels_InterceptorCompiles()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Select(o => new { o.Tag })
                             .GroupBy(a => a.Tag)
                             .Select(g => g.Count())
                             .ToList();
                    }
                }
            }
            """;

        var (result, output) = RunInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length, "Expected the interceptor file to be generated.");
        Assert.IsFalse(result.Diagnostics.Any(d => d.Id == "EXP0010"),
            "No call site should be dropped: " + string.Join("; ", result.Diagnostics));

        var errors = output.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.AreEqual(0, errors.Count,
            "Interceptor output must compile. Errors:\n" + FormatErrors(output));
    }
}
