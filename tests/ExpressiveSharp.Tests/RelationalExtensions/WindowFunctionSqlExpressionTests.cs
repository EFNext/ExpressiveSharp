#pragma warning disable EF1001

using System.Linq.Expressions;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.RelationalExtensions;

[TestClass]
public class WindowFunctionSqlExpressionTests
{
    private static readonly SqlFragmentExpression ColPrice = new("[Price]");
    private static readonly SqlFragmentExpression ColTotal = new("[Total]");
    private static readonly SqlFragmentExpression ColCustomerId = new("[CustomerId]");

    private static string Print(Expression expr)
    {
        var printer = new ExpressionPrinter();
        printer.Visit(expr);
        return printer.ToString();
    }

    private static WindowFunctionSqlExpression MakeSum(Type type) =>
        new(
            "SUM",
            arguments: [ColPrice],
            partitions: [ColCustomerId],
            orderings: [new OrderingExpression(ColPrice, ascending: true)],
            type: type,
            typeMapping: null);

    private sealed class ReplacingVisitor(Expression from, Expression to) : ExpressionVisitor
    {
        public override Expression? Visit(Expression? node)
            => ReferenceEquals(node, from) ? to : base.Visit(node);
    }

    [TestMethod]
    public void VisitChildren_ReplacedArgument_IsReflectedInTheResult()
    {
        var expr = MakeSum(typeof(double));

        var visited = new ReplacingVisitor(ColPrice, ColTotal).Visit(expr)!;

        var printed = Print(visited);
        StringAssert.Contains(printed, "[Total]");
    }

    [TestMethod]
    public void Equals_DifferingClrType_IsNotEqual()
    {
        var asDouble = MakeSum(typeof(double));
        var asDecimal = MakeSum(typeof(decimal));

        Assert.AreNotEqual(asDouble, asDecimal);
    }

    [TestMethod]
    public void UnwrapAll_DuplicatedPlaceholder_RestoresBothOccurrences()
    {
        var windowFunction = MakeSum(typeof(double));

        var wrapped = WindowFunctionSqlExpressionWrapper.WrapAll(windowFunction, out var stash);
        Assert.AreEqual(1, stash.Count);

        var duplicated = Expression.Block(wrapped, wrapped);

        var unwrapped = (BlockExpression)WindowFunctionSqlExpressionWrapper.UnwrapAll(duplicated, stash);

        Assert.IsInstanceOfType<WindowFunctionSqlExpression>(unwrapped.Expressions[0]);
        Assert.IsInstanceOfType<WindowFunctionSqlExpression>(unwrapped.Expressions[1]);
    }
}
