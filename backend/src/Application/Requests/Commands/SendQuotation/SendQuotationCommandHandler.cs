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
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Quotation {entity.RequestNo}</title>
                    <style>
                        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 0; color: #333; }}
                        .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05); }}
                        .header {{ background-color: #050a18; padding: 30px; text-align: center; border-bottom: 4px solid #2980b9; }}
                        .header img {{ max-width: 150px; }}
                        .header h2 {{ color: #ffffff; margin: 20px 0 0 0; font-weight: 300; letter-spacing: 1px; }}
                        .content {{ padding: 40px 30px; }}
                        .greeting {{ font-size: 18px; font-weight: 600; margin-bottom: 20px; color: #2c3e50; }}
                        .intro-text {{ font-size: 15px; line-height: 1.6; color: #555; margin-bottom: 30px; }}
                        .table-container {{ overflow-x: auto; }}
                        table {{ width: 100%; border-collapse: collapse; margin-bottom: 30px; border-radius: 4px; overflow: hidden; }}
                        th {{ background-color: #f8f9fa; color: #2c3e50; font-weight: 600; text-transform: uppercase; font-size: 12px; padding: 15px; text-align: left; border-bottom: 2px solid #e9ecef; }}
                        td {{ padding: 15px; border-bottom: 1px solid #e9ecef; font-size: 14px; color: #555; }}
                        .grand-total {{ text-align: right; background-color: #f8f9fa; padding: 20px 30px; border-top: 2px solid #e9ecef; font-size: 20px; color: #2c3e50; font-weight: bold; }}
                        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #999; border-top: 1px solid #e9ecef; }}
                        .footer p {{ margin: 5px 0; }}
                        .footer a {{ color: #2980b9; text-decoration: none; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <img src=""https://piton.com.tr/images/logo/piton-white-logo.svg"" alt=""PITON Technology"">
                            <h2>{_localizer["Email_QuotationDetails"].Value}</h2>
                        </div>
                        <div class=""content"">
                            <p class=""greeting"">{_localizer["Email_Hello", entity.Customer.Name].Value}</p>
                            <p class=""intro-text"">{_localizer["Email_QuotationIntro", entity.RequestNo].Value}</p>
                            
                            <div class=""table-container"">
                                <table>
                                    <thead>
                                        <tr>
                                            <th>{_localizer["Email_Product"].Value}</th>
                                            <th>{_localizer["Email_Quantity"].Value}</th>
                                            <th>{_localizer["Email_UnitPrice"].Value}</th>
                                            <th>{_localizer["Email_Discount"].Value}</th>
                                            <th>{_localizer["Email_Total"].Value}</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {tableRows}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class=""grand-total"">
                            {_localizer["Email_GrandTotal", entity.TotalAmount.ToString("C"), entity.Currency].Value}
                        </div>
                        <div class=""footer"">
                            <p>{_localizer["Email_ThankYou"].Value}</p>
                            <p>PITON Technology - Eskişehir Osmangazi Üniversitesi Meşelik Kampüsü, ETGB Teknoparkı No:202</p>
                            <p><a href=""https://piton.com.tr/"">www.piton.com.tr</a></p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var subject = _localizer["Email_Subject", entity.RequestNo].Value;
            if (string.IsNullOrEmpty(subject) || subject == "Email_Subject")
            {
                subject = $"Quotation {entity.RequestNo}";
            }

            await _emailService.SendEmailAsync(entity.Customer.Email, subject, htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email for request {RequestId}", entity.Id);
            // We return success because DB commit was successful, but log the email failure.
        }

        return ApiResponse.Success();
    }
}
