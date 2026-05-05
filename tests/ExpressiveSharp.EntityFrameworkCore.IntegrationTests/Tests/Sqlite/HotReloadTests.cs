using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
[DoNotParallelize]
public class HotReloadTests : EFCoreTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        var handle = TestContextFactories.CreateSqlite();
        context = handle.Context;
        return handle;
    }

    [TestInitialize]
    public Task SeedHotReloadData() => Context.SeedStoreAsync();

    [TestMethod]
    public async Task HotReload_NewExpressiveBody_FlowsThroughToSqlAndResults()
    {
        var baselineSql = Context.Set<Order>().Where(o => o.Total > 200).ToQueryString();
        var baselineTotals = await Context.Set<Order>().Select(o => o.Total).OrderBy(t => t).ToListAsync();
        CollectionAssert.AreEqual(new[] { 30.0, 240.0, 250.0, 1500.0 }, baselineTotals);

        var (registryType, mapField, totalKey) = HotReloadRegistry.Locate(typeof(Order), nameof(Order.Total));
        var map = (IDictionary<nint, LambdaExpression>)mapField.GetValue(null)!;
        Expression<Func<Order, double>> reloaded = o => o.Price * o.Quantity * 2;

        try
        {
            map[totalKey] = reloaded;
            HotReloadRegistry.ClearResolverCaches();

            var reloadedSql = Context.Set<Order>().Where(o => o.Total > 200).ToQueryString();
            Assert.AreNotEqual(baselineSql, reloadedSql,
                "ExpressiveQueryCompiler did not pick up the reloaded body — SQL is unchanged.");

            var reloadedTotals = await Context.Set<Order>().Select(o => o.Total).OrderBy(t => t).ToListAsync();
            CollectionAssert.AreEqual(new[] { 60.0, 480.0, 500.0, 3000.0 }, reloadedTotals);
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
