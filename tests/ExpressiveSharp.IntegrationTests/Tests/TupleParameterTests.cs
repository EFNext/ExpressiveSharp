using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class TupleParameterTests
{
    [TestMethod]
    public void TupleParameter_ExpandedExpression_MatchesDirectInvocation()
    {
        var pair = (5, 2);
        Expression<Func<int>> expr = () => TupleParameterFixtures.First(pair);
        var fn = ((Expression<Func<int>>)expr.ExpandExpressives()).Compile();

        Assert.AreEqual(TupleParameterFixtures.First(pair), fn());
    }
}

public static class TupleParameterFixtures
{
    [Expressive]
    public static int First((int, int) pair) => pair.Item1;
}
