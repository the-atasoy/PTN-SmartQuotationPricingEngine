using Application.Common.Extensions;
using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProductPriceHistory;

public class GetProductPriceHistoryQueryHandler : IRequestHandler<GetProductPriceHistoryQuery, ApiResponse<PaginatedResult<PriceHistoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductPriceHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaginatedResult<PriceHistoryDto>>> Handle(GetProductPriceHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ProductPriceHistories
            .Include(h => h.Request)
                .ThenInclude(r => r.Customer)
            .Where(h => h.ProductId == request.ProductId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Sort(request.SortColumn, request.SortDirection, "CreatedAt", true)
            .Paginate(request.Page, request.PageSize)
            .Select(h => new PriceHistoryDto
            {
                Date = h.CreatedAt,
                Price = h.Price,
                Currency = h.Currency,
                RequestNo = h.Request.RequestNo,
                CustomerName = h.Request.Customer.Name
            })
            .ToListAsync(cancellationToken);

        var paginatedResult = PaginatedResult<PriceHistoryDto>.Create(items, request.Page, request.PageSize, totalCount);

        return ApiResponse<PaginatedResult<PriceHistoryDto>>.Success(paginatedResult);
    }
}
