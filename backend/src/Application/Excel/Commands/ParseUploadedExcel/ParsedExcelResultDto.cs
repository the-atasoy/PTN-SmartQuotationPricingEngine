namespace Application.Excel.Commands.ParseUploadedExcel;

public class ParsedExcelResultDto
{
    public string RequestNo { get; set; } = default!;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal? LastRequestPrice { get; set; }
    public DateTime? LastRequestDate { get; set; }
    public bool HasPreviousPrice { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal Discount { get; set; }
}
