using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.Transformers;

[TestClass]
public class FlattenConcatArrayCallsTests
{
    private readonly FlattenConcatArrayCalls _sut = new();

    private static readonly MethodInfo ConcatArray = typeof(string).GetMethod(
        nameof(string.Concat), [typeof(string[])])!;

    private static readonly MethodInfo Concat2 = typeof(string).GetMethod(
        nameof(string.Concat), [typeof(string), typeof(string)])!;

    private static readonly MethodInfo Concat3 = typeof(string).GetMethod(
        nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])!;

    private static readonly MethodInfo Concat4 = typeof(string).GetMethod(
        nameof(string.Concat), [typeof(string), typeof(string), typeof(string), typeof(string)])!;

    [TestMethod]
    public void EmptyArray_ReturnsEmptyConstant()
    {
        var array = Expression.NewArrayInit(typeof(string));
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        Assert.IsInstanceOfType<ConstantExpression>(result);
        Assert.AreEqual("", ((ConstantExpression)result).Value);
    }

    [TestMethod]
    public void SingleElement_ReturnsThatElement()
    {
        var element = Expression.Constant("hello");
        var array = Expression.NewArrayInit(typeof(string), element);
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        Assert.AreSame(element, result);
    }

    [TestMethod]
    public void TwoElements_UsesConcat2()
    {
        var a = Expression.Constant("a");
        var b = Expression.Constant("b");
        var array = Expression.NewArrayInit(typeof(string), a, b);
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var methodCall = (MethodCallExpression)result;
        Assert.AreEqual(Concat2, methodCall.Method);
        Assert.AreEqual(2, methodCall.Arguments.Count);
    }

    [TestMethod]
    public void ThreeElements_UsesConcat3()
    {
        var parts = Enumerable.Range(0, 3).Select(i => Expression.Constant($"p{i}")).ToArray();
        var array = Expression.NewArrayInit(typeof(string), parts);
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var methodCall = (MethodCallExpression)result;
        Assert.AreEqual(Concat3, methodCall.Method);
    }

    [TestMethod]
    public void FourElements_UsesConcat4()
    {
        var parts = Enumerable.Range(0, 4).Select(i => Expression.Constant($"p{i}")).ToArray();
        var array = Expression.NewArrayInit(typeof(string), parts);
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var methodCall = (MethodCallExpression)result;
        Assert.AreEqual(Concat4, methodCall.Method);
    }

    [TestMethod]
    public void FiveElements_ChainsConcat4ThenConcat2()
    {
        var parts = Enumerable.Range(0, 5).Select(i => Expression.Constant($"p{i}")).ToArray();
        var array = Expression.NewArrayInit(typeof(string), parts);
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        // Concat2(Concat4(p0, p1, p2, p3), p4)
        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var outer = (MethodCallExpression)result;
        Assert.AreEqual(Concat2, outer.Method);

        var inner = (MethodCallExpression)outer.Arguments[0];
        Assert.AreEqual(Concat4, inner.Method);
    }

    [TestMethod]
    public void SevenElements_ChainsConcat4ThenConcat4()
    {
        var parts = Enumerable.Range(0, 7).Select(i => Expression.Constant($"p{i}")).ToArray();
        var array = Expression.NewArrayInit(typeof(string), parts);
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        // Concat4(Concat4(p0, p1, p2, p3), p4, p5, p6)
        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var outer = (MethodCallExpression)result;
        Assert.AreEqual(Concat4, outer.Method);

        var inner = (MethodCallExpression)outer.Arguments[0];
        Assert.AreEqual(Concat4, inner.Method);
    }

    [TestMethod]
    public void NonArrayArgument_NotTransformed()
    {
        // string.Concat(string, string) should pass through unchanged
        var call = Expression.Call(Concat2, Expression.Constant("a"), Expression.Constant("b"));

        var result = _sut.Transform(call);

        Assert.AreSame(call, result);
    }

    [TestMethod]
    public void ProducesCorrectValue_WhenCompiled()
    {
        var parts = new[] { "Hello", " ", "world", "!", " ", "How", " are you?" }
            .Select(s => (Expression)Expression.Constant(s)).ToArray();
        var array = Expression.NewArrayInit(typeof(string), parts);
        var call = Expression.Call(ConcatArray, array);

        var result = _sut.Transform(call);

        // Compile and invoke to verify the result is correct
        var lambda = Expression.Lambda<Func<string>>(result);
        var compiled = lambda.Compile();
        Assert.AreEqual("Hello world! How are you?", compiled());
    }
}
