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

        var productIds = parsedRows.Select(r => r.ProductId).Distinct().ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var row in parsedRows)
        {
            if (products.TryGetValue(row.ProductId, out var product))
            {
                row.LastRequestPrice = product.LastRequestPrice;
                row.LastRequestDate = product.LastRequestDate;
                row.HasPreviousPrice = product.LastRequestPrice.HasValue;
            }
        }

        return ApiResponse<List<ParsedExcelResultDto>>.Success(parsedRows);
    }
}
