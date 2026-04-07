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
    public void MethodCallOnReceiver_KeepsNullCheck()
    {
        // Method calls (e.g. s?.ToUpper()) are intentionally left alone: not all
        // LINQ providers propagate null through function calls the way SQL does.
        // MongoDB's aggregation $toUpper, for example, returns "" on a null/missing
        // field rather than null, so stripping the null check would silently change
        // semantics. Only pure member access chains are flattened.
        var s = Expression.Parameter(typeof(string), "s");
        var toUpper = Expression.Call(s, typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!);

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            toUpper,
            Expression.Constant(null, typeof(string)),
            typeof(string));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<ConditionalExpression>(result);
        Assert.AreSame(conditional, result);
    }

    [TestMethod]
    public void ConvertWrappingMethodCall_KeepsNullCheck()
    {
        // Pattern: (s != null) ? (object)s.ToString() : null
        // ContainsMethodCall must walk through the Convert wrapper to detect
        // that the inner expression is a method call.
        var s = Expression.Parameter(typeof(string), "s");
        var toString = Expression.Call(s, typeof(string).GetMethod("ToString", Type.EmptyTypes)!);
        var convertedToObject = Expression.Convert(toString, typeof(object));

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            convertedToObject,
            Expression.Constant(null, typeof(object)),
            typeof(object));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<ConditionalExpression>(result);
        Assert.AreSame(conditional, result);
    }

    [TestMethod]
    public void ChainedNullConditional_WithInnerMethodCall_KeepsOuterNullCheck()
    {
        // Pattern: a?.Inner?.Value.ToString() — the inner nested conditional
        // contains a method call, so ContainsMethodCall must recurse into it
        // and the outer null check must NOT be stripped.
        var a = Expression.Parameter(typeof(NullTestOuter), "a");
        var innerProp = Expression.Property(a, "Inner");
        var valueProp = Expression.Property(innerProp, "Value");
        var toString = Expression.Call(valueProp, typeof(object).GetMethod("ToString", Type.EmptyTypes)!);

        var innerConditional = Expression.Condition(
            Expression.NotEqual(innerProp, Expression.Constant(null, typeof(NullTestInner))),
            toString,
            Expression.Constant(null, typeof(string)),
            typeof(string));

        var outerConditional = Expression.Condition(
            Expression.NotEqual(a, Expression.Constant(null, typeof(NullTestOuter))),
            innerConditional,
            Expression.Constant(null, typeof(string)),
            typeof(string));

        var result = _sut.Transform(outerConditional);

        // Outer is preserved (the inner method call prevents flattening).
        Assert.IsInstanceOfType<ConditionalExpression>(result);
        Assert.AreSame(outerConditional, result);
    }

    [TestMethod]
    public void DefaultExpressionAsFalseBranch_Matches()
    {
        // Same shape as SimpleNullCheck_MemberAccess_RemovesPattern but uses
        // Expression.Default(T) instead of Expression.Constant(null, T) on the
        // false branch — verifies the transformer treats both as "null-like".
        var s = Expression.Parameter(typeof(string), "s");
        var lengthProp = Expression.Property(s, "Length");

        var conditional = Expression.Condition(
            Expression.NotEqual(s, Expression.Constant(null, typeof(string))),
            lengthProp,
            Expression.Default(typeof(int)),
            typeof(int));

        var result = _sut.Transform(conditional);

        Assert.IsInstanceOfType<MemberExpression>(result);
        var member = (MemberExpression)result;
        Assert.AreEqual("Length", member.Member.Name);
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
