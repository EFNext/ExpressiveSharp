namespace ExpressiveSharp.Tests.Extensions;

[TestClass]
public class ExpressionPolyfillTests
{
    [TestMethod]
    public void Create_NullTransformerArgument_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            ExpressionPolyfill.Create((int n) => n + 1, (IExpressionTreeTransformer)null!));
    }
}
