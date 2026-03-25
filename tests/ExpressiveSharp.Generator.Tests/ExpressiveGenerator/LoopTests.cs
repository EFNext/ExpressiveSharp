using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class LoopTests : GeneratorTestBase
{
    [TestMethod]
    public void ForEach_EnabledByDefault_NoDiagnostics()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Sum(List<int> items)
                    {
                        var sum = 0;
                        foreach (var x in items) { sum += x; }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            "Loops are enabled by default — no diagnostics expected");
    }

    [TestMethod]
    public Task ForEach_WithLoopDisableFlag_StillEmitsLoop()
    {
        // The Disable=Loop flag was a legacy concept for blocking loop-to-LINQ conversion.
        // The emitter now emits loops as-is via IOperation — the disable flag has no effect.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Sum(List<int> items)
                    {
                        var sum = 0;
                        foreach (var x in items) { sum += x; }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task WhileLoop_EmitsBlockExpression()
    {
        // While loops emit valid expression trees but warn that they can't be converted to LINQ.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Countdown(int n)
                    {
                        var result = 0;
                        while (n > 0) { result += n; n--; }
                        return result;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0006"),
            "Expected EXP0006 warning for while loop");
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_Sum()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Sum(List<int> items)
                    {
                        var sum = 0;
                        foreach (var x in items) { sum += x; }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_SumWithSelector()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Item {
                    public int Value { get; set; }
                }
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Sum(List<Item> items)
                    {
                        var sum = 0;
                        foreach (var x in items) { sum += x.Value; }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_CountWithPredicate()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int CountPositive(List<int> items)
                    {
                        var count = 0;
                        foreach (var x in items) { if (x > 0) count++; }
                        return count;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_CountAll()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int CountAll(List<int> items)
                    {
                        var count = 0;
                        foreach (var x in items) { count++; }
                        return count;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_Min()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int FindMin(List<int> items)
                    {
                        var min = int.MaxValue;
                        foreach (var x in items) { min = Math.Min(min, x); }
                        return min;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_Any()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public bool HasNegative(List<int> items)
                    {
                        var found = false;
                        foreach (var x in items) { if (x < 0) found = true; }
                        return found;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_All()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public bool AllPositive(List<int> items)
                    {
                        var all = true;
                        foreach (var x in items) { if (!(x > 0)) all = false; }
                        return all;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_Aggregate()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Product(List<int> items)
                    {
                        var product = 1;
                        foreach (var x in items) { product = product * x; }
                        return product;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForLoop_ArraySum()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Sum(int[] arr)
                    {
                        var sum = 0;
                        for (var i = 0; i < arr.Length; i++) { sum += arr[i]; }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0006"),
            "Expected EXP0006 warning for for loop");
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_ConditionalSum()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int SumEven(List<int> items)
                    {
                        var sum = 0;
                        foreach (var i in items) { if (i % 2 == 0) { sum += i; } }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_ConditionalAggregate()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int ProductOfPositive(List<int> items)
                    {
                        var product = 1;
                        foreach (var x in items) { if (x > 0) product = product * x; }
                        return product;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_SumWithMethodCall()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public static int Transform(int x) => x * x;

                    [Expressive(AllowBlockBody = true)]
                    public int SumOfSquares(List<int> items)
                    {
                        var sum = 0;
                        foreach (var x in items) { sum += Transform(x); }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_SumWithChainedMemberAccess()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Inner { public int Score { get; set; } }
                class Outer { public Inner Inner { get; set; } }
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Sum(List<Outer> items)
                    {
                        var sum = 0;
                        foreach (var x in items) { sum += x.Inner.Score; }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_SelectWithMethodCall()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public List<string> Names(List<int> items)
                    {
                        var result = new List<string>();
                        foreach (var x in items) { result.Add(x.ToString()); }
                        return result;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_ConditionalSelectWithMethodCall()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Item { public int Value { get; set; } public string Name { get; set; } }
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public List<string> NamesOfExpensive(List<Item> items)
                    {
                        var result = new List<string>();
                        foreach (var x in items) { if (x.Value > 100) result.Add(x.Name); }
                        return result;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_ConditionalMinWithMemberAccess()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Item { public int Value { get; set; } }
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int MinPositive(List<Item> items)
                    {
                        var min = int.MaxValue;
                        foreach (var x in items) { if (x.Value > 0) min = Math.Min(min, x.Value); }
                        return min;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ForEach_InstanceMethodInBody()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Multiplier { get; set; }

                    [Expressive(AllowBlockBody = true)]
                    public int WeightedSum(List<int> items)
                    {
                        var sum = 0;
                        foreach (var x in items) { sum += x * Multiplier; }
                        return sum;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length,
            string.Join("\n", result.Diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
