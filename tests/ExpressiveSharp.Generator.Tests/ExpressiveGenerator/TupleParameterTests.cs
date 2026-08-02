using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class TupleParameterTests : GeneratorTestBase
{
    [TestMethod]
    public void TupleParameter_GeneratedCodeCompiles()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public static int First((int, int) pair) => pair.Item1;
                }
            }
            """);

        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
    }

    [TestMethod]
    public void NestedTupleParameter_GeneratedCodeCompiles()
    {
        var compilation = CreateCompilation(
            """
            using System.Collections.Generic;
            namespace Foo {
                class C {
                    [Expressive]
                    public static int CountPairs(List<(int A, string B)> pairs) => pairs.Count;
                }
            }
            """);

        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.GeneratedTrees.Length);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
    }
}
