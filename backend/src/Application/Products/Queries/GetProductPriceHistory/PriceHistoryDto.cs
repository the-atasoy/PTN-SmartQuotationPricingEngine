namespace Application.Products.Queries.GetProductPriceHistory;

public class PriceHistoryDto
{
    public DateTime Date { get; set; }
    public decimal QuotedPrice { get; set; }
    public string RequestNo { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
}
