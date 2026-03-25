using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.Benchmarks.Helpers;

public class TestDbContext : DbContext
{
    private readonly bool _useExpressives;

    public TestDbContext(bool useExpressives)
    {
        _useExpressives = useExpressives;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=:memory:");
        if (_useExpressives)
            optionsBuilder.UseExpressives();
    }

    public DbSet<TestEntity> Entities => Set<TestEntity>();
}
