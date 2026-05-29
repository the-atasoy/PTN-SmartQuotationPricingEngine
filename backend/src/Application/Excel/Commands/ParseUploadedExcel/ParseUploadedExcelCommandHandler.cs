using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Resources;
using Microsoft.Extensions.Localization;

namespace Application.Excel.Commands.ParseUploadedExcel;

public class ParseUploadedExcelCommandHandler : IRequestHandler<ParseUploadedExcelCommand, ApiResponse<List<ParsedExcelResultDto>>>
{
    private readonly IExcelService _excelService;
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ParseUploadedExcelCommandHandler(IExcelService excelService, IApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _excelService = excelService;
        _context = context;
        _localizer = localizer;
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
            return ApiResponse<List<ParsedExcelResultDto>>.Fail(_localizer["ExcelParseFailed", ex.Message].Value, 400);
        }

        if (!parsedRows.Any())
        {
            return ApiResponse<List<ParsedExcelResultDto>>.Fail(_localizer["ExcelEmpty"].Value, 422);
        }

        var req = await _context.Requests.FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);
        if (req == null)
        {
            return ApiResponse<List<ParsedExcelResultDto>>.Fail(_localizer["RequestNotFound"].Value, 404);
        }

        if (parsedRows.Any(r => r.RequestNo != req.RequestNo))
        {
            return ApiResponse<List<ParsedExcelResultDto>>.Fail(_localizer["ExcelNotBelongToRequest"].Value, 400);
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
                row.BasePrice = product.BasePrice;
            }
        }

        return ApiResponse<List<ParsedExcelResultDto>>.Success(parsedRows);
    }
}
