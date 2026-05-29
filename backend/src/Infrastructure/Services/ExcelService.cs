using Application.Excel.Commands.ParseUploadedExcel;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;
using System.Drawing;

namespace Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ExcelService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
        ExcelPackage.License.SetNonCommercialOrganization("SmartQuotation");
    }

    public byte[] GenerateQuotationRequestExcel(Request request)
    {
        using var package = new ExcelPackage();
        var sheetName = _localizer["Excel_SheetName"];
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        // Headers
        worksheet.Cells[1, 1].Value = _localizer["Excel_RequestNo"];
        worksheet.Cells[1, 2].Value = _localizer["Excel_ProductId"];
        worksheet.Cells[1, 3].Value = _localizer["Excel_ProductName"];
        worksheet.Cells[1, 4].Value = _localizer["Excel_Quantity"];
        worksheet.Cells[1, 5].Value = _localizer["Excel_UnitPrice"];
        worksheet.Cells[1, 6].Value = "Discount (Per Unit)";
        worksheet.Cells[1, 7].Value = "Discount % (Per Unit)";
        worksheet.Cells[1, 8].Value = "Line Total";

        // Make headers bold and colorful
        using (var range = worksheet.Cells[1, 1, 1, 8])
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(52, 73, 94)); // Dark slate blue
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }


        // Rows
        var row = 2;
        foreach (var item in request.Items)
        {
            worksheet.Cells[row, 1].Value = request.RequestNo;
            worksheet.Cells[row, 2].Value = item.ProductId.ToString();
            worksheet.Cells[row, 3].Value = item.Product.Name;
            worksheet.Cells[row, 4].Value = item.Quantity;
            worksheet.Cells[row, 5].Value = item.Product.LastRequestPrice ?? 0;
            worksheet.Cells[row, 6].Value = 0; // Default discount is 0
            
            // Highlight editable fields
            worksheet.Cells[row, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 250, 205)); // LemonChiffon

            worksheet.Cells[row, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[row, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 250, 205)); // LemonChiffon

            worksheet.Cells[row, 7].Formula = $"IF(E{row}>0, F{row}/E{row}, 0)";
            worksheet.Cells[row, 7].Style.Numberformat.Format = "0.00%";
            
            worksheet.Cells[row, 8].Formula = $"D{row}*(E{row}-F{row})";
            worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0.00";

            // Alternating row colors
            if (row % 2 == 0)
            {
                worksheet.Cells[row, 1, row, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[row, 1, row, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 247, 250));

                worksheet.Cells[row, 7, row, 8].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[row, 7, row, 8].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 247, 250));
            }

            row++;
        }

        var lastDataRow = row - 1;

        // Apply borders to all data cells
        using (var dataRange = worksheet.Cells[1, 1, lastDataRow, 8])
        {
            dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            dataRange.Style.Border.Top.Color.SetColor(Color.LightGray);
            dataRange.Style.Border.Left.Color.SetColor(Color.LightGray);
            dataRange.Style.Border.Right.Color.SetColor(Color.LightGray);
            dataRange.Style.Border.Bottom.Color.SetColor(Color.LightGray);
        }

        // Totals Row Styling
        using (var totalRange = worksheet.Cells[row, 1, row, 8])
        {
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Font.Color.SetColor(Color.White);
            totalRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            totalRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(41, 128, 185)); // Belize Hole (Blue)
        }

        worksheet.Cells[row, 3].Value = "TOTALS";

        worksheet.Cells[row, 6].Formula = $"SUMPRODUCT(D2:D{lastDataRow}, F2:F{lastDataRow})";
        worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
        worksheet.Cells[row, 6].Style.Font.Bold = true;

        worksheet.Cells[row, 7].Formula = $"IF(H{row}+F{row}>0, F{row}/(H{row}+F{row}), 0)";
        worksheet.Cells[row, 7].Style.Numberformat.Format = "0.00%";
        worksheet.Cells[row, 7].Style.Font.Bold = true;

        worksheet.Cells[row, 8].Formula = $"SUM(H2:H{lastDataRow})";
        worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0.00";
        worksheet.Cells[row, 8].Style.Font.Bold = true;

        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }

    public List<ParsedExcelResultDto> ParseExcel(Stream stream)
    {
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            throw new InvalidOperationException("Excel file is empty or invalid.");
        }

        var results = new List<ParsedExcelResultDto>();
        int rowCount = worksheet.Dimension?.Rows ?? 0;

        for (int row = 2; row <= rowCount; row++) // Skip header
        {
            var requestNoStr = worksheet.Cells[row, 1].Text;
            var productIdStr = worksheet.Cells[row, 2].Text;
            var productName = worksheet.Cells[row, 3].Text;
            var quantityStr = worksheet.Cells[row, 4].Text;
            var unitPriceStr = worksheet.Cells[row, 5].Text;
            var discountStr = worksheet.Cells[row, 6].Text;

            if (string.IsNullOrWhiteSpace(productIdStr) || !Guid.TryParse(productIdStr, out var productId))
                continue; // Invalid row

            if (string.IsNullOrWhiteSpace(quantityStr) || !int.TryParse(quantityStr, out var quantity))
                continue; // Invalid row

            decimal? unitPrice = null;
            if (!string.IsNullOrWhiteSpace(unitPriceStr) && decimal.TryParse(unitPriceStr, out var parsedPrice))
                unitPrice = parsedPrice;

            decimal discount = 0;
            if (!string.IsNullOrWhiteSpace(discountStr) && decimal.TryParse(discountStr, out var parsedDiscount))
                discount = parsedDiscount;

            results.Add(new ParsedExcelResultDto
            {
                RequestNo = requestNoStr,
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Discount = discount
            });
        }

        return results;
    }
}
