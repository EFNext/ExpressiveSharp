using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.Transformers;

[TestClass]
public class ConvertLoopsToLinqTests
{
    private readonly ConvertLoopsToLinq _sut = new();

    /// <summary>
    /// Builds the expression tree structure the emitter produces for a foreach loop:
    /// Block([accumulator], [
    ///   Assign(accumulator, initValue),
    ///   Block([enumerator, iterVar], [
    ///     Assign(enumerator, Call(collection, GetEnumerator)),
    ///     Loop(IfThenElse(Call(enum, MoveNext), Block(Assign(iter, Current), body), Break), label)
    ///   ]),
    ///   accumulator
    /// ])
    /// </summary>
    private static Expression BuildForEachLoop(
        ParameterExpression collection,
        ParameterExpression accumulator,
        Expression initValue,
        Type elementType,
        Func<ParameterExpression, ParameterExpression, Expression> buildBody)
    {
        var iterVar = Expression.Variable(elementType, "item");

        var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
        var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);
        var getEnumerator = enumerableType.GetMethod("GetEnumerator")!;
        var moveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext")!;
        var current = enumeratorType.GetProperty("Current")!;

        var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
        var breakLabel = Expression.Label("break");

        var body = buildBody(iterVar, accumulator);

        var loopBody = Expression.Block(
            Expression.Assign(iterVar, Expression.Property(enumeratorVar, current)),
            body);

