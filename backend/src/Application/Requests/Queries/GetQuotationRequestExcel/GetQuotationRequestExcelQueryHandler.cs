using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Queries.GetQuotationRequestExcel;

public class GetQuotationRequestExcelQueryHandler : IRequestHandler<GetQuotationRequestExcelQuery, ApiResponse<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;

    public GetQuotationRequestExcelQueryHandler(IApplicationDbContext context, IExcelService excelService)
    {
        _context = context;
        _excelService = excelService;
    }

    public async Task<ApiResponse<byte[]>> Handle(GetQuotationRequestExcelQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Requests
            .Include(r => r.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null)
            return ApiResponse<byte[]>.Fail("Request not found.", 404);

        try
        {
            var excelData = _excelService.GenerateQuotationRequestExcel(entity);
            return ApiResponse<byte[]>.Success(excelData);
        }
        catch (Exception ex)
        {
            return ApiResponse<byte[]>.Fail($"Failed to generate Excel: {ex.Message}", 500);
        }
    }
}
