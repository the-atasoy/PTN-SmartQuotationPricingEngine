using Domain.Common;

namespace Domain.Entities;

public class ProductPriceHistory : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid RequestId { get; private set; }
    public decimal QuotedPrice { get; private set; }

    // Navigation properties
    public Product Product { get; private set; } = default!;
    public Request Request { get; private set; } = default!;

    private ProductPriceHistory() { } // EF Core

    public static ProductPriceHistory Create(Guid productId, Guid requestId, decimal quotedPrice)
    {
        if (quotedPrice < 0)
            throw new ArgumentException("Quoted price cannot be negative.", nameof(quotedPrice));

        return new ProductPriceHistory
        {
            ProductId = productId,
            RequestId = requestId,
            QuotedPrice = quotedPrice
        };
    }
}
