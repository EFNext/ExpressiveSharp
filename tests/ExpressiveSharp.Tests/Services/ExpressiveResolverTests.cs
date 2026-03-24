using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.Services;
using ExpressiveSharp.Tests.TestFixtures;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressiveResolverTests
{
    private readonly ExpressiveResolver _resolver = new();

    [TestMethod]
    public void FindGeneratedExpression_PropertyWithExpressive_ReturnsLambdaExpression()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var result = _resolver.FindGeneratedExpression(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<LambdaExpression>(result);
    }

    [TestMethod]
    public void FindGeneratedExpression_MethodWithExpressive_ReturnsLambdaExpression()
    {
        var memberInfo = typeof(Product).GetMethod(nameof(Product.Label))!;

        var result = _resolver.FindGeneratedExpression(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<LambdaExpression>(result);
    }

    [TestMethod]
    public void FindGeneratedExpression_SameMember_ReturnsCachedResult()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var first = _resolver.FindGeneratedExpression(memberInfo);
        var second = _resolver.FindGeneratedExpression(memberInfo);

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void FindGeneratedExpression_NonExpressiveMember_ThrowsInvalidOperationException()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Price))!;

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            _resolver.FindGeneratedExpression(memberInfo));
    }

    [TestMethod]
    public void FindGeneratedExpressionViaReflection_KnownMember_ReturnsExpression()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var result = ExpressiveResolver.FindGeneratedExpressionViaReflection(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<LambdaExpression>(result);
    }

    [TestMethod]
    public void FindGeneratedExpressionViaReflection_UnknownMember_ReturnsNull()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Price))!;

        var result = ExpressiveResolver.FindGeneratedExpressionViaReflection(memberInfo);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindGeneratedExpression_PropertyBody_ContainsMultiply()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var result = _resolver.FindGeneratedExpression(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsTrue(ContainsNodeType(result.Body, ExpressionType.Multiply),
            "Expected Product.Total expression to contain a Multiply node");
    }

    private static bool ContainsNodeType(Expression expr, ExpressionType nodeType)
    {
        if (expr.NodeType == nodeType)
            return true;

        return expr switch
        {
            BinaryExpression b => ContainsNodeType(b.Left, nodeType) || ContainsNodeType(b.Right, nodeType),
            UnaryExpression u => ContainsNodeType(u.Operand, nodeType),
            LambdaExpression l => ContainsNodeType(l.Body, nodeType),
            _ => false
        };
    }
}
