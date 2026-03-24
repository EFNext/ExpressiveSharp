using System.Linq.Expressions;
using ExpressiveSharp.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.Transformers;

[TestClass]
public class FlattenTupleComparisonsTests
{
    private readonly FlattenTupleComparisons _sut = new();

    [TestMethod]
    public void FieldAccessOnNewValueTuple_ReplacedWithArgument()
    {
        // new ValueTuple<int, string>(42, "hello").Item1 → 42
        var ctor = typeof(ValueTuple<int, string>).GetConstructor(new[] { typeof(int), typeof(string) })!;
        var newTuple = Expression.New(ctor, Expression.Constant(42), Expression.Constant("hello"));
        var item1Field = typeof(ValueTuple<int, string>).GetField("Item1")!;
        var fieldAccess = Expression.Field(newTuple, item1Field);

        var result = _sut.Transform(fieldAccess);

        Assert.IsInstanceOfType<ConstantExpression>(result);
        Assert.AreEqual(42, ((ConstantExpression)result).Value);
    }

    [TestMethod]
    public void FieldAccessItem2_ReplacedWithSecondArgument()
    {
        var ctor = typeof(ValueTuple<int, string>).GetConstructor(new[] { typeof(int), typeof(string) })!;
        var newTuple = Expression.New(ctor, Expression.Constant(42), Expression.Constant("hello"));
        var item2Field = typeof(ValueTuple<int, string>).GetField("Item2")!;
        var fieldAccess = Expression.Field(newTuple, item2Field);

        var result = _sut.Transform(fieldAccess);

        Assert.IsInstanceOfType<ConstantExpression>(result);
        Assert.AreEqual("hello", ((ConstantExpression)result).Value);
    }

    [TestMethod]
    public void TupleEqualityComparison_SimplifiesToElementWise()
    {
        // Equal(new VT(a, b).Item1, new VT(c, d).Item1) → Equal(a, c)
        var ctor = typeof(ValueTuple<int, int>).GetConstructor(new[] { typeof(int), typeof(int) })!;
        var item1Field = typeof(ValueTuple<int, int>).GetField("Item1")!;

        var paramA = Expression.Parameter(typeof(int), "a");
        var paramB = Expression.Parameter(typeof(int), "b");
        var paramC = Expression.Parameter(typeof(int), "c");
        var paramD = Expression.Parameter(typeof(int), "d");

        var left = Expression.Field(Expression.New(ctor, paramA, paramB), item1Field);
        var right = Expression.Field(Expression.New(ctor, paramC, paramD), item1Field);
        var equal = Expression.Equal(left, right);

        var result = _sut.Transform(equal);

        Assert.IsInstanceOfType<BinaryExpression>(result);
        var binary = (BinaryExpression)result;
        Assert.AreEqual(ExpressionType.Equal, binary.NodeType);
        Assert.AreSame(paramA, binary.Left);
        Assert.AreSame(paramC, binary.Right);
    }

    [TestMethod]
    public void NonTupleExpression_LeftUnchanged()
    {
        var constant = Expression.Constant(42);
        var result = _sut.Transform(constant);

        Assert.AreSame(constant, result);
    }

    [TestMethod]
    public void FieldAccessOnNonNewExpression_LeftUnchanged()
    {
        // param.Item1 where param is a ValueTuple parameter (not a New expression) — should not be rewritten
        var param = Expression.Parameter(typeof(ValueTuple<int, int>), "t");
        var item1Field = typeof(ValueTuple<int, int>).GetField("Item1")!;
        var fieldAccess = Expression.Field(param, item1Field);

        var result = _sut.Transform(fieldAccess);

        Assert.IsInstanceOfType<MemberExpression>(result);
        var member = (MemberExpression)result;
        Assert.AreSame(param, member.Expression);
    }
}
