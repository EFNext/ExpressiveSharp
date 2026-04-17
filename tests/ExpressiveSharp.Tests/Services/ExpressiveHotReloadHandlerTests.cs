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
}
