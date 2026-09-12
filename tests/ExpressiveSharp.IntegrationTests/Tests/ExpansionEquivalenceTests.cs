using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class ExpansionEquivalenceTests
{
    [TestMethod]
    public void PostfixIncrement_ExpandedExpression_MatchesDirectInvocation()
    {
        Expression<Func<int, int>> expr = s => EquivalenceFixtures.PostfixSemantics(s);
        var fn = ((Expression<Func<int, int>>)expr.ExpandExpressives()).Compile();

        Assert.AreEqual(EquivalenceFixtures.PostfixSemantics(5), fn(5));
    }

    [TestMethod]
    public void NestedAndChainedConditionalAccess_ExpandedExpression_MatchesDirectInvocation()
    {
        Expression<Func<ChainNode?, string?>> chained = n => EquivalenceFixtures.ChainedAccess(n);
        var chainedFn = ((Expression<Func<ChainNode?, string?>>)chained.ExpandExpressives()).Compile();

        var leaf = new ChainNode { Name = "leaf" };
        var mid = new ChainNode { Name = "mid", Next = leaf };
        var root = new ChainNode { Name = "root", Next = mid };

        Assert.AreEqual(EquivalenceFixtures.ChainedAccess(root), chainedFn(root));
        Assert.AreEqual(EquivalenceFixtures.ChainedAccess(mid), chainedFn(mid));
        Assert.AreEqual(EquivalenceFixtures.ChainedAccess(leaf), chainedFn(leaf));
        Assert.AreEqual(EquivalenceFixtures.ChainedAccess(null), chainedFn(null));

        Expression<Func<ChainNode?, ChainNode?, int?>> sibling =
            (a, b) => EquivalenceFixtures.SiblingAccess(a, b);
        var siblingFn = ((Expression<Func<ChainNode?, ChainNode?, int?>>)sibling.ExpandExpressives()).Compile();

        Assert.AreEqual(EquivalenceFixtures.SiblingAccess(root, leaf), siblingFn(root, leaf));
        Assert.AreEqual(EquivalenceFixtures.SiblingAccess(null, leaf), siblingFn(null, leaf));
        Assert.AreEqual(EquivalenceFixtures.SiblingAccess(root, null), siblingFn(root, null));
    }
}

public class ChainNode
{
    public string? Name { get; set; }

    public ChainNode? Next { get; set; }
}

public static class EquivalenceFixtures
{
    [Expressive(AllowBlockBody = true)]
    public static int PostfixSemantics(int seed)
    {
        int i = seed;
        int j = i++;
        return i * 10 + j;
    }

    [Expressive]
    public static string? ChainedAccess(ChainNode? node)
        => node?.Next?.Name;

    [Expressive]
    public static int? SiblingAccess(ChainNode? a, ChainNode? b)
        => (a?.Name ?? "").Length + b?.Name?.Length;
}
