namespace ExpressiveSharp.Tests.Extensions;

[TestClass]
public class ExpressiveQueryableExtensionsTests
{
    [TestMethod]
    public void AsExpressive_WrapsAsExpressiveQueryable()
    {
        var source = new[] { 1, 2, 3 }.AsQueryable();

        var result = source.AsExpressive();

        Assert.IsInstanceOfType<IExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void AsExpressive_DelegatesProperties()
    {
        var source = new[] { 1, 2, 3 }.AsQueryable();

        var result = source.AsExpressive();

        Assert.AreEqual(source.ElementType, result.ElementType);
        Assert.AreEqual(source.Expression, result.Expression);
        Assert.AreEqual(source.Provider, result.Provider);
    }

    [TestMethod]
    public void AsExpressive_NullSource_ThrowsArgumentNullException()
    {
        IQueryable<int>? source = null;

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            source!.AsExpressive());
    }

    [TestMethod]
    public void AsExpressive_OrderedSource_WrapsAsOrderedExpressiveQueryable()
    {
        var source = new[] { 3, 1, 2 }.AsQueryable().OrderBy(x => x);

        var result = source.AsExpressive();

        Assert.IsInstanceOfType<IOrderedExpressiveQueryable<int>>(result);
    }

    [TestMethod]
    public void AsExpressive_AlreadyOrderedExpressive_ReturnsSameInstance()
    {
        IOrderedQueryable<int> source = new[] { 3, 1, 2 }.AsQueryable().OrderBy(x => x).AsExpressive();

        var result = source.AsExpressive();

        Assert.AreSame(source, result);
    }

    [TestMethod]
    public void AsExpressive_OrderedSource_DelegatesProperties()
    {
        var source = new[] { 3, 1, 2 }.AsQueryable().OrderBy(x => x);

        var result = source.AsExpressive();

        Assert.AreEqual(source.ElementType, result.ElementType);
        Assert.AreEqual(source.Expression, result.Expression);
        Assert.AreEqual(source.Provider, result.Provider);
    }

    [TestMethod]
    public void AsExpressive_NullOrderedSource_ThrowsArgumentNullException()
    {
        IOrderedQueryable<int>? source = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => source!.AsExpressive());
    }
}
