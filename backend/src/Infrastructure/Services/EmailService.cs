using Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public EmailService(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _configuration["Email:SmtpHost"];
        var portStr = _configuration["Email:SmtpPort"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portStr) || !int.TryParse(portStr, out int port))
        {
            throw new InvalidOperationException("SMTP settings are not configured properly.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Smart Quotation Engine", "noreply@smartquotation.com"));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = body
        };

        using var client = new SmtpClient();

        // SSL Settings based on environment
        if(_hostEnvironment.IsDevelopment())
        {
            // Accept all SSL certificates (in case of local testing with Mailpit/etc.)
            client.ServerCertificateValidationCallback = (_, _, _, _) => true;
            await client.ConnectAsync(host, port);
        }
        else
        {
            // Enforce strict SSL/TLS validation in production environments
            client.ServerCertificateValidationCallback = null; // Uses default strict validation
            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
        }
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
