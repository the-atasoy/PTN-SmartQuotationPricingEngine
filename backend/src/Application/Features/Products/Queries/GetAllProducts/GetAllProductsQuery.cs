using Application.Common.Enums;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(int Page = 0, int PageSize = 10, string? SortColumn = "Name", SortDirection? SortDirection = null) : IRequest<ApiResponse<PaginatedResult<ProductDto>>>;
