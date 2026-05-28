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
        var sheetName = _localizer["Excel_SheetName"] ?? "Quotation Request";
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        // Headers
        worksheet.Cells[1, 1].Value = _localizer["Excel_RequestNo"] ?? "Request No";
        worksheet.Cells[1, 2].Value = _localizer["Excel_ProductId"] ?? "Product ID";
        worksheet.Cells[1, 3].Value = _localizer["Excel_ProductName"] ?? "Product Name";
        worksheet.Cells[1, 4].Value = _localizer["Excel_Quantity"] ?? "Quantity";

        // Make headers bold
        using (var range = worksheet.Cells[1, 1, 1, 4])
        {
            range.Style.Font.Bold = true;
        }

        var generalCategory = _localizer["Excel_General"] ?? "General";

        // Rows
        int row = 2;
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
}
