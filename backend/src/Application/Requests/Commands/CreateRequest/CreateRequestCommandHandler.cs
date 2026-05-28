using Application.Common.Models;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, ApiResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var email = request.CustomerEmail.Trim().ToLowerInvariant();

        // 1. Find or create Customer
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

        if (customer == null)
        {
            // If customer doesn't exist, create a basic one using the email prefix as name if no name is available
            var name = email.Split('@')[0];
            customer = Customer.Create(name, email);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken); // Save to generate ID
        }

        // 2. Generate request_no (RQ-YYYYMMDD-NNN)
        var today = DateTime.UtcNow.Date;
        var todayStr = today.ToString("yyyyMMdd");
        
        var countToday = await _context.Requests
            .Where(r => r.RequestNo.StartsWith($"RQ-{todayStr}"))
            .CountAsync(cancellationToken);

        var requestNo = $"RQ-{todayStr}-{(countToday + 1):D3}";

        if (!Enum.TryParse<Currency>(request.Currency, out var currency))
        {
            return ApiResponse<Guid>.Fail("Invalid currency format.", 422);
        }

        // 3. Create Request
        var newRequest = Request.Create(requestNo, customer.Id, currency);

        // 4. Create RequestItem rows
        foreach (var item in request.Items)
        {
            newRequest.AddItem(item.ProductId, item.Quantity, 0, 0); // initial unit price is 0
        }

        _context.Requests.Add(newRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Return the Request ID
        return ApiResponse<Guid>.Success(newRequest.Id);
    }
}
