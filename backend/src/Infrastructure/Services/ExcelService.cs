using Application.Excel.Commands.ParseUploadedExcel;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ExcelService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
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

        // Make headers bold
        using (var range = worksheet.Cells[1, 1, 1, 4])
        {
            range.Style.Font.Bold = true;
        }


        // Rows
        var row = 2;
        foreach (var item in request.Items)
        {
            worksheet.Cells[row, 1].Value = request.RequestNo;
            worksheet.Cells[row, 2].Value = item.ProductId.ToString();
            worksheet.Cells[row, 3].Value = item.Product.Name;
            worksheet.Cells[row, 4].Value = item.Quantity;
            row++;
        }

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
            var productIdStr = worksheet.Cells[row, 2].Text;
            var productName = worksheet.Cells[row, 3].Text;
            var quantityStr = worksheet.Cells[row, 4].Text;

            if (string.IsNullOrWhiteSpace(productIdStr) || !Guid.TryParse(productIdStr, out var productId))
                continue; // Invalid row

            if (string.IsNullOrWhiteSpace(quantityStr) || !int.TryParse(quantityStr, out var quantity))
                continue; // Invalid row

            results.Add(new ParsedExcelResultDto
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity
            });
        }

        return results;
    }
}
