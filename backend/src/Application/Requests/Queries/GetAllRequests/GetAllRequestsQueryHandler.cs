using Application.Common.Extensions;
using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Queries.GetAllRequests;

public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, ApiResponse<PaginatedResult<RequestListItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllRequestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaginatedResult<RequestListItemDto>>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Requests
            .Include(r => r.Customer)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Sort(request.SortColumn, request.SortDirection, "CreatedAt", true)
            .Paginate(request.Page, request.PageSize)
            .Select(r => new RequestListItemDto
            {
                Id = r.Id,
                RequestNo = r.RequestNo,
                CustomerName = r.Customer.Name,
                CustomerEmail = r.Customer.Email,
                TotalAmount = r.TotalAmount,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                LastModified = r.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        var paginatedResult = PaginatedResult<RequestListItemDto>.Create(items, request.Page, request.PageSize, totalCount);

        return ApiResponse<PaginatedResult<RequestListItemDto>>.Success(paginatedResult);
    }
}
