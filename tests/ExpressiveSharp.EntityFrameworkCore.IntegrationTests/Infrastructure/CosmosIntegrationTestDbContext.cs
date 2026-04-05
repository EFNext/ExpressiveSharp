#if TEST_COSMOS
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Cosmos DB version of the integration test context.
/// Models Customer and Address as owned types embedded within Order documents,
/// since Cosmos DB does not support cross-document JOINs or foreign keys.
/// </summary>
public class CosmosIntegrationTestDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();

    public CosmosIntegrationTestDbContext(DbContextOptions<CosmosIntegrationTestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Cosmos requires partition key properties to be strings (or have
            // a string value converter). Order.Id is int, so convert it.
            entity.Property(e => e.Id).HasConversion<string>();
            entity.HasPartitionKey(e => e.Id);

            // Customer is an owned type (embedded document) in Cosmos
            entity.OwnsOne(e => e.Customer, customer =>
            {
                customer.OwnsOne(c => c.Address);
            });

            // Items are owned collection (embedded array) in Cosmos
            entity.OwnsMany(e => e.Items);
        });
    }
}
#endif
