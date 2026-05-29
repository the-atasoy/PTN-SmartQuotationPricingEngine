using Application.Common.Models;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, ApiResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public CreateRequestCommandHandler(IApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var email = request.CustomerEmail.Trim().ToLowerInvariant();

        // 1. Find or create Customer
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

        if (customer == null)
        {
            return ApiResponse<Guid>.Fail(_localizer["CustomerNotFound"].Value, 404);
        }

        // 2. Generate request_no (RQ-YYYYMMDD-NNN)
        var today = DateTime.UtcNow.Date;
        var todayStr = today.ToString("yyyyMMdd");
        
        var countToday = await _context.Requests
            .Where(r => r.RequestNo.StartsWith($"RQ-{todayStr}"))
            .CountAsync(cancellationToken);

        var requestNo = $"RQ-{todayStr}-{(countToday + 1):D3}";

        // 3. Create Request
        var newRequest = Request.Create(requestNo, customer.Id);

        // 4. Create RequestItem rows
        foreach (var item in request.Items)
        {
            newRequest.AddItem(item.ProductId, item.Quantity, 0); // initial unit price is 0
        }

        _context.Requests.Add(newRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Return the Request ID
        return ApiResponse<Guid>.Success(newRequest.Id);
    }
}
