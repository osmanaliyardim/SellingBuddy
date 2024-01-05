using Microsoft.EntityFrameworkCore;
using SellingBuddy.CatalogService.Api.Core.Domain;
using SellingBuddy.CatalogService.Api.Infrastructure.EntityConfiguration;

namespace SellingBuddy.CatalogService.Api.Infrastructure.Context;

public class CatalogContext : DbContext
{
    public const string DEFAULT_SCHEMA = "catalog";

    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
    {
        
    }

    public DbSet<CatalogItem> CatalogItems { get; set; }

    public DbSet<CatalogBrand> CatalogBrands { get; set; }

    public DbSet<CatalogType> CatalogTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
    }
}
