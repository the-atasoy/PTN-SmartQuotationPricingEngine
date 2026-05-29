using Domain.Enums;

namespace Application.Features.Products.DTOs;

public class PriceHistoryDto
{
    public Guid Id { get; init; }
    public Guid RequestId { get; init; }
    public string RequestNo { get; init; } = default!;
    public string CustomerName { get; init; } = default!;
    public decimal Price { get; init; }
    public Currency Currency { get; init; }
    public DateTime CreatedAt { get; init; }
}
