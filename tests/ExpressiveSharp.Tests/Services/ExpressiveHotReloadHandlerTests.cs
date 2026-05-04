using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using ExpressiveSharp.Services;
using ExpressiveSharp.Tests.TestFixtures;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressiveHotReloadHandlerTests
{
    [TestMethod]
    public void ClearCache_AfterResolve_RemovesMemberFromCache()
    {
        var mi = typeof(Product).GetProperty(nameof(Product.Total))!;
        var resolver = new ExpressiveResolver();

        _ = resolver.FindGeneratedExpression(mi);
        Assert.IsTrue(ExpressiveResolver.IsExpressionCached(mi));

        ExpressiveHotReloadHandler.ClearCache(null);

        Assert.IsFalse(ExpressiveResolver.IsExpressionCached(mi));
    }

    [TestMethod]
    public void ClearCache_PreservesAssemblyScanFilter()
    {
        var sentinel = new Func<Assembly, bool>(_ => true);
        ExpressiveResolver.SetAssemblyScanFilter(sentinel);
        try
        {
            ExpressiveHotReloadHandler.ClearCache(null);

            Assert.AreSame(sentinel, ExpressiveResolver.GetAssemblyScanFilter());
        }
        finally
        {
            ExpressiveResolver.SetAssemblyScanFilter(null);
        }
    }

    [TestMethod]
    public void ClearCache_RebuildReturnsEquivalentExpression()
    {
        var mi = typeof(Product).GetProperty(nameof(Product.Total))!;
        var resolver = new ExpressiveResolver();

        var before = resolver.FindGeneratedExpression(mi).ToString();

        ExpressiveHotReloadHandler.ClearCache(null);

        var after = resolver.FindGeneratedExpression(mi).ToString();

        Assert.AreEqual(before, after);
    }

    [TestMethod]
    public void ClearCache_WithNullAndEmptyAndPopulatedArrays_DoesNotThrow()
    {
        ExpressiveHotReloadHandler.ClearCache(null);
        ExpressiveHotReloadHandler.ClearCache([]);
        ExpressiveHotReloadHandler.ClearCache([typeof(Product)]);
    }

    [TestMethod]
    public void ClearCache_WithUpdatedTypes_ClearsResolverCache()
    {
        var mi = typeof(Product).GetProperty(nameof(Product.Total))!;
        var resolver = new ExpressiveResolver();

        _ = resolver.FindGeneratedExpression(mi);
        Assert.IsTrue(ExpressiveResolver.IsExpressionCached(mi));

        ExpressiveHotReloadHandler.ClearCache([typeof(Product)]);

        Assert.IsFalse(ExpressiveResolver.IsExpressionCached(mi));
    }

    [TestMethod]
    public void UpdateApplication_WithNull_DoesNotThrow()
    {
        ExpressiveHotReloadHandler.UpdateApplication(null);
    }

    [TestMethod]
    public void Assembly_RegistersExpressiveHotReloadHandler()
    {
        var attributes = typeof(ExpressiveResolver).Assembly
            .GetCustomAttributes<MetadataUpdateHandlerAttribute>()
            .ToList();

        Assert.IsTrue(attributes.Any(a => a.HandlerType == typeof(ExpressiveHotReloadHandler)),
            "MetadataUpdateHandlerAttribute for ExpressiveHotReloadHandler not found on ExpressiveSharp assembly.");
    }
    
    [TestMethod]
    public void ClearCache_RebuildsGeneratedRegistryMap()
    {
        var registryType = typeof(Product).Assembly.GetType("ExpressiveSharp.Generated.ExpressionRegistry");
        Assert.IsNotNull(registryType, "Generated ExpressionRegistry not found in test assembly.");

        var mapField = registryType!.GetField("_map", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(mapField, "ExpressionRegistry._map field not found.");

        var totalProperty = typeof(Product).GetProperty(nameof(Product.Total))!;
        var resolver = new ExpressiveResolver();

        var initial = resolver.FindGeneratedExpression(totalProperty);
        Assert.IsNotNull(initial);
        Assert.AreEqual(ExpressionType.Multiply, initial.Body.NodeType);

        var mapBefore = mapField!.GetValue(null);
        Assert.IsNotNull(mapBefore);

        ExpressiveHotReloadHandler.ClearCache([typeof(Product)]);

        var mapAfter = mapField.GetValue(null);
        Assert.IsNotNull(mapAfter);
        Assert.IsFalse(ReferenceEquals(mapBefore, mapAfter),
            "ExpressionRegistry._map was not rebuilt — ResetMap was not invoked by the hot-reload handler.");

        var rebuilt = resolver.FindGeneratedExpression(totalProperty);
        Assert.IsNotNull(rebuilt);
        Assert.AreEqual(initial.ToString(), rebuilt.ToString());
    }
}
