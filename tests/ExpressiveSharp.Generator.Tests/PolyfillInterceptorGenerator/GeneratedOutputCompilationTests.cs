using System.Collections.Generic;
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
    public void CollidingFileTags_SameCallPosition_InterceptorsCompile()
    {
        var (pathA, pathB) = FindCollidingPaths();
        TestContext.WriteLine($"Colliding paths: {pathA} / {pathB} (tag 0x{Fnv1a(pathA) & 0xFFFF:x4})");

        const string sharedSource =
            """
            namespace Shared { public class Order { public string Tag { get; set; } } }
            """;
        const string sourceTemplate =
            """
            using ExpressiveSharp;
            using Shared;

            namespace {0}
            {{
                class TestClass
                {{
                    public void Run(System.Linq.IQueryable<Order> query)
                    {{
                        query.AsExpressive().Where(o => o.Tag != null).ToList();
                    }}
                }}
            }}
            """;

        var compilation = CreateCompilation(
            (sharedSource, "Shared.cs"),
            (string.Format(sourceTemplate, "NsA"), pathA),
            (string.Format(sourceTemplate, "NsB"), pathB));

        var (result, output) = RunInterceptorGenerator(compilation);

        Assert.AreEqual(2, result.GeneratedTrees.Length, "Expected one interceptor file per source file.");

        var errors = output.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.AreEqual(0, errors.Count,
            "Interceptors from files with colliding 16-bit path tags must not clash. Errors:\n"
            + FormatErrors(output));
    }

    private static uint Fnv1a(string s)
    {
        unchecked
        {
            var hash = 2166136261u;
            for (var i = 0; i < s.Length; i++)
            {
                hash ^= (uint)s[i];
                hash *= 16777619u;
            }
            return hash;
        }
    }

    private static (string PathA, string PathB) FindCollidingPaths()
    {
        var seen = new Dictionary<uint, (string Path, uint Hash)>();
        for (var i = 0; ; i++)
        {
            var path = $"File{i}.cs";
            var hash = Fnv1a(path);
            var tag = hash & 0xFFFFu;
            if (seen.TryGetValue(tag, out var other) && other.Hash != hash)
            {
                return (other.Path, path);
            }
            seen[tag] = (path, hash);
        }
    }
}
