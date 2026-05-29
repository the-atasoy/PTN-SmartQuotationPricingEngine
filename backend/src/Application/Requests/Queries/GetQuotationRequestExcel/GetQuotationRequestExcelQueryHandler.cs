using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Resources;
using Microsoft.Extensions.Localization;

namespace Application.Requests.Queries.GetQuotationRequestExcel;

public class GetQuotationRequestExcelQueryHandler : IRequestHandler<GetQuotationRequestExcelQuery, ApiResponse<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public GetQuotationRequestExcelQueryHandler(IApplicationDbContext context, IExcelService excelService, IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _excelService = excelService;
        _localizer = localizer;
    }

    public async Task<ApiResponse<byte[]>> Handle(GetQuotationRequestExcelQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Requests
            .Include(r => r.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null)
            return ApiResponse<byte[]>.Fail(_localizer["RequestNotFound"].Value, 404);

        try
        {
            var excelData = _excelService.GenerateQuotationRequestExcel(entity);
            return ApiResponse<byte[]>.Success(excelData);
        }
        catch (Exception ex)
        {
            return ApiResponse<byte[]>.Fail(_localizer["ExcelGenerateFailed", ex.Message].Value, 500);
        }
    }
}
