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
            var server = Environment.GetEnvironmentVariable("SMTP_SERVER");
            var port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            var from = Environment.GetEnvironmentVariable("SMTP_FROM");
            var username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            _logger.LogInformation($"Attempting to send email via SMTP. Server: {server}, Port: {port}, From: {from}, Username: {username}");

            if (string.IsNullOrEmpty(password))
            {
                _logger.LogError("SMTP_PASSWORD environment variable not set.");
                throw new InvalidOperationException("SMTP_PASSWORD environment variable not set.");
            }

            var client = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
