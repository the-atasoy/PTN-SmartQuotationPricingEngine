using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = default!;
    public decimal BasePrice { get; private set; }
    public Currency BasePriceCurrency { get; private set; }
    public decimal? LastRequestPrice { get; private set; }
    public Currency? LastRequestCurrency { get; private set; }
    public DateTime? LastRequestDate { get; private set; }

    // Navigation properties
    private readonly List<ProductPriceHistory> _priceHistories = [];
    public IReadOnlyCollection<ProductPriceHistory> PriceHistories => _priceHistories.AsReadOnly();

    private Product() { } // EF Core

    public static Product Create(string name, decimal basePrice, Currency basePriceCurrency)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        if (basePrice < 0)
            throw new ArgumentException("Base price cannot be negative.", nameof(basePrice));

        return new Product
        {
            Name = name.Trim(),
            BasePrice = basePrice,
            BasePriceCurrency = basePriceCurrency
        };
    }

    /// <summary>
    /// Updates the last quoted price and date. Called when a request containing this product is sent.
    /// </summary>
    public void UpdateLastRequestPrice(decimal price, Currency currency, DateTime date)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        LastRequestPrice = price;
        LastRequestCurrency = currency;
        LastRequestDate = date;
    }
}
