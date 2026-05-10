using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class ExpansionEdgeCasesTests
{
    [TestMethod]
    public void VirtualMethod_ExpansionPreservesPolymorphicDispatch()
    {
        var derived = new VirtualDispatchDerived { Id = 7, Name = "x" };
        var directCall = ((VirtualDispatchBase)derived).Describe();

        Expression<Func<VirtualDispatchBase, string>> expr = b => b.Describe();
        var expanded = (Expression<Func<VirtualDispatchBase, string>>)expr.ExpandExpressives();
        var fromExpansion = expanded.Compile()(derived);

        Assert.AreEqual(directCall, fromExpansion);
    }

    [TestMethod]
    public void Polyfill_TypePatternSwitch_WithNullArm_BuildsExpression()
    {
        var expr = ExpressionPolyfill.Create((object o) => o switch
        {
            int i => i + 1,
            string s => s.Length,
            null => -1,
            _ => 0,
        });

        var fn = expr.Compile();
        Assert.AreEqual(6, fn(5));
        Assert.AreEqual(5, fn("hello"));
        Assert.AreEqual(-1, fn(null!));
    }

    [TestMethod]
    public void Polyfill_LambdaCapturingTupleLocal_BuildsExpression()
    {
        var p = (X: 3, Y: 4);
        var expr = ExpressionPolyfill.Create((int n) => n + p.X + p.Y);
        Assert.AreEqual(8, expr.Compile()(1));
    }

    [TestMethod]
    public void Polyfill_LambdaWithListPattern_IsIntercepted()
    {
        var expr = ExpressionPolyfill.Create((int[] a) => a switch
        {
            [] => "empty",
            [_] => "one",
            _ => "many",
        });

        var fn = expr.Compile();
        Assert.AreEqual("empty", fn(Array.Empty<int>()));
        Assert.AreEqual("one", fn(new[] { 1 }));
        Assert.AreEqual("many", fn(new[] { 1, 2 }));
    }

    [Ignore("ExpressiveReplacer has no recursion guard; expansion currently throws StackOverflowException, which is uncatchable and terminates the test runner.")]
    [TestMethod]
    public void RecursiveExpressiveMember_ExpandsWithoutCrashingTheProcess()
    {
        Expression<Func<RecursiveTree, int>> expr = t => t.Sum;
        var expanded = expr.ExpandExpressives();
        Assert.IsNotNull(expanded);
    }
}

public class VirtualDispatchBase
{
    public int Id { get; set; }

    [Expressive]
    public virtual string Describe() => $"base#{Id}";
}

public class VirtualDispatchDerived : VirtualDispatchBase
{
    public string? Name { get; set; }

    [Expressive]
    public override string Describe() => $"derived#{Id}/{Name}";
}

public class RecursiveTree
{
    public int Value { get; set; }
    public RecursiveTree? Left { get; set; }
    public RecursiveTree? Right { get; set; }

    [Expressive]
    public int Sum => Value
        + (Left == null ? 0 : Left.Sum)
        + (Right == null ? 0 : Right.Sum);
}
