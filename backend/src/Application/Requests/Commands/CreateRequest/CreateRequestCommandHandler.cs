using Application.Common.Models;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;

namespace Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, ApiResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public CreateRequestCommandHandler(
        IApplicationDbContext context, 
        IStringLocalizer<SharedResource> localizer,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _localizer = localizer;
        _emailService = emailService;
        _configuration = configuration;
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
        var newRequest = Request.Create(requestNo, customer.Id, request.Currency);

        // 4. Create RequestItem rows
        foreach (var item in request.Items)
        {
            newRequest.AddItem(item.ProductId, item.Quantity, 0); // initial unit price is 0
        }

        _context.Requests.Add(newRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // 5. Notify Admins
        await NotifyAdminsAsync(newRequest, customer, cancellationToken);

        // 6. Return the Request ID
        return ApiResponse<Guid>.Success(newRequest.Id);
    }

    private async Task NotifyAdminsAsync(Request newRequest, Customer customer, CancellationToken cancellationToken)
    {
        var adminEmails = await _context.Users
            .Where(u => u.Role == UserRole.Admin)
            .Select(u => u.Email)
            .ToListAsync(cancellationToken);

        if (!adminEmails.Any())
            return;

        var frontendUrl = _configuration["FrontendUrl"];
        var requestLink = $"{frontendUrl}/admin/requests/{newRequest.Id}";
        var emailSubject = $"New Request Created: {newRequest.RequestNo}";
        
        var emailBody = $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>New Request {newRequest.RequestNo}</title>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 0; color: #333; }}
                    .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05); }}
                    .header {{ background-color: #050a18; padding: 30px; text-align: center; border-bottom: 4px solid #2980b9; }}
                    .header img {{ max-width: 150px; }}
                    .header h2 {{ color: #ffffff; margin: 20px 0 0 0; font-weight: 300; letter-spacing: 1px; }}
                    .content {{ padding: 40px 30px; }}
                    .greeting {{ font-size: 18px; font-weight: 600; margin-bottom: 20px; color: #2c3e50; }}
                    .intro-text {{ font-size: 15px; line-height: 1.6; color: #555; margin-bottom: 30px; }}
                    .details-box {{ background-color: #f8f9fa; border-left: 4px solid #2980b9; padding: 15px; margin-bottom: 30px; border-radius: 0 4px 4px 0; }}
                    .details-box p {{ margin: 5px 0; font-size: 14px; color: #555; }}
                    .button-container {{ text-align: center; margin: 30px 0; }}
                    .button {{ background-color: #2980b9; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: 600; display: inline-block; }}
                    .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #999; border-top: 1px solid #e9ecef; }}
                    .footer p {{ margin: 5px 0; }}
                    .footer a {{ color: #2980b9; text-decoration: none; }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header"">
                        <img src=""https://piton.com.tr/images/logo/piton-white-logo.svg"" alt=""PITON Technology"">
                        <h2>New Request Received</h2>
                    </div>
                    <div class=""content"">
                        <p class=""greeting"">Hello Admin,</p>
                        <p class=""intro-text"">A new quotation request has been created in the system.</p>
                        
                        <div class=""details-box"">
                            <p><strong>Request No:</strong> {newRequest.RequestNo}</p>
                            <p><strong>Customer:</strong> {customer.Name}</p>
                            <p><strong>Email:</strong> {customer.Email}</p>
                        </div>
                        
                        <div class=""button-container"">
                            <a href=""{requestLink}"" class=""button"">View Request Details</a>
                        </div>
                    </div>
                    <div class=""footer"">
                        <p>This is an automated message. Please do not reply.</p>
                        <p>PITON Technology - Eskişehir Osmangazi Üniversitesi Meşelik Kampüsü, ETGB Teknoparkı No:202</p>
                        <p><a href=""https://piton.com.tr/"">www.piton.com.tr</a></p>
                    </div>
                </div>
            </body>
            </html>
        ";

        foreach (var adminEmail in adminEmails)
        {
            try
            {
                await _emailService.SendEmailAsync(adminEmail, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                // Optionally log the exception so one failing email doesn't stop others, 
                // but since we don't have logger injected, we'll let it bubble or just swallow.
                // It's better to add ILogger, but for now we'll just ignore individual failures 
                // to not interrupt the request flow completely if one email fails.
                Console.WriteLine($"Error sending email to {adminEmail}: {ex.Message}");
            }
        }
    }
}
