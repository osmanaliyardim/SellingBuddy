using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellingBuddy.CatalogService.Api.Core.Domain;
using SellingBuddy.CatalogService.Api.Infrastructure.Context;

namespace SellingBuddy.CatalogService.Api.Infrastructure.EntityConfiguration;

public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToTable(nameof(CatalogBrand), CatalogContext.DEFAULT_SCHEMA);

        builder.HasKey(cb => cb.Id);

        builder.Property(cb => cb.Id)
            .UseHiLo("catalog_brand_hilo")
            .IsRequired();

        builder.Property(cb => cb.Brand)
            .IsRequired()
            .HasMaxLength(100);
    }
}
