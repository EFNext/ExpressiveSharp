namespace ExpressiveSharp.Tests.Extensions;

[TestClass]
public class ExpressiveQueryableLinqExtensionsTests
{
    private IExpressiveQueryable<int> CreateExpressiveSource()
        => new[] { 1, 2, 3 }.AsQueryable().AsExpressive();

    [TestMethod]
    public void Take_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Take(2);
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void Skip_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Skip(1);
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void Distinct_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Distinct();
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void Reverse_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Reverse();
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void Concat_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Concat(new[] { 4, 5 });
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void Union_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Union(new[] { 3, 4 });
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void Intersect_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Intersect(new[] { 2, 3 });
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void Except_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Except(new[] { 1 });
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void DefaultIfEmpty_PreservesExpressiveQueryable()
    {
        var source = new string[0].AsQueryable().AsExpressive();
        var result = source.DefaultIfEmpty();
        Assert.IsInstanceOfType<IExpressiveQueryable<string?>>(result);
    }

    [TestMethod]
    public void DefaultIfEmpty_WithValue_PreservesExpressiveQueryable()
    {
        var source = new int[0].AsQueryable().AsExpressive();
        var result = source.DefaultIfEmpty(42);
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void ChainedPassthrough_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Take(10).Skip(1).Distinct();
        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

#if NET9_0_OR_GREATER
    [TestMethod]
    public void Index_PreservesExpressiveQueryable()
    {
        var source = CreateExpressiveSource();
        var result = source.Index();
        Assert.IsInstanceOfType<IExpressiveQueryable<(int Index, int Item)>>(result);
    }
#endif
}
