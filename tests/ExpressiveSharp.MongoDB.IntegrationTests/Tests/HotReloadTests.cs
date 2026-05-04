using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.Extensions;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using ExpressiveSharp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

[TestClass]
[DoNotParallelize]
public class HotReloadTests : MongoTestBase
{
    [TestMethod]
    public async Task HotReload_NewExpressiveBody_FlowsThroughToMongoQueryProvider()
    {
        var baselineTotals = await Orders.AsExpressive()
            .Select(o => o.Total)
            .ToListAsync();
        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, baselineTotals);

        var (registryType, mapField, totalKey) = HotReloadRegistry.Locate(typeof(Order), nameof(Order.Total));
        var map = (IDictionary<nint, LambdaExpression>)mapField.GetValue(null)!;

        Expression<Func<Order, double>> reloaded = o => o.Price * o.Quantity * 2;
        map[totalKey] = reloaded;
        HotReloadRegistry.ClearResolverCaches();

        try
        {
            var reloadedTotals = await Orders.AsExpressive()
                .Select(o => o.Total)
                .ToListAsync();
            CollectionAssert.AreEquivalent(new[] { 480.0, 3000.0, 60.0, 500.0 }, reloadedTotals);
        }
        finally
        {
            HotReloadRegistry.Reset(registryType);
            HotReloadRegistry.ClearResolverCaches();
        }
    }
}

internal static class HotReloadRegistry
{
    public static (Type RegistryType, FieldInfo MapField, nint MemberKey) Locate(Type declaringType, string propertyName)
    {
        var registryType = declaringType.Assembly.GetType("ExpressiveSharp.Generated.ExpressionRegistry")
            ?? throw new InvalidOperationException(
                $"Generated ExpressionRegistry not found in {declaringType.Assembly.GetName().Name}.");
        var mapField = registryType.GetField("_map", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ExpressionRegistry._map not found.");
        var key = declaringType.GetProperty(propertyName)!.GetMethod!.MethodHandle.Value;
        return (registryType, mapField, key);
    }

    public static void Reset(Type registryType)
    {
        var reset = registryType.GetMethod("ResetMap", BindingFlags.Static | BindingFlags.NonPublic)!;
        reset.Invoke(null, null);
    }

    public static void ClearResolverCaches()
    {
        var method = typeof(ExpressiveResolver).GetMethod(
            "ClearCachesForMetadataUpdate",
            BindingFlags.Static | BindingFlags.NonPublic)!;
        method.Invoke(null, null);
    }
}
