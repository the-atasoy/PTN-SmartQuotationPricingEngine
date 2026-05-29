using Application.Common.Models;
using MediatR;

namespace Application.Products.Queries.GetProductPriceHistory;

public class GetProductPriceHistoryQuery : IRequest<ApiResponse<PaginatedResult<PriceHistoryDto>>>
{
    public Guid ProductId { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public Common.Enums.SortDirection? SortDirection { get; set; }
}
