using Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.DAL;

internal sealed class RepositoryContext : DbContext
{
    public RepositoryContext()
    {
        Database.EnsureCreated();
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(ProductsInitializer.Initialize());
    }
}