using Application.Common.Models;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Application.Resources;
using Microsoft.Extensions.Localization;

namespace Application.Requests.Commands.SendQuotation;

public class SendQuotationCommandHandler : IRequestHandler<SendQuotationCommand, ApiResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<SendQuotationCommandHandler> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SendQuotationCommandHandler(IApplicationDbContext context, IEmailService emailService, ILogger<SendQuotationCommandHandler> logger, IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<ApiResponse> Handle(SendQuotationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Requests
            .Include(r => r.Items)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null)
            return ApiResponse.Fail(_localizer["RequestNotFound"].Value, 404);

        if (entity.Status == RequestStatus.Sent)
            return ApiResponse.Fail(_localizer["RequestAlreadySent"].Value, 409);

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var now = DateTime.UtcNow;

            foreach (var inputItem in request.Items)
            {
                var requestItem = entity.Items.FirstOrDefault(i => i.ProductId == inputItem.ProductId);
                if (requestItem == null)
                    return ApiResponse.Fail(_localizer["ProductNotInRequest", inputItem.ProductId].Value, 400);

                // Update request item
                entity.UpdateItem(requestItem.Id, requestItem.Quantity, inputItem.UnitPrice, inputItem.Discount);

                if (products.TryGetValue(inputItem.ProductId, out var product))
                {
                    // Update product last price
                    product.UpdateLastRequestPrice(inputItem.UnitPrice, entity.Currency, now);

                    // Insert price history
                    var history = ProductPriceHistory.Create(product.Id, entity.Id, inputItem.UnitPrice, entity.Currency);
                    _context.ProductPriceHistories.Add(history);
                }
            }

            entity.MarkAsSent();

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Transaction failed while sending quotation.");
            return ApiResponse.Fail(_localizer["QuotationSaveError"].Value, 500);
        }

        // Send Email
        try
        {
            var tableRows = string.Join("", entity.Items.Select(i => 
                $"<tr><td>{products[i.ProductId].Name}</td><td>{i.Quantity}</td><td>{i.UnitPrice:C}</td><td>{i.Discount:C}</td><td>{i.LineTotal:C}</td></tr>"
            ));

            var htmlBody = $@"
                <h2>{_localizer["Email_QuotationDetails"].Value}</h2>
                <p>{_localizer["Email_Hello", entity.Customer.Name].Value}</p>
                <p>{_localizer["Email_QuotationIntro", entity.RequestNo].Value}</p>
                <table border='1' cellpadding='5' cellspacing='0'>
                    <thead>
                        <tr><th>{_localizer["Email_Product"].Value}</th><th>{_localizer["Email_Quantity"].Value}</th><th>{_localizer["Email_UnitPrice"].Value}</th><th>{_localizer["Email_Discount"].Value}</th><th>{_localizer["Email_Total"].Value}</th></tr>
                    </thead>
                    <tbody>
                        {tableRows}
                    </tbody>
                </table>
                <h3>{_localizer["Email_GrandTotal", entity.TotalAmount.ToString("C"), entity.Currency].Value}</h3>
                <p>{_localizer["Email_ThankYou"].Value}</p>
            ";

            await _emailService.SendEmailAsync(entity.Customer.Email, $"Quotation {entity.RequestNo}", htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email for request {RequestId}", entity.Id);
            // We return success because DB commit was successful, but log the email failure.
        }

        return ApiResponse.Success();
    }
}
