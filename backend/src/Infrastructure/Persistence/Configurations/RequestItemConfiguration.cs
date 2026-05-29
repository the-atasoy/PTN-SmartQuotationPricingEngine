using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RequestItemConfiguration : BaseEntityConfiguration<RequestItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RequestItem> builder)
    {
        builder.ToTable("request_items");

        builder.Property(ri => ri.RequestId)
            .HasColumnName("request_id")
            .IsRequired();

        builder.Property(ri => ri.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(ri => ri.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(ri => ri.UnitPrice)
            .HasColumnName("unit_price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ri => ri.Discount)
            .HasColumnName("discount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ri => ri.LineTotal)
            .HasColumnName("line_total")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(ri => ri.Product)
            .WithMany()
            .HasForeignKey(ri => ri.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
