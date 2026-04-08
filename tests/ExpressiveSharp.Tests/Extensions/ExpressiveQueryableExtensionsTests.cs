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
}
