namespace Application.Features.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public decimal BasePrice { get; init; }
    public Domain.Enums.Currency BasePriceCurrency { get; init; }
    public decimal? LastRequestPrice { get; init; }
    public Domain.Enums.Currency? LastRequestCurrency { get; init; }
    public DateTime? LastRequestDate { get; init; }
}
