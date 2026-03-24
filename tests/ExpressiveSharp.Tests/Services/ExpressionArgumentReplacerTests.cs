using System.Linq.Expressions;
using ExpressiveSharp.Services;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressionArgumentReplacerTests
{
    [TestMethod]
    public void Visit_ParameterInMapping_ReplacesWithMappedExpression()
    {
        var paramX = Expression.Parameter(typeof(int), "x");
        var constant42 = Expression.Constant(42);

        var replacer = new ExpressionArgumentReplacer();
        replacer.ParameterArgumentMapping[paramX] = constant42;

        var result = replacer.Visit(paramX);

        Assert.AreEqual(constant42, result);
    }

    [TestMethod]
    public void Visit_ParameterNotInMapping_ReturnsOriginal()
    {
        var paramX = Expression.Parameter(typeof(int), "x");
        var paramY = Expression.Parameter(typeof(int), "y");
        var constant42 = Expression.Constant(42);

        var replacer = new ExpressionArgumentReplacer();
        replacer.ParameterArgumentMapping[paramX] = constant42;

        var result = replacer.Visit(paramY);

        Assert.AreEqual(paramY, result);
    }

    [TestMethod]
    public void Visit_ComplexExpression_ReplacesAllMappedParameters()
    {
        var paramX = Expression.Parameter(typeof(int), "x");
        var paramY = Expression.Parameter(typeof(int), "y");
        var constant1 = Expression.Constant(1);
        var constant2 = Expression.Constant(2);
        var addExpression = Expression.Add(paramX, paramY);

        var replacer = new ExpressionArgumentReplacer();
        replacer.ParameterArgumentMapping[paramX] = constant1;
        replacer.ParameterArgumentMapping[paramY] = constant2;

        var result = (BinaryExpression)replacer.Visit(addExpression);

        Assert.AreEqual(constant1, result.Left);
        Assert.AreEqual(constant2, result.Right);
    }
}
