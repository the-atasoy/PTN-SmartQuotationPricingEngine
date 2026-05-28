namespace Application.Features.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public decimal BasePrice { get; init; }
    public decimal? LastRequestPrice { get; init; }
    public DateTime? LastRequestDate { get; init; }
}
