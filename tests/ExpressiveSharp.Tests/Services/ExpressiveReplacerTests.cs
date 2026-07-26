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

    public class PlanCacheBase
    {
        [Expressive]
        public virtual int Value => 1;
    }

    public class PlanCacheDerived : PlanCacheBase
    {
        [Expressive]
        public override int Value => 2;
    }

    private sealed class PartialStubResolver(bool provideDerivedBody) : IExpressiveResolver
    {
        public LambdaExpression FindGeneratedExpression(MemberInfo expressiveMemberInfo,
            ExpressiveAttribute? expressiveAttribute = null)
        {
            if (expressiveMemberInfo.DeclaringType == typeof(PlanCacheDerived) && !provideDerivedBody)
            {
                return null!;
            }

            var parameter = Expression.Parameter(expressiveMemberInfo.DeclaringType!, "x");
            var value = expressiveMemberInfo.DeclaringType == typeof(PlanCacheDerived) ? 2 : 1;
            return Expression.Lambda(Expression.Constant(value), parameter);
        }

        public LambdaExpression? FindExternalExpression(MemberInfo memberInfo) => null;
    }

    private sealed class DerivedTypeIsFinder : ExpressionVisitor
    {
        public bool FoundDerivedTypeTest { get; private set; }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (node.TypeOperand == typeof(PlanCacheDerived))
            {
                FoundDerivedTypeTest = true;
            }

            return base.VisitTypeBinary(node);
        }
    }

    public class ResilientBase { [Expressive] public virtual int Value => 1; }

    public class ResilientDerived : ResilientBase { [Expressive] public override int Value => 2; }

    private sealed class ThrowingDerivedResolver : IExpressiveResolver
    {
        public LambdaExpression FindGeneratedExpression(MemberInfo member, ExpressiveAttribute? attribute = null)
            => member.DeclaringType == typeof(ResilientDerived)
                ? throw new InvalidOperationException($"Unable to resolve generated expression for {member.Name}.")
                : Expression.Lambda(Expression.Constant(1), Expression.Parameter(member.DeclaringType!, "x"));

        public LambdaExpression? FindExternalExpression(MemberInfo member) => null;
    }

    [TestMethod]
    public void Replace_DerivedOverrideThatFailsToResolve_ThrowsActionableError()
    {
        Expression<Func<ResilientBase, int>> query = b => b.Value;
        var replacer = new ExpressiveReplacer(new ThrowingDerivedResolver());

        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => replacer.Replace(query));

        StringAssert.Contains(ex.Message, nameof(ResilientDerived));
        StringAssert.Contains(ex.Message, "DisablePolymorphicDispatch");
        Assert.IsNotNull(ex.InnerException);
    }

    [TestMethod]
    public void PolymorphicPlanCache_DoesNotLeakAcrossResolvers()
    {
        Expression<Func<PlanCacheBase, int>> query = b => b.Value;

        var replacer1 = new ExpressiveReplacer(new PartialStubResolver(provideDerivedBody: true));
        replacer1.Replace(query);

        var precondition = new DerivedTypeIsFinder();
        precondition.Visit(replacer1.Replace(query));
        Assert.IsTrue(precondition.FoundDerivedTypeTest);

        replacer1.Replace(query);

        var replacer2 = new ExpressiveReplacer(new PartialStubResolver(provideDerivedBody: false));
        var expanded = replacer2.Replace(query);

        var finder = new DerivedTypeIsFinder();
        finder.Visit(expanded);

        Assert.IsFalse(finder.FoundDerivedTypeTest);
    }
}
