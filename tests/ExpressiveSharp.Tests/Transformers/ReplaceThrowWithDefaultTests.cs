using System.Linq.Expressions;
using ExpressiveSharp.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.Transformers;

[TestClass]
public class ReplaceThrowWithDefaultTests
{
    private readonly ReplaceThrowWithDefault _sut = new();

    [TestMethod]
    public void CoalesceWithThrow_ReplacesThrowWithDefault()
    {
        // Coalesce(x, Throw(new Exception(), typeof(string))) → Coalesce(x, Default(string))
        var x = Expression.Parameter(typeof(string), "x");
        var throwExpr = Expression.Throw(
            Expression.New(typeof(InvalidOperationException)),
            typeof(string));
        var coalesce = Expression.Coalesce(x, throwExpr);

        var result = _sut.Transform(coalesce);

        Assert.AreEqual(ExpressionType.Coalesce, result.NodeType,
            "Coalesce node should be preserved");
        var binary = (BinaryExpression)result;
        Assert.AreEqual(ExpressionType.Default, binary.Right.NodeType,
            "Throw should be replaced with Default");
        Assert.AreEqual(typeof(string), binary.Right.Type,
            "Default should have the same type as the original Throw");
    }

    [TestMethod]
    public void ConditionalWithThrowInFalse_ReplacesThrowWithDefault()
    {
        var test = Expression.Constant(true);
        var ifTrue = Expression.Constant(42);
        var throwExpr = Expression.Throw(
            Expression.New(typeof(InvalidOperationException)),
            typeof(int));
        var conditional = Expression.Condition(test, ifTrue, throwExpr);

        var result = _sut.Transform(conditional);

        Assert.AreEqual(ExpressionType.Conditional, result.NodeType,
            "Conditional node should be preserved");
        var cond = (ConditionalExpression)result;
        Assert.AreEqual(ExpressionType.Default, cond.IfFalse.NodeType,
            "Throw in false arm should be replaced with Default");
        Assert.AreEqual(typeof(int), cond.IfFalse.Type);
    }

    [TestMethod]
    public void ConditionalWithThrowInTrue_ReplacesThrowWithDefault()
    {
        var test = Expression.Constant(false);
        var throwExpr = Expression.Throw(
            Expression.New(typeof(InvalidOperationException)),
            typeof(int));
        var ifFalse = Expression.Constant(42);
        var conditional = Expression.Condition(test, throwExpr, ifFalse);

        var result = _sut.Transform(conditional);

        Assert.AreEqual(ExpressionType.Conditional, result.NodeType);
        var cond = (ConditionalExpression)result;
        Assert.AreEqual(ExpressionType.Default, cond.IfTrue.NodeType,
            "Throw in true arm should be replaced with Default");
    }

    [TestMethod]
    public void StandaloneThrow_BecomesDefault()
    {
        var throwExpr = Expression.Throw(
            Expression.New(typeof(InvalidOperationException)),
            typeof(void));

        var result = _sut.Transform(throwExpr);

        Assert.AreEqual(ExpressionType.Default, result.NodeType);
        Assert.AreEqual(typeof(void), result.Type);
    }

    [TestMethod]
    public void NonThrowCoalesce_Unchanged()
    {
        var x = Expression.Parameter(typeof(string), "x");
        var fallback = Expression.Constant("default");
        var coalesce = Expression.Coalesce(x, fallback);

        var result = _sut.Transform(coalesce);

        Assert.AreEqual(ExpressionType.Coalesce, result.NodeType);
        var binary = (BinaryExpression)result;
        Assert.AreEqual(ExpressionType.Constant, binary.Right.NodeType,
            "Non-throw coalesce should be unchanged");
    }

    [TestMethod]
    public void CoalesceWithThrow_CompilesCorrectly()
    {
        var x = Expression.Parameter(typeof(string), "x");
        var throwExpr = Expression.Throw(
            Expression.New(typeof(InvalidOperationException)),
            typeof(string));
        var coalesce = Expression.Coalesce(x, throwExpr);
        var lambda = Expression.Lambda<Func<string?, string>>(coalesce, x);

        var transformed = (Expression<Func<string?, string>>)_sut.Transform(lambda);
        var compiled = transformed.Compile();

        Assert.AreEqual("hello", compiled("hello"));
        Assert.IsNull(compiled(null));
    }
}
