using System.Linq.Expressions;
using ExpressiveSharp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class ExpansionEdgeCasesTests
{
    [TestMethod]
    public void VirtualMethod_OnBaseReceiver_DispatchesPolymorphically()
    {
        // Static receiver is the base type; the body is chosen by the runtime type.
        Expression<Func<VirtualDispatchBase, string>> expr = b => b.Describe();
        var fn = ((Expression<Func<VirtualDispatchBase, string>>)expr.ExpandExpressives()).Compile();

        Assert.AreEqual("derived#7/x", fn(new VirtualDispatchDerived { Id = 7, Name = "x" }));
        Assert.AreEqual("base#3", fn(new VirtualDispatchBase { Id = 3 }));

        // The expansion is a runtime type-test, not a static inline.
        Assert.IsTrue(expr.ExpandExpressives().ToString().Contains("Is VirtualDispatchDerived", StringComparison.Ordinal),
            "Expected a runtime `is` type-test in the expansion. Got: " + expr.ExpandExpressives());
    }

    [TestMethod]
    public void OverrideProperty_OnDerivedReceiver_ExpandsOverrideBody()
    {
        // Receiver static type is itself the override declarer, so its body wins over the base slot.
        // ScoreDerived.Score => base.Score + 1 => (Id * 2) + 1.
        Expression<Func<ScoreDerived, int>> expr = d => d.Score;
        var expanded = (Expression<Func<ScoreDerived, int>>)expr.ExpandExpressives();

        Assert.AreEqual(11, expanded.Compile()(new ScoreDerived { Id = 5 }));
    }

    [TestMethod]
    public void OverrideProperty_OnBaseReceiver_DispatchesPolymorphically()
    {
        Expression<Func<ScoreBase, int>> expr = b => b.Score;
        var fn = ((Expression<Func<ScoreBase, int>>)expr.ExpandExpressives()).Compile();

        Assert.AreEqual(11, fn(new ScoreDerived { Id = 5 }));   // (5 * 2) + 1
        Assert.AreEqual(10, fn(new ScoreBase { Id = 5 }));      // 5 * 2
    }

    [TestMethod]
    public void OverrideMethod_OnDerivedReceiver_ExpandsOverrideBody()
    {
        // GreetDerived.Greet() => base.Greet() + 1 => (Id * 10) + 1.
        Expression<Func<GreetDerived, int>> expr = d => d.Greet();
        var expanded = (Expression<Func<GreetDerived, int>>)expr.ExpandExpressives();

        Assert.AreEqual(31, expanded.Compile()(new GreetDerived { Id = 3 }));
    }

    [TestMethod]
    public void MethodWithArgument_DispatchesPolymorphically()
    {
        Expression<Func<CalcBase, int>> expr = c => c.Calc(2);
        var fn = ((Expression<Func<CalcBase, int>>)expr.ExpandExpressives()).Compile();

        Assert.AreEqual(12, fn(new CalcDerived { Id = 6 }));   // Id * n
        Assert.AreEqual(8, fn(new CalcBase { Id = 6 }));       // Id + n
    }

    [TestMethod]
    public void MultiLevelHierarchy_PicksNearestOverride()
    {
        Expression<Func<Node, string>> expr = n => n.Kind;
        var fn = ((Expression<Func<Node, string>>)expr.ExpandExpressives()).Compile();

        Assert.AreEqual("leaf:1", fn(new Leaf { Id = 1 }));           // most-derived override
        Assert.AreEqual("branch:2", fn(new Branch { Id = 2 }));       // intermediate override
        Assert.AreEqual("branch:3", fn(new PlainBranch { Id = 3 }));  // inherits Branch's override
    }

    [TestMethod]
    public void DisablePolymorphicDispatch_RevertsToStaticBaseBody()
    {
        var options = new ExpressiveOptions();
        options.DisablePolymorphicDispatch();

        Expression<Func<VirtualDispatchBase, string>> expr = b => b.Describe();
        var fn = ((Expression<Func<VirtualDispatchBase, string>>)expr.ExpandExpressives(options)).Compile();

        Assert.AreEqual("base#7", fn(new VirtualDispatchDerived { Id = 7, Name = "x" }));
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

    [TestMethod]
    public void SelfRecursiveExpressiveMember_ExpandsOnceAndLeavesNestedReferenceIntact()
    {
        Expression<Func<RecursiveTree, int>> expr = t => t.Sum;
        var expanded = (Expression<Func<RecursiveTree, int>>)expr.ExpandExpressives();

        var tree = new RecursiveTree
        {
            Value = 1,
            Left = new RecursiveTree { Value = 2 },
            Right = new RecursiveTree
            {
                Value = 3,
                Right = new RecursiveTree { Value = 4 },
            },
        };
        Assert.AreEqual(10, expanded.Compile()(tree));
    }

    [TestMethod]
    public void MutuallyRecursiveExpressiveMembers_ExpandWithoutInfiniteLoop()
    {
        Expression<Func<MutualA, int>> expr = a => a.FromA;
        var expanded = (Expression<Func<MutualA, int>>)expr.ExpandExpressives();
        var fn = expanded.Compile();

        Assert.AreEqual(7, fn(new MutualA { X = 7 }));
        Assert.AreEqual(99, fn(new MutualA { X = 7, B = new MutualB { Y = 99 } }));
    }

    [TestMethod]
    public void Polyfill_StringRangeSlice_ProducesSubstring()
    {
        var fromStart = ExpressionPolyfill.Create((string s) => s[..3]);
        Assert.AreEqual("hel", fromStart.Compile()("hello"));

        var inMiddle = ExpressionPolyfill.Create((string s) => s[1..4]);
        Assert.AreEqual("ell", inMiddle.Compile()("hello"));

        var toEnd = ExpressionPolyfill.Create((string s) => s[2..]);
        Assert.AreEqual("llo", toEnd.Compile()("hello"));

        var fromEndBoth = ExpressionPolyfill.Create((string s) => s[^2..]);
        Assert.AreEqual("lo", fromEndBoth.Compile()("hello"));
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

public class ScoreBase
{
    public int Id { get; set; }

    [Expressive]
    public virtual int Score => Id * 2;
}

public class ScoreDerived : ScoreBase
{
    [Expressive]
    public override int Score => base.Score + 1;
}

public class GreetBase
{
    public int Id { get; set; }

    [Expressive]
    public virtual int Greet() => Id * 10;
}

public class GreetDerived : GreetBase
{
    [Expressive]
    public override int Greet() => base.Greet() + 1;
}

public class CalcBase
{
    public int Id { get; set; }

    [Expressive]
    public virtual int Calc(int n) => Id + n;
}

public class CalcDerived : CalcBase
{
    [Expressive]
    public override int Calc(int n) => Id * n;
}

public abstract class Node
{
    public int Id { get; set; }

    [Expressive]
    public virtual string Kind => $"node:{Id}";
}

public class Branch : Node
{
    [Expressive]
    public override string Kind => $"branch:{Id}";
}

public class Leaf : Branch
{
    [Expressive]
    public override string Kind => $"leaf:{Id}";
}

// Inherits Branch's override without redeclaring — should resolve via the `is Branch` arm.
public class PlainBranch : Branch
{
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

public class MutualA
{
    public int X { get; set; }
    public MutualB? B { get; set; }

    [Expressive]
    public int FromA => B == null ? X : B.FromB;
}

public class MutualB
{
    public int Y { get; set; }
    public MutualA? A { get; set; }

    [Expressive]
    public int FromB => A == null ? Y : A.FromA;
}

public class NestedCtorChildEntity { public int Value { get; set; } }
public class NestedCtorParentEntity { public int Id { get; set; } public NestedCtorChildEntity? Child { get; set; } }

public class NestedCtorChildDto
{
    public int Value { get; set; }
    public NestedCtorChildDto() { }
    [Expressive] public NestedCtorChildDto(NestedCtorChildEntity c) { Value = c.Value; }
}

public class NestedCtorParentDto
{
    public int Id { get; set; }
    public NestedCtorChildDto? Child { get; set; }
    public NestedCtorParentDto() { }
    [Expressive]
    public NestedCtorParentDto(NestedCtorParentEntity p)
    {
        Id = p.Id;
        Child = new NestedCtorChildDto(p.Child!);
    }
}

[TestClass]
public class NestedExpressiveCtorTests
{
    [TestMethod]
    public void Nested_ExpressiveCtor_InlinedViaExpandExpressives()
    {
        Expression<Func<NestedCtorParentEntity, NestedCtorParentDto>> expr = p => new NestedCtorParentDto(p);
        var expanded = (Expression<Func<NestedCtorParentEntity, NestedCtorParentDto>>)expr.ExpandExpressives();

        var fn = expanded.Compile();
        var result = fn(new NestedCtorParentEntity { Id = 7, Child = new NestedCtorChildEntity { Value = 42 } });

        Assert.AreEqual(7, result.Id);
        Assert.AreEqual(42, result.Child!.Value);

        var text = expanded.ToString();
        Assert.IsFalse(text.Contains("NestedCtorChildDto(p.", StringComparison.Ordinal),
            "Nested ctor should be inlined to MemberInit, not survive as `new NestedCtorChildDto(p...)`. Got: " + text);
    }
}
