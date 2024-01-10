using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellingBuddy.OrderService.Domain.AggregateModels.OrderAggregate;
using SellingBuddy.OrderService.Infrastructure.Context;

namespace SellingBuddy.OrderService.Infrastructure.EntityConfigurations;

public class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("orderstatus", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(os => os.Id);
        builder.Property(os => os.Id).ValueGeneratedOnAdd();

        builder.Property(os => os.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(os => os.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
