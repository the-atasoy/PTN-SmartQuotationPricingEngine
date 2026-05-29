using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RequestConfiguration : BaseEntityConfiguration<Request>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("requests");

        builder.Property(r => r.RequestNo)
            .HasColumnName("request_no")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(r => r.RequestNo)
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.Property(r => r.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(r => r.TotalAmount)
            .HasColumnName("total_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.HasIndex(r => r.Status);

        // Items — owned by this aggregate
        builder.HasMany(r => r.Items)
            .WithOne(i => i.Request)
            .HasForeignKey(i => i.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Access the backing field for the Items collection
        builder.Navigation(r => r.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
