using System.Linq.Expressions;
using ExpressiveSharp.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.Transformers;

[TestClass]
public class FlattenBlockExpressionsTests
{
    private readonly FlattenBlockExpressions _sut = new();

    [TestMethod]
    public void SingleVariableAssignment_Inlines()
    {
        var v = Expression.Variable(typeof(int), "v");
        var block = Expression.Block(
            new[] { v },
            Expression.Assign(v, Expression.Constant(42)),
            v);

        var result = _sut.Transform(block);

        Assert.IsInstanceOfType<ConstantExpression>(result);
        Assert.AreEqual(42, ((ConstantExpression)result).Value);
    }

    [TestMethod]
    public void TransitiveDependencies_FullyResolves()
    {
        var paramX = Expression.Parameter(typeof(int), "x");
        var a = Expression.Variable(typeof(int), "a");
        var b = Expression.Variable(typeof(int), "b");

        var block = Expression.Block(
            new[] { a, b },
            Expression.Assign(a, paramX),
            Expression.Assign(b, Expression.Add(a, Expression.Constant(1))),
            b);

        var result = _sut.Transform(block);

        Assert.IsInstanceOfType<BinaryExpression>(result);
        var binary = (BinaryExpression)result;
        Assert.AreEqual(ExpressionType.Add, binary.NodeType);
        Assert.AreSame(paramX, binary.Left);
        Assert.AreEqual(1, ((ConstantExpression)binary.Right).Value);
    }

    [TestMethod]
    public void MultipleAssignmentsSameVariable_ReturnsBlockUnchanged()
    {
        var v = Expression.Variable(typeof(int), "v");
        var block = Expression.Block(
            new[] { v },
            Expression.Assign(v, Expression.Constant(1)),
            Expression.Assign(v, Expression.Constant(2)),
            v);

        var result = _sut.Transform(block);

        Assert.IsInstanceOfType<BlockExpression>(result);
    }

    [TestMethod]
    public void NoVariables_SingleExpression_ReturnsThatExpression()
    {
        var block = Expression.Block(Expression.Constant(99));

        var result = _sut.Transform(block);

        Assert.IsInstanceOfType<ConstantExpression>(result);
        Assert.AreEqual(99, ((ConstantExpression)result).Value);
    }

    [TestMethod]
    public void NoVariables_MultipleExpressions_ReturnsBlock()
    {
        var block = Expression.Block(
            typeof(int),
            Expression.Constant(1),
            Expression.Constant(2));

        var result = _sut.Transform(block);

        Assert.IsInstanceOfType<BlockExpression>(result);
    }

    [TestMethod]
    public void NestedBlocks_RecursivelyFlattened()
    {
        var inner = Expression.Variable(typeof(int), "inner");
        var innerBlock = Expression.Block(
            new[] { inner },
            Expression.Assign(inner, Expression.Constant(7)),
            inner);

        var outer = Expression.Variable(typeof(int), "outer");
        var outerBlock = Expression.Block(
            new[] { outer },
            Expression.Assign(outer, innerBlock),
            outer);

        var result = _sut.Transform(outerBlock);

        Assert.IsInstanceOfType<ConstantExpression>(result);
        Assert.AreEqual(7, ((ConstantExpression)result).Value);
    }

    [TestMethod]
    public void NonBlockExpression_PassesThrough()
    {
        var expr = Expression.Add(Expression.Constant(1), Expression.Constant(2));

        var result = _sut.Transform(expr);

        Assert.AreSame(expr, result);
    }

    [TestMethod]
    public void VariableWithoutAssignment_ReturnsBlockUnchanged()
    {
        var v = Expression.Variable(typeof(int), "v");
        var block = Expression.Block(
            new[] { v },
            v);

        var result = _sut.Transform(block);

        Assert.IsInstanceOfType<BlockExpression>(result);
    }

    [TestMethod]
    public void ThreeChainedDependencies_FullyInlines()
    {
        var paramX = Expression.Parameter(typeof(int), "x");
        var a = Expression.Variable(typeof(int), "a");
        var b = Expression.Variable(typeof(int), "b");
        var c = Expression.Variable(typeof(int), "c");

        var block = Expression.Block(
            new[] { a, b, c },
            Expression.Assign(a, paramX),
            Expression.Assign(b, Expression.Add(a, Expression.Constant(1))),
            Expression.Assign(c, Expression.Multiply(b, Expression.Constant(2))),
            c);

        var result = _sut.Transform(block);

        Assert.IsInstanceOfType<BinaryExpression>(result);
        var multiply = (BinaryExpression)result;
        Assert.AreEqual(ExpressionType.Multiply, multiply.NodeType);
        Assert.AreEqual(2, ((ConstantExpression)multiply.Right).Value);

        var add = (BinaryExpression)multiply.Left;
        Assert.AreEqual(ExpressionType.Add, add.NodeType);
        Assert.AreSame(paramX, add.Left);
        Assert.AreEqual(1, ((ConstantExpression)add.Right).Value);
    }

    [TestMethod]
    public void BlockInsideLambda_FlattensBody()
    {
        var param = Expression.Parameter(typeof(int), "x");
        var v = Expression.Variable(typeof(int), "v");
        var block = Expression.Block(
            new[] { v },
            Expression.Assign(v, Expression.Add(param, Expression.Constant(10))),
            v);
        var lambda = Expression.Lambda<Func<int, int>>(block, param);

        var result = _sut.Transform(lambda);

        Assert.IsInstanceOfType<LambdaExpression>(result);
        var resultLambda = (LambdaExpression)result;
        Assert.IsInstanceOfType<BinaryExpression>(resultLambda.Body);
        var body = (BinaryExpression)resultLambda.Body;
        Assert.AreEqual(ExpressionType.Add, body.NodeType);
    }

    [TestMethod]
    public void ImplementsIExpressionTreeTransformer()
    {
        IExpressionTreeTransformer transformer = _sut;

        var expr = Expression.Constant(42);
        var result = transformer.Transform(expr);

        Assert.AreSame(expr, result);
    }
}
