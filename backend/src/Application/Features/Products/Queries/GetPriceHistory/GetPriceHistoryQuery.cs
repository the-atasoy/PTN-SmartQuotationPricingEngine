using Application.Common.Enums;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetPriceHistory;

public record GetPriceHistoryQuery(Guid ProductId, int Page = 0, int PageSize = 10, string? SortColumn = null, SortDirection? SortDirection = null) : IRequest<ApiResponse<PaginatedResult<PriceHistoryDto>>>;
