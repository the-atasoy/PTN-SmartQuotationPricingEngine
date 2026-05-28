using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductPriceHistoryConfiguration : BaseEntityConfiguration<ProductPriceHistory>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProductPriceHistory> builder)
    {
        builder.ToTable("product_price_histories");

        builder.Property(ph => ph.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(ph => ph.RequestId)
            .HasColumnName("request_id")
            .IsRequired();

        builder.Property(ph => ph.QuotedPrice)
            .HasColumnName("quoted_price")
            .HasPrecision(18, 2)
            .IsRequired();

        // Composite index for querying price history by product ordered by date
        builder.HasIndex(ph => new { ph.ProductId, ph.CreatedAt });

        builder.HasOne(ph => ph.Request)
            .WithMany()
            .HasForeignKey(ph => ph.RequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
