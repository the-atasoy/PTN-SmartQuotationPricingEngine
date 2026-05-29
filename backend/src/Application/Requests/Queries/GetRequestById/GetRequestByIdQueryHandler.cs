using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Queries.GetRequestById;

public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, ApiResponse<RequestDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<RequestDetailDto>> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Requests
            .Include(r => r.Customer)
            .Include(r => r.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return ApiResponse<RequestDetailDto>.Fail("Request not found", 404);
        }

        var dto = new RequestDetailDto
        {
            Id = entity.Id,
            RequestNo = entity.RequestNo,
            CustomerId = entity.CustomerId,
            CustomerName = entity.Customer.Name,
            CustomerEmail = entity.Customer.Email,
            TotalAmount = entity.TotalAmount,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            LastModified = entity.UpdatedAt,
            Items = entity.Items.Select(i => new RequestItemDetailDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal,
                LastRequestPrice = i.Product.LastRequestPrice,
                LastRequestDate = i.Product.LastRequestDate
            }).ToList()
        };

        return ApiResponse<RequestDetailDto>.Success(dto);
    }
}