        var loop = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(enumeratorVar, moveNext),
                loopBody,
                Expression.Break(breakLabel)),
            breakLabel);

        var innerBlock = Expression.Block(
            new[] { enumeratorVar, iterVar },
            Expression.Assign(enumeratorVar, Expression.Call(collection, getEnumerator)),
            loop);

        return Expression.Block(
            new[] { accumulator },
            Expression.Assign(accumulator, initValue),
            innerBlock,
            accumulator);
    }

    [TestMethod]
    public void ForEach_Sum_RewritesToLinqSum()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var sum = Expression.Variable(typeof(int), "sum");

        var loopExpr = BuildForEachLoop(items, sum, Expression.Constant(0),
            typeof(int),
            (iter, acc) => Expression.Assign(acc, Expression.Add(acc, iter)));

        var result = _sut.Transform(loopExpr);

        // Should be a MethodCallExpression to Enumerable.Sum
        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("Sum", call.Method.Name);
        Assert.AreEqual(typeof(Enumerable), call.Method.DeclaringType);
    }

    [TestMethod]
    public void ForEach_Count_RewritesToLinqCount()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var count = Expression.Variable(typeof(int), "count");

        var loopExpr = BuildForEachLoop(items, count, Expression.Constant(0),
            typeof(int),
            (iter, acc) => Expression.Assign(acc, Expression.Add(acc, Expression.Constant(1))));

        var result = _sut.Transform(loopExpr);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("Count", call.Method.Name);
    }

    [TestMethod]
    public void ForEach_Any_RewritesToLinqAny()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var found = Expression.Variable(typeof(bool), "found");

        var loopExpr = BuildForEachLoop(items, found, Expression.Constant(false),
            typeof(int),
            (iter, acc) => Expression.IfThen(
                Expression.GreaterThan(iter, Expression.Constant(5)),
                Expression.Assign(acc, Expression.Constant(true))));

        var result = _sut.Transform(loopExpr);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("Any", call.Method.Name);
    }

    [TestMethod]
    public void ForEach_All_RewritesToLinqAll()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var all = Expression.Variable(typeof(bool), "all");

        var loopExpr = BuildForEachLoop(items, all, Expression.Constant(true),
            typeof(int),
            (iter, acc) => Expression.IfThen(
                Expression.Not(Expression.GreaterThan(iter, Expression.Constant(0))),
                Expression.Assign(acc, Expression.Constant(false))));

        var result = _sut.Transform(loopExpr);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("All", call.Method.Name);
    }

    [TestMethod]
    public void ForEach_ConditionalSum_RewritesToWhereAndSum()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var sum = Expression.Variable(typeof(int), "sum");

        var loopExpr = BuildForEachLoop(items, sum, Expression.Constant(0),
            typeof(int),
            (iter, acc) => Expression.IfThen(
                Expression.GreaterThan(iter, Expression.Constant(0)),
                Expression.Assign(acc, Expression.Add(acc, iter))));

        var result = _sut.Transform(loopExpr);

        // Should be Sum(Where(...))
        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var sumCall = (MethodCallExpression)result;
        Assert.AreEqual("Sum", sumCall.Method.Name);
        Assert.IsInstanceOfType<MethodCallExpression>(sumCall.Arguments[0]);
        var whereCall = (MethodCallExpression)sumCall.Arguments[0];
        Assert.AreEqual("Where", whereCall.Method.Name);
    }

    [TestMethod]
    public void ForEach_UnrecognizedPattern_Throws()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var acc = Expression.Variable(typeof(int), "acc");

        // Unrecognized: acc = acc * iter (multiplication, not addition — not a Sum pattern)
        var loopExpr = BuildForEachLoop(items, acc, Expression.Constant(1),
            typeof(int),
            (iter, accum) => Expression.Assign(accum, Expression.Multiply(accum, iter)));

        Assert.ThrowsExactly<InvalidOperationException>(() => _sut.Transform(loopExpr));
    }

    [TestMethod]
    public void ForEach_Sum_ExecutesCorrectly()
    {
        // End-to-end: build loop, transform, compile, execute
        var items = Expression.Parameter(typeof(List<int>), "items");
        var sum = Expression.Variable(typeof(int), "sum");

        var loopExpr = BuildForEachLoop(items, sum, Expression.Constant(0),
            typeof(int),
            (iter, acc) => Expression.Assign(acc, Expression.Add(acc, iter)));

        var transformed = _sut.Transform(loopExpr);
        var lambda = Expression.Lambda<Func<List<int>, int>>(transformed, items);
        var compiled = lambda.Compile();

        var result = compiled(new List<int> { 1, 2, 3, 4, 5 });
        Assert.AreEqual(15, result);
    }

    [TestMethod]
    public void ForEach_Min_RewritesToLinqMin()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var min = Expression.Variable(typeof(int), "min");

        var mathMin = typeof(Math).GetMethod("Min", new[] { typeof(int), typeof(int) })!;

        var loopExpr = BuildForEachLoop(items, min,
            Expression.Field(null, typeof(int).GetField("MaxValue")!),
            typeof(int),
            (iter, acc) => Expression.Assign(acc, Expression.Call(mathMin, acc, iter)));

        var result = _sut.Transform(loopExpr);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("Min", call.Method.Name);
    }

    [TestMethod]
    public void ForEach_Max_RewritesToLinqMax()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var max = Expression.Variable(typeof(int), "max");

        var mathMax = typeof(Math).GetMethod("Max", new[] { typeof(int), typeof(int) })!;

        var loopExpr = BuildForEachLoop(items, max,
            Expression.Field(null, typeof(int).GetField("MinValue")!),
            typeof(int),
            (iter, acc) => Expression.Assign(acc, Expression.Call(mathMax, acc, iter)));

        var result = _sut.Transform(loopExpr);

        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var call = (MethodCallExpression)result;
        Assert.AreEqual("Max", call.Method.Name);
    }

    [TestMethod]
    public void ForEach_Select_RewritesToLinqSelectToList()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var resultList = Expression.Variable(typeof(List<string>), "result");
        var listCtor = typeof(List<string>).GetConstructor(Type.EmptyTypes)!;
        var addMethod = typeof(List<string>).GetMethod("Add")!;
        var toStringMethod = typeof(int).GetMethod("ToString", Type.EmptyTypes)!;

        var loopExpr = BuildForEachLoop(items, resultList,
            Expression.New(listCtor),
            typeof(int),
            (iter, acc) => Expression.Call(acc, addMethod, Expression.Call(iter, toStringMethod)));

        var result = _sut.Transform(loopExpr);

        // Should be ToList(Select(...))
        Assert.IsInstanceOfType<MethodCallExpression>(result);
        var toListCall = (MethodCallExpression)result;
        Assert.AreEqual("ToList", toListCall.Method.Name);
        Assert.IsInstanceOfType<MethodCallExpression>(toListCall.Arguments[0]);
        var selectCall = (MethodCallExpression)toListCall.Arguments[0];
        Assert.AreEqual("Select", selectCall.Method.Name);
    }

    [TestMethod]
    public void ForEach_Select_ExecutesCorrectly()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var resultList = Expression.Variable(typeof(List<string>), "result");
        var listCtor = typeof(List<string>).GetConstructor(Type.EmptyTypes)!;
        var addMethod = typeof(List<string>).GetMethod("Add")!;
        var toStringMethod = typeof(int).GetMethod("ToString", Type.EmptyTypes)!;

        var loopExpr = BuildForEachLoop(items, resultList,
            Expression.New(listCtor),
            typeof(int),
            (iter, acc) => Expression.Call(acc, addMethod, Expression.Call(iter, toStringMethod)));

        var transformed = _sut.Transform(loopExpr);
        var lambda = Expression.Lambda<Func<List<int>, List<string>>>(transformed, items);
        var compiled = lambda.Compile();

        var actual = compiled(new List<int> { 1, 2, 3 });
        CollectionAssert.AreEqual(new[] { "1", "2", "3" }, actual);
    }

    [TestMethod]
    public void ForEach_Min_ExecutesCorrectly()
    {
        var items = Expression.Parameter(typeof(List<int>), "items");
        var min = Expression.Variable(typeof(int), "min");
        var mathMin = typeof(Math).GetMethod("Min", new[] { typeof(int), typeof(int) })!;

        var loopExpr = BuildForEachLoop(items, min,
            Expression.Field(null, typeof(int).GetField("MaxValue")!),
            typeof(int),
            (iter, acc) => Expression.Assign(acc, Expression.Call(mathMin, acc, iter)));

        var transformed = _sut.Transform(loopExpr);
        var lambda = Expression.Lambda<Func<List<int>, int>>(transformed, items);
        var compiled = lambda.Compile();

        var result = compiled(new List<int> { 5, 2, 8, 1, 9 });
        Assert.AreEqual(1, result);
    }
}
