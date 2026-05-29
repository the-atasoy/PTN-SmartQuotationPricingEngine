using Application.Common.Extensions;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ApiResponse<PaginatedResult<ProductDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaginatedResult<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .Sort(request.SortColumn, request.SortDirection, "Name")
            .Paginate(request.Page, request.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                BasePrice = p.BasePrice,
                LastRequestPrice = request.IsAdmin ? p.LastRequestPrice : null,
                LastRequestCurrency = request.IsAdmin ? p.LastRequestCurrency : null,
                LastRequestDate = request.IsAdmin ? p.LastRequestDate : null
            })
            .ToListAsync(cancellationToken);

        var paginatedResult = PaginatedResult<ProductDto>.Create(products, request.Page, request.PageSize, totalCount);

        return ApiResponse<PaginatedResult<ProductDto>>.Success(paginatedResult);
    }
}
