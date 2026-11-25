using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class SpanOverloadTests
{
    [TestMethod]
    public void ArrayIndexOf_ResolvesReadOnlySpanOverload()
    {
        Expression<Func<SpanOverloadEntity, int>> expr = e => e.Rank;
        var expanded = (Expression<Func<SpanOverloadEntity, int>>)expr.ExpandExpressives();
        var rank = expanded.Compile();

        Assert.AreEqual(1, rank(new SpanOverloadEntity { Id = 10 }));
        Assert.AreEqual(-1, rank(new SpanOverloadEntity { Id = 99 }));
    }
}

public class SpanOverloadEntity
{
    private static readonly int[] Ranked = [30, 10, 20];

    public int Id { get; set; }

    [Expressive]
    public int Rank => Ranked.IndexOf(Id);
}
