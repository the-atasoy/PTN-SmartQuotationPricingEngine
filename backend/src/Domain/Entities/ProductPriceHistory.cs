using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class ProductPriceHistory : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid RequestId { get; private set; }
    public Currency Currency { get; private set; }
    public decimal Price { get; private set; }

    // Navigation properties
    public Product Product { get; private set; } = default!;
    public Request Request { get; private set; } = default!;

    private ProductPriceHistory() { } // EF Core

    public static ProductPriceHistory Create(Guid productId, Guid requestId, decimal price, Currency currency)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        return new ProductPriceHistory
        {
            ProductId = productId,
            RequestId = requestId,
            Currency = currency,
            Price = price
        };
    }
}
