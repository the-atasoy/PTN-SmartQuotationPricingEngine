using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Resources;
using Microsoft.Extensions.Localization;

namespace Application.Requests.Queries.GetRequestById;

public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, ApiResponse<RequestDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public GetRequestByIdQueryHandler(IApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _localizer = localizer;
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
            return ApiResponse<RequestDetailDto>.Fail(_localizer["RequestNotFound"].Value, 404);
        }

        var dto = new RequestDetailDto
        {
            Id = entity.Id,
            RequestNo = entity.RequestNo,
            CustomerId = entity.CustomerId,
            CustomerName = entity.Customer.Name,
            CustomerEmail = entity.Customer.Email,
            TotalAmount = entity.TotalAmount,
            Currency = entity.Currency,
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
                Discount = i.Discount,
                LastRequestPrice = i.Product.LastRequestPrice,
                LastRequestCurrency = i.Product.LastRequestCurrency,
                LastRequestDate = i.Product.LastRequestDate
            }).ToList()
        };

        return ApiResponse<RequestDetailDto>.Success(dto);
    }
}
