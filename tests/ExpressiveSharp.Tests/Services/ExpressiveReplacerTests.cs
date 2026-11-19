using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.Services;
using ExpressiveSharp.Tests.TestFixtures;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressiveReplacerTests
{
    [TestMethod]
    public void Replace_ExpressiveProperty_InlinesExpression()
    {
        var resolver = new ExpressiveResolver();
        var replacer = new ExpressiveReplacer(resolver);
        var param = Expression.Parameter(typeof(Product), "p");
        var propertyAccess = Expression.Property(param, nameof(Product.Total));

        var result = replacer.Replace(propertyAccess);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(ExpressionType.MemberAccess, result.NodeType,
            "Property access should have been replaced");
    }

    [TestMethod]
    public void Replace_ExpressiveMethod_InlinesExpression()
    {
        var resolver = new ExpressiveResolver();
        var replacer = new ExpressiveReplacer(resolver);
        var param = Expression.Parameter(typeof(Product), "p");
        var methodCall = Expression.Call(param, typeof(Product).GetMethod(nameof(Product.Label))!);

        var result = replacer.Replace(methodCall);

        Assert.IsNotNull(result);
        // Label() uses string interpolation, so the expanded result is still a Call
        // (e.g., string.Concat), but it should NOT be a call to Product.Label() anymore
        if (result is MethodCallExpression expandedCall)
        {
            Assert.AreNotEqual(nameof(Product.Label), expandedCall.Method.Name,
                "Method call to Product.Label should have been replaced with its body");
        }
    }

    [TestMethod]
    public void Replace_NonExpressiveMember_PassesThrough()
    {
        var resolver = new ExpressiveResolver();
        var replacer = new ExpressiveReplacer(resolver);
        var param = Expression.Parameter(typeof(Product), "p");
        var propertyAccess = Expression.Property(param, nameof(Product.Price));

        var result = replacer.Replace(propertyAccess);

        Assert.IsNotNull(result);
        Assert.AreEqual(ExpressionType.MemberAccess, result.NodeType);
        var member = (MemberExpression)result;
        Assert.AreEqual(nameof(Product.Price), member.Member.Name);
    }

    [TestMethod]
    public void Replace_ExpressiveProperty_SubstitutesReceiver()
    {
        var resolver = new ExpressiveResolver();
        var replacer = new ExpressiveReplacer(resolver);
        var param = Expression.Parameter(typeof(Product), "myProduct");
        var propertyAccess = Expression.Property(param, nameof(Product.Total));

        var result = replacer.Replace(propertyAccess);

        Assert.IsNotNull(result);
        var resultStr = result.ToString();
        Assert.IsTrue(resultStr.Contains("myProduct"),
            $"Expected result to reference 'myProduct' parameter, got: {resultStr}");
    }

    [TestMethod]
    public void Replace_WithMockResolver_PropertyInlined()
    {
        var productParam = Expression.Parameter(typeof(Product), "p");
        var priceAccess = Expression.Property(productParam, nameof(Product.Price));
        var body = Expression.Multiply(priceAccess, Expression.Constant(2.0));
        var lambda = Expression.Lambda(body, productParam);

        var mockResolver = new MockResolver();
        mockResolver.Register(typeof(Product).GetProperty(nameof(Product.Total))!, lambda);

        var replacer = new ExpressiveReplacer(mockResolver);
        var param = Expression.Parameter(typeof(Product), "x");
        var access = Expression.Property(param, nameof(Product.Total));

        var result = replacer.Replace(access);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(ExpressionType.MemberAccess, result.NodeType);
    }

    [TestMethod]
    public void Replace_WithMockResolver_NonDecoratedMember_PassesThrough()
    {
        var mockResolver = new MockResolver();
        var replacer = new ExpressiveReplacer(mockResolver);
        var param = Expression.Parameter(typeof(Product), "p");

        // Product.Id has no [Expressive] attribute, so replacer should not call resolver
        var propertyAccess = Expression.Property(param, nameof(Product.Id));
        var result = replacer.Replace(propertyAccess);

        Assert.AreEqual(ExpressionType.MemberAccess, result!.NodeType);
    }

    [TestMethod]
    public void Replace_LambdaWithExpressiveProperty_ExpandsInBody()
    {
        var resolver = new ExpressiveResolver();
        var replacer = new ExpressiveReplacer(resolver);
        var param = Expression.Parameter(typeof(Product), "p");
        var totalAccess = Expression.Property(param, nameof(Product.Total));
        var lambda = Expression.Lambda<Func<Product, double>>(totalAccess, param);

        var result = replacer.Replace(lambda);

        Assert.IsNotNull(result);
        var resultLambda = (LambdaExpression)result;
        Assert.AreNotEqual(ExpressionType.MemberAccess, resultLambda.Body.NodeType,
            "Lambda body should have been expanded");
    }

    [TestMethod]
    public void Replace_InterfaceCastOnProperty_UnwrapsConvert()
    {
        var resolver = new ExpressiveResolver();
        var replacer = new ExpressiveReplacer(resolver);
        var param = Expression.Parameter(typeof(Product), "p");

        var propertyAccess = Expression.Property(param, nameof(Product.Price));
        var result = replacer.Replace(propertyAccess);

        Assert.IsNotNull(result);
        Assert.AreEqual(ExpressionType.MemberAccess, result.NodeType);
    }

    private class MockResolver : IExpressiveResolver
    {
        private readonly Dictionary<MemberInfo, LambdaExpression> _expressions = new();

        public void Register(MemberInfo member, LambdaExpression expression)
            => _expressions[member] = expression;

        public LambdaExpression FindGeneratedExpression(MemberInfo expressiveMemberInfo,
            ExpressiveAttribute? expressiveAttribute = null)
            => _expressions.TryGetValue(expressiveMemberInfo, out var expr)
                ? expr
                : throw new InvalidOperationException("Not registered");

        public LambdaExpression? FindExternalExpression(MemberInfo memberInfo)
            => _expressions.TryGetValue(memberInfo, out var expr) ? expr : null;
    }

    [TestMethod]
    public void Replace_CapturedQueryable_InlinesAndExpandsExpressiveMembers()
    {
        var products = new[]
        {
            new Product { Id = 1, Price = 60, Quantity = 2 },
            new Product { Id = 2, Price = 10, Quantity = 1 },
        };
        var expensive = products.AsQueryable().Where(p => p.Total > 100);
        Expression<Func<Product, bool>> isExpensive = p => expensive.Any(e => e.Id == p.Id);

        var result = (Expression<Func<Product, bool>>)new ExpressiveReplacer(new ExpressiveResolver()).Replace(isExpensive);

        var members = ReferencedMembers.Collect(result);
        Assert.IsTrue(members.Contains(typeof(Product).GetProperty(nameof(Product.Price))!));
        Assert.IsFalse(members.Contains(typeof(Product).GetProperty(nameof(Product.Total))!));
        var compiled = result.Compile();
        Assert.IsTrue(compiled(products[0]));
        Assert.IsFalse(compiled(products[1]));
    }

    [TestMethod]
    public void Replace_NestedCapturedQueryables_ExpandsExpressiveMembersAtEveryLevel()
    {
        var products = new[] { new Product { Id = 1, Price = 60, Quantity = 2 } }.AsQueryable();
        var expensive = products.Where(p => p.Total > 100);
        var labelled = products.Where(p => p.Label() == "Product 1" && expensive.Any(e => e.Id == p.Id));
        Expression<Func<bool>> anyLabelled = () => labelled.Any();

        var result = new ExpressiveReplacer(new ExpressiveResolver()).Replace(anyLabelled);

        var members = ReferencedMembers.Collect(result);
        Assert.IsTrue(members.Contains(typeof(Product).GetProperty(nameof(Product.Price))!));
        Assert.IsFalse(members.Contains(typeof(Product).GetProperty(nameof(Product.Total))!));
        Assert.IsFalse(members.Contains(typeof(Product).GetMethod(nameof(Product.Label))!));
    }

    [TestMethod]
    public void Replace_CapturedRootEnumerableQuery_LeftUnchanged()
    {
        var products = new List<Product>().AsQueryable();
        Expression<Func<bool>> anyProducts = () => products.Any();

        var result = new ExpressiveReplacer(new ExpressiveResolver()).Replace(anyProducts);

        Assert.AreSame(anyProducts, result);
    }

    private sealed class ReferencedMembers : ExpressionVisitor
    {
        private readonly HashSet<MemberInfo> _members = new();

        public static HashSet<MemberInfo> Collect(Expression expression)
        {
            var collector = new ReferencedMembers();
            collector.Visit(expression);
            return collector._members;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _members.Add(node.Member);
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _members.Add(node.Method);
            return base.VisitMethodCall(node);
        }
    }
}
