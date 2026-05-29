using Application.Common.Extensions;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using Application.Interfaces;
using Application.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Products.Queries.GetPriceHistory;

public class GetPriceHistoryQueryHandler : IRequestHandler<GetPriceHistoryQuery, ApiResponse<PaginatedResult<PriceHistoryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public GetPriceHistoryQueryHandler(IApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<ApiResponse<PaginatedResult<PriceHistoryDto>>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists)
        {
            return ApiResponse<PaginatedResult<PriceHistoryDto>>.Fail(_localizer["ProductNotFound"].Value, 404);
        }

        var query = _context.ProductPriceHistories
            .Include(ph => ph.Request)
            .ThenInclude(r => r.Customer)
            .Where(ph => ph.ProductId == request.ProductId)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var history = await query
            .Sort(request.SortColumn, request.SortDirection, "CreatedAt", true)
            .Paginate(request.Page, request.PageSize)
            .Select(ph => new PriceHistoryDto
            {
                Id = ph.Id,
                RequestId = ph.RequestId,
                RequestNo = ph.Request.RequestNo,
                CustomerName = ph.Request.Customer.Name,
                Price = ph.Price,
                Currency = ph.Currency,
                CreatedAt = ph.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var paginatedResult = PaginatedResult<PriceHistoryDto>.Create(history, request.Page, request.PageSize, totalCount);

        return ApiResponse<PaginatedResult<PriceHistoryDto>>.Success(paginatedResult);
    }
}
