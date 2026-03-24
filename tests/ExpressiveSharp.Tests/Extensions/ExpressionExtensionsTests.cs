using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.Services;
using ExpressiveSharp.Tests.TestFixtures;

namespace ExpressiveSharp.Tests.Extensions;

[TestClass]
public class ExpressionExtensionsTests
{
    [TestCleanup]
    public void Cleanup()
    {
        ExpressiveDefaults.ClearTransformers();
    }

    [TestMethod]
    public void ExpandExpressives_NoTransformers_ExpandsExpressiveMembers()
    {
        var param = Expression.Parameter(typeof(Product), "p");
        var totalAccess = Expression.Property(param, nameof(Product.Total));
        Expression<Func<Product, double>> lambda = Expression.Lambda<Func<Product, double>>(totalAccess, param);

        var result = lambda.Body.ExpandExpressives();

        Assert.AreNotEqual(ExpressionType.MemberAccess, result.NodeType,
            "Product.Total should have been expanded");
    }

    [TestMethod]
    public void ExpandExpressives_ExplicitTransformers_AppliedInOrder()
    {
        var callOrder = new List<int>();
        var transformer1 = new RecordingTransformer(1, callOrder);
        var transformer2 = new RecordingTransformer(2, callOrder);
        var expr = Expression.Constant(42);

        expr.ExpandExpressives(transformer1, transformer2);

        Assert.AreEqual(2, callOrder.Count);
        Assert.AreEqual(1, callOrder[0]);
        Assert.AreEqual(2, callOrder[1]);
    }

    [TestMethod]
    public void ExpandExpressives_DefaultTransformers_UsedWhenNoExplicit()
    {
        var callOrder = new List<int>();
        var transformer = new RecordingTransformer(1, callOrder);
        ExpressiveDefaults.AddTransformers(transformer);
        var expr = Expression.Constant(42);

        expr.ExpandExpressives();

        Assert.AreEqual(1, callOrder.Count);
        Assert.AreEqual(1, callOrder[0]);
    }

    [TestMethod]
    public void ExpandExpressives_ExplicitTransformers_OverrideDefaults()
    {
        var defaultOrder = new List<int>();
        var explicitOrder = new List<int>();
        ExpressiveDefaults.AddTransformers(new RecordingTransformer(1, defaultOrder));
        var explicitTransformer = new RecordingTransformer(2, explicitOrder);
        var expr = Expression.Constant(42);

        expr.ExpandExpressives(explicitTransformer);

        Assert.AreEqual(0, defaultOrder.Count, "Default transformer should not have been called");
        Assert.AreEqual(1, explicitOrder.Count, "Explicit transformer should have been called");
    }

    private class RecordingTransformer : IExpressionTreeTransformer
    {
        private readonly int _id;
        private readonly List<int> _callOrder;

        public RecordingTransformer(int id, List<int> callOrder)
        {
            _id = id;
            _callOrder = callOrder;
        }

        public Expression Transform(Expression expression)
        {
            _callOrder.Add(_id);
            return expression;
        }
    }
}
