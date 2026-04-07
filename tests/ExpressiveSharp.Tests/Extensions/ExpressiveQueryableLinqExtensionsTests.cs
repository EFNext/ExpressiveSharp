using ExpressiveSharp.Extensions;

namespace ExpressiveSharp.Tests.Extensions;

[TestClass]
public class RewritableQueryableLinqExtensionsTests
{
    private IRewritableQueryable<int> CreateRewritableSource()
        => new[] { 1, 2, 3 }.AsQueryable().AsExpressive();

    [TestMethod]
    public void Take_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Take(2);
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void Skip_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Skip(1);
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void Distinct_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Distinct();
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void Reverse_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Reverse();
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void Concat_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Concat(new[] { 4, 5 });
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void Union_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Union(new[] { 3, 4 });
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void Intersect_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Intersect(new[] { 2, 3 });
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void Except_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Except(new[] { 1 });
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void DefaultIfEmpty_PreservesRewritableQueryable()
    {
        var source = new string[0].AsQueryable().AsExpressive();
        var result = source.DefaultIfEmpty();
        Assert.IsInstanceOfType<IRewritableQueryable<string?>>(result);
    }

    [TestMethod]
    public void DefaultIfEmpty_WithValue_PreservesRewritableQueryable()
    {
        var source = new int[0].AsQueryable().AsExpressive();
        var result = source.DefaultIfEmpty(42);
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void ChainedPassthrough_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Take(10).Skip(1).Distinct();
        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

#if NET9_0_OR_GREATER
    [TestMethod]
    public void Index_PreservesRewritableQueryable()
    {
        var source = CreateRewritableSource();
        var result = source.Index();
        Assert.IsInstanceOfType<IRewritableQueryable<(int Index, int Item)>>(result);
    }
#endif
}
