using Application.Common.Enums;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetPriceHistory;

public record GetPriceHistoryQuery(Guid ProductId) : PaginatedQuery, IRequest<ApiResponse<PaginatedResult<PriceHistoryDto>>>;
