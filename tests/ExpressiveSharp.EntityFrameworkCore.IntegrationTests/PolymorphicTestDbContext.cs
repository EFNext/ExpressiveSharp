using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests;

// Table-per-hierarchy mapping for Animal/Dog/Cat. The virtual [Expressive] Description
// expands to a runtime type-test chain, which EF Core translates against the "Kind"
// discriminator.
public class PolymorphicTestDbContext : DbContext
{
    public DbSet<Animal> Animals => Set<Animal>();

    public PolymorphicTestDbContext(DbContextOptions<PolymorphicTestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasDiscriminator<string>("Kind")
                .HasValue<Animal>("animal")
                .HasValue<Dog>("dog")
                .HasValue<Cat>("cat");
        });
    }
}
