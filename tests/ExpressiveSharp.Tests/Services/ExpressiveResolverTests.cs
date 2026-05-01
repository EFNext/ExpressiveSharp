using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.Services;
using ExpressiveSharp.Tests.TestFixtures;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressiveResolverTests
{
    private readonly ExpressiveResolver _resolver = new();

    [TestMethod]
    public void FindGeneratedExpression_PropertyWithExpressive_ReturnsLambdaExpression()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var result = _resolver.FindGeneratedExpression(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<LambdaExpression>(result);
    }

    [TestMethod]
    public void FindGeneratedExpression_MethodWithExpressive_ReturnsLambdaExpression()
    {
        var memberInfo = typeof(Product).GetMethod(nameof(Product.Label))!;

        var result = _resolver.FindGeneratedExpression(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<LambdaExpression>(result);
    }

    [TestMethod]
    public void FindGeneratedExpression_SameMember_ReturnsCachedResult()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var first = _resolver.FindGeneratedExpression(memberInfo);
        var second = _resolver.FindGeneratedExpression(memberInfo);

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void FindGeneratedExpression_NonExpressiveMember_ThrowsInvalidOperationException()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Price))!;

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            _resolver.FindGeneratedExpression(memberInfo));
    }

    [TestMethod]
    public void FindGeneratedExpressionViaReflection_KnownMember_ReturnsExpression()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var result = ExpressiveResolver.FindGeneratedExpressionViaReflection(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<LambdaExpression>(result);
    }

    [TestMethod]
    public void FindGeneratedExpressionViaReflection_UnknownMember_ReturnsNull()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Price))!;

        var result = ExpressiveResolver.FindGeneratedExpressionViaReflection(memberInfo);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindGeneratedExpression_NonPublicExpressiveProperty_ResolvesAndCompiles()
    {
        // Issue #50 follow-up: factory used GetProperty without BindingFlags.
        var memberInfo = typeof(NonPublicExpressive).GetProperty(
            nameof(NonPublicExpressive.DoubledWidth),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

        var result = _resolver.FindGeneratedExpression(memberInfo);

        Assert.IsNotNull(result);
        var compiled = (Func<NonPublicExpressive, int>)result.Compile();
        Assert.AreEqual(14, compiled(new NonPublicExpressive { Width = 7 }));
    }

    [TestMethod]
    public void FindGeneratedExpression_PropertyBody_ContainsMultiply()
    {
        var memberInfo = typeof(Product).GetProperty(nameof(Product.Total))!;

        var result = _resolver.FindGeneratedExpression(memberInfo);

        Assert.IsNotNull(result);
        Assert.IsTrue(ContainsNodeType(result.Body, ExpressionType.Multiply),
            "Expected Product.Total expression to contain a Multiply node");
    }

    // The most load-bearing correctness point: the generator must register the formula lambda
    // under the synthesized property's getter MethodHandle. If the registry were keyed off the
    // stub member (e.g. FullNameExpression), `ExpressiveReplacer` would never find a match at
    // runtime when user code accesses the public property and the rewrite would silently never fire.

    [TestMethod]
    public void FindGeneratedExpression_SynthesizedProperty_ResolvesByPropertyGetter()
    {
        var propertyInfo = typeof(SynthesizedCustomer).GetProperty(nameof(SynthesizedCustomer.FullName))!;

        var result = _resolver.FindGeneratedExpression(propertyInfo);

        Assert.IsNotNull(result, "Resolver must return a lambda for a synthesized property");
        Assert.IsInstanceOfType<LambdaExpression>(result);
    }

    [TestMethod]
    public void FindGeneratedExpression_SynthesizedProperty_BodyIsFormulaOnly()
    {
        var propertyInfo = typeof(SynthesizedCustomer).GetProperty(nameof(SynthesizedCustomer.FullName))!;

        var result = _resolver.FindGeneratedExpression(propertyInfo);

        Assert.IsNotNull(result);
        // The body must be the stub's formula — the synthesized property's own get accessor
        // (which contains the `??` coalesce) is invisible to the registry.
        Assert.IsFalse(ContainsNodeType(result.Body, ExpressionType.Coalesce),
            "Synthesized-property expression body must be the formula only, not the wrapping '??' coalesce");
        Assert.IsTrue(ContainsMemberAccess(result.Body, nameof(SynthesizedCustomer.LastName))
                      && ContainsMemberAccess(result.Body, nameof(SynthesizedCustomer.FirstName)),
            "Synthesized-property expression body must reference both dependencies of the formula");
    }

    [TestMethod]
    public void FindGeneratedExpression_SynthesizedProperty_BackingFieldIsNotInRegistry()
    {
        // Reflect across the generator-emitted backing field for FullName. Calling
        // FindGeneratedExpressionViaReflection on it should return null — the registry is
        // keyed on the property's getter, not on the backing field.
        var backingField = typeof(SynthesizedCustomer).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(f => f.Name.Contains("fullName", StringComparison.OrdinalIgnoreCase));

        if (backingField is null)
        {
            Assert.Inconclusive("Backing field not found via reflection; skipping.");
            return;
        }

        var result = ExpressiveResolver.FindGeneratedExpressionViaReflection(backingField);

        Assert.IsNull(result, "Registry must NOT resolve an entry for the backing field");
    }

    private static bool ContainsMemberAccess(Expression expr, string memberName) => expr switch
    {
        MemberExpression m when m.Member.Name == memberName => true,
        MemberExpression m => m.Expression is not null && ContainsMemberAccess(m.Expression, memberName),
        BinaryExpression b => ContainsMemberAccess(b.Left, memberName) || ContainsMemberAccess(b.Right, memberName),
        UnaryExpression u => ContainsMemberAccess(u.Operand, memberName),
        LambdaExpression l => ContainsMemberAccess(l.Body, memberName),
        MethodCallExpression mc => mc.Arguments.Any(a => ContainsMemberAccess(a, memberName))
            || (mc.Object is not null && ContainsMemberAccess(mc.Object, memberName)),
        _ => false
    };

    private static bool ContainsNodeType(Expression expr, ExpressionType nodeType)
    {
        if (expr.NodeType == nodeType)
            return true;

        return expr switch
        {
            BinaryExpression b => ContainsNodeType(b.Left, nodeType) || ContainsNodeType(b.Right, nodeType),
            UnaryExpression u => ContainsNodeType(u.Operand, nodeType),
            LambdaExpression l => ContainsNodeType(l.Body, nodeType),
            MethodCallExpression mc => mc.Arguments.Any(a => ContainsNodeType(a, nodeType))
                || (mc.Object is not null && ContainsNodeType(mc.Object, nodeType)),
            _ => false
        };
    }
}

/// <summary>
/// Test-local fixture for synthesized-property resolver tests. Declared here (not in the shared
/// <c>TestFixtures</c>) to keep the <c>[ExpressiveProperty]</c> dependency contained.
/// </summary>
public partial class SynthesizedCustomer
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    [ExpressiveSharp.Mapping.ExpressiveProperty("FullName")]
    private string FullNameExpression => LastName + ", " + FirstName;
}
