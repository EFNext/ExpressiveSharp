using System.Linq.Expressions;
using ExpressiveSharp.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.Transformers;

public class NullTestOuter
{
    public NullTestInner? Inner { get; set; }
}

public class NullTestInner
{
    public string? Value { get; set; }
}

[TestClass]
public class RemoveNullConditionalPatternsTests
{
    private readonly RemoveNullConditionalPatterns _sut = new();

    [TestMethod]
    public void SimpleNullCheck_MemberAccess_RemovesPattern()
    {
        var s = Expression.Parameter(typeof(string), "s");
        var lengthProp = Expression.Property(s, "Length");

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            lengthProp,
            Expression.Default(typeof(int)),
            typeof(int));

        var result = _sut.Transform(conditional);

        Assert.AreNotEqual(ExpressionType.Conditional, result.NodeType,
            "Null check conditional should have been removed");
        Assert.IsTrue(result.ToString().Contains("Length"),
            $"Result should still reference Length, got: {result}");
    }

    [TestMethod]
    public void ChainedNullConditional_RemovesNestedTernaries()
    {
        var a = Expression.Parameter(typeof(NullTestOuter), "a");
        var innerProp = Expression.Property(a, "Inner");
        var valueProp = Expression.Property(innerProp, "Value");

        var innerConditional = Expression.Condition(
            Expression.NotEqual(innerProp, Expression.Constant(null, typeof(NullTestInner))),
            valueProp,
            Expression.Default(typeof(string)),
            typeof(string));

        var outerConditional = Expression.Condition(
            Expression.NotEqual(a, Expression.Constant(null, typeof(NullTestOuter))),
            innerConditional,
            Expression.Default(typeof(string)),
            typeof(string));

        var result = _sut.Transform(outerConditional);

        Assert.IsInstanceOfType<MemberExpression>(result);
        var member = (MemberExpression)result;
        Assert.AreEqual("Value", member.Member.Name);
    }

    [TestMethod]
    public void WithConvertWrapping_KeepsConvertWhenTypesDontMatch()
    {
        var s = Expression.Parameter(typeof(string), "s");
        var lengthProp = Expression.Property(s, "Length");
        var convertedLength = Expression.Convert(lengthProp, typeof(int?));

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            convertedLength,
            Expression.Default(typeof(int?)),
            typeof(int?));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<UnaryExpression>(result);
        var convert = (UnaryExpression)result;
        Assert.AreEqual(ExpressionType.Convert, convert.NodeType);
        Assert.AreEqual(typeof(int?), convert.Type);
        Assert.IsInstanceOfType<MemberExpression>(convert.Operand);
    }

    [TestMethod]
    public void NonMatchingConditional_NoTransform()
    {
        var x = Expression.Parameter(typeof(int), "x");
        var conditional = Expression.Condition(
            Expression.GreaterThan(x, Expression.Constant(0)),
            Expression.Constant(1),
            Expression.Constant(2));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<ConditionalExpression>(result);
        Assert.AreSame(conditional, result);
    }

    [TestMethod]
    public void MethodCallOnReceiver_RemovesNullCheck()
    {
        var s = Expression.Parameter(typeof(string), "s");
        var toUpper = Expression.Call(s, typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!);

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            toUpper,
            Expression.Constant(null, typeof(string)),
            typeof(string));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("ToUpper", call.Method.Name);
    }

    [TestMethod]
    public void DefaultExpressionAsFalseBranch_Matches()
    {
        var s = Expression.Parameter(typeof(string), "s");
        var toUpper = Expression.Call(s, typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!);

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            toUpper,
            Expression.Default(typeof(string)),
            typeof(string));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("ToUpper", call.Method.Name);
    }

    [TestMethod]
    public void NonDefaultFalseBranch_NoTransform()
    {
        var s = Expression.Parameter(typeof(string), "s");
        var lengthProp = Expression.Property(s, "Length");

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            lengthProp,
            Expression.Constant(99));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<ConditionalExpression>(result);
        Assert.AreSame(conditional, result);
    }

    [TestMethod]
    public void TrueBranchDoesNotAccessReceiver_NoTransform()
    {
        var s = Expression.Parameter(typeof(string), "s");

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            Expression.Constant(42, typeof(int)),
            Expression.Default(typeof(int)),
            typeof(int));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<ConditionalExpression>(result);
        Assert.AreSame(conditional, result);
    }

    [TestMethod]
    public void ImplementsIExpressionTreeTransformer()
    {
        IExpressionTreeTransformer transformer = _sut;

        var expr = Expression.Constant("hello");
        var result = transformer.Transform(expr);

        Assert.AreSame(expr, result);
    }
}
