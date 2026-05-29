using Domain.Enums;

namespace Application.Products.Queries.GetProductPriceHistory;

public class PriceHistoryDto
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public Currency Currency { get; set; }
    public string RequestNo { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
}
