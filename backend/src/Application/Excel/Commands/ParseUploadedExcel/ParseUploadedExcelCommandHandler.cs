using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Excel.Commands.ParseUploadedExcel;

public class ParseUploadedExcelCommandHandler : IRequestHandler<ParseUploadedExcelCommand, ApiResponse<List<ParsedExcelResultDto>>>
{
    private readonly IExcelService _excelService;
    private readonly IApplicationDbContext _context;

    public ParseUploadedExcelCommandHandler(IExcelService excelService, IApplicationDbContext context)
    {
        _excelService = excelService;
        _context = context;
    }

    public async Task<ApiResponse<List<ParsedExcelResultDto>>> Handle(ParseUploadedExcelCommand request, CancellationToken cancellationToken)
    {
        List<ParsedExcelResultDto> parsedRows;
        try
        {
            parsedRows = _excelService.ParseExcel(request.ExcelStream);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ParsedExcelResultDto>>.Fail($"Failed to parse Excel file: {ex.Message}", 400);
        }

        if (!parsedRows.Any())
        {
            return ApiResponse<List<ParsedExcelResultDto>>.Fail("Excel file is empty or missing required columns.", 422);
        }

        var req = await _context.Requests.FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);
        if (req == null)
        {
            return ApiResponse<List<ParsedExcelResultDto>>.Fail("Request not found", 404);
        }

        if (parsedRows.Any(r => r.RequestNo != req.RequestNo))
        {
            return ApiResponse<List<ParsedExcelResultDto>>.Fail("Uploaded Excel file does not belong to this request.", 400);
        }

        var productIds = parsedRows.Select(r => r.ProductId).Distinct().ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var row in parsedRows)
        {
            if (products.TryGetValue(row.ProductId, out var product))
            {
                row.LastRequestPrice = product.LastRequestPrice;
                row.LastRequestCurrency = product.LastRequestCurrency;
                row.LastRequestDate = product.LastRequestDate;
                row.HasPreviousPrice = product.LastRequestPrice.HasValue && product.LastRequestCurrency.HasValue;
            }
        }

        return ApiResponse<List<ParsedExcelResultDto>>.Success(parsedRows);
    }
}
