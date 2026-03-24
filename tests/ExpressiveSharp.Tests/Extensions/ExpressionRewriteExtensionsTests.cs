using ExpressiveSharp.Extensions;

namespace ExpressiveSharp.Tests.Extensions;

[TestClass]
public class ExpressionRewriteExtensionsTests
{
    [TestMethod]
    public void WithExpressionRewrite_WrapsAsRewritableQueryable()
    {
        var source = new[] { 1, 2, 3 }.AsQueryable();

        var result = source.WithExpressionRewrite();

        Assert.IsInstanceOfType<IRewritableQueryable<int>>(result);
    }

    [TestMethod]
    public void WithExpressionRewrite_DelegatesProperties()
    {
        var source = new[] { 1, 2, 3 }.AsQueryable();

        var result = source.WithExpressionRewrite();

        Assert.AreEqual(source.ElementType, result.ElementType);
        Assert.AreEqual(source.Expression, result.Expression);
        Assert.AreEqual(source.Provider, result.Provider);
    }

    [TestMethod]
    public void WithExpressionRewrite_NullSource_ThrowsArgumentNullException()
    {
        IQueryable<int>? source = null;

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            source!.WithExpressionRewrite());
    }
}
