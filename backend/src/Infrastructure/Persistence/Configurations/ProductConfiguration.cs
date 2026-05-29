using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : BaseEntityConfiguration<Product>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.BasePrice)
            .HasColumnName("base_price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.BasePriceCurrency)
            .HasColumnName("base_price_currency")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.LastRequestPrice)
            .HasColumnName("last_request_price")
            .HasPrecision(18, 2);

        builder.Property(p => p.LastRequestCurrency)
            .HasColumnName("last_request_currency")
            .HasConversion<int>();

        builder.Property(p => p.LastRequestDate)
            .HasColumnName("last_request_date");

        builder.HasMany(p => p.PriceHistories)
            .WithOne(ph => ph.Product)
            .HasForeignKey(ph => ph.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
