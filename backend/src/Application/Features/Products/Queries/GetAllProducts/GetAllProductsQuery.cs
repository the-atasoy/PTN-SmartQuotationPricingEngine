using Application.Common.Enums;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : PaginatedQuery, IRequest<ApiResponse<PaginatedResult<ProductDto>>>
{
    public GetAllProductsQuery()
    {
        SortColumn = "Name";
    }
}
