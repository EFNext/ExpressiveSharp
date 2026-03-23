using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store;

public abstract class StoreTestBase : IntegrationTestBase
{
    [TestInitialize]
    public async Task SeedStoreData()
        => await Runner.SeedAsync(SeedData.Addresses, SeedData.Customers, SeedData.Orders, SeedData.LineItems);
}
