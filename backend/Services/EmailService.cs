using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace TentRentalSaaS.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                _logger.LogError("EmailService.SendEmailAsync called with empty 'to' address.");
                throw new ArgumentException("Recipient email address is required.", nameof(to));
            }

            var server = Environment.GetEnvironmentVariable("SMTP_SERVER");
            var port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            var from = Environment.GetEnvironmentVariable("SMTP_FROM");
            var username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            if (string.IsNullOrWhiteSpace(server))
            {
                _logger.LogError("SMTP_SERVER environment variable not set.");
                throw new InvalidOperationException("SMTP_SERVER environment variable not set.");
            }
            if (string.IsNullOrWhiteSpace(from))
            {
                _logger.LogError("SMTP_FROM environment variable not set.");
                throw new InvalidOperationException("SMTP_FROM environment variable not set.");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogError("SMTP_USERNAME environment variable not set.");
                throw new InvalidOperationException("SMTP_USERNAME environment variable not set.");
            }
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogError("SMTP_PASSWORD environment variable not set.");
                throw new InvalidOperationException("SMTP_PASSWORD environment variable not set.");
            }

            using var client = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(from!),
                Subject = subject ?? string.Empty,
                Body = body ?? string.Empty,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
