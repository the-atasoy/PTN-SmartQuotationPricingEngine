using Domain.Enums;

namespace Application.Excel.Commands.ParseUploadedExcel;

public class ParsedExcelResultDto
{
    public string RequestNo { get; init; } = default!;
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public int Quantity { get; init; }
    public decimal? LastRequestPrice { get; set; }
    public Currency? LastRequestCurrency { get; set; }
    public DateTime? LastRequestDate { get; set; }
    public bool HasPreviousPrice { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? UnitPrice { get; init; }
    public decimal Discount { get; init; }
}
