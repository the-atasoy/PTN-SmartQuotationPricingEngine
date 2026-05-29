using Domain.Common;

namespace Domain.Entities;

public class RequestItem : BaseEntity
{
    public Guid RequestId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal LineTotal { get; private set; }

    // Navigation properties
    public Request Request { get; private set; } = default!;
    public Product Product { get; private set; } = default!;

    private RequestItem() { } // EF Core

    internal static RequestItem Create(Guid requestId, Guid productId, int quantity, decimal unitPrice, decimal discount = 0)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        if (discount < 0 || discount > unitPrice)
            throw new ArgumentException("Discount cannot be negative or greater than unit price.", nameof(discount));

        var item = new RequestItem
        {
            RequestId = requestId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Discount = discount
        };

        item.CalculateLineTotal();
        return item;
    }

    internal void Update(int quantity, decimal unitPrice, decimal discount = 0)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        if (discount < 0 || discount > unitPrice)
            throw new ArgumentException("Discount cannot be negative or greater than unit price.", nameof(discount));

        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        CalculateLineTotal();
    }

    private void CalculateLineTotal()
    {
        LineTotal = Quantity * (UnitPrice - Discount);
    }
}
