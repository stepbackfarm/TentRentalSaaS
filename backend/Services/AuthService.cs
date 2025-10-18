using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

namespace TentRentalSaaS.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiDbContext _dbContext;
        private readonly IEmailService _emailService;

        public AuthService(ApiDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public async Task RequestLoginLinkAsync(string email)
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);

            if (customer != null)
            {
                var loginToken = new LoginToken
                {
                    Token = Guid.NewGuid().ToString("n"), // Generate a secure, random token
                    CustomerId = customer.CustomerId,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(15), // Token is valid for 15 minutes
                    IsUsed = false
                };

                _dbContext.LoginTokens.Add(loginToken);
                await _dbContext.SaveChangesAsync();

                // The base URL for the frontend portal login page.
                // In a real app, this would come from configuration.
                var loginUrl = $"https://tent-rental-hh1bx2kh8-davids-projects-15ffe845.vercel.app/portal/login?token={loginToken.Token}";

                var subject = "Your Secure Login Link";
                var body = $"<h1>Login to Your Account</h1>"
                         + $"<p>Click the link below to securely log in to your account. This link is valid for 15 minutes.</p>"
                         + $"<a href=\"{loginUrl}\">Click here to log in</a>";

                await _emailService.SendEmailAsync(customer.Email, subject, body);
            }
            // If the customer is not found, we do nothing. This prevents attackers from discovering valid email addresses.
        }

        public async Task<PortalDataDto> VerifyLoginTokenAsync(string token)
        {
            var loginToken = await _dbContext.LoginTokens
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiryDate > DateTime.UtcNow);

            if (loginToken == null)
            {
                // Token is invalid, expired, or already used.
                return null;
            }

            // Mark the token as used so it cannot be used again.
            loginToken.IsUsed = true;
            await _dbContext.SaveChangesAsync();

            var customerBookings = await _dbContext.Bookings
                .Where(b => b.CustomerId == loginToken.CustomerId)
                .OrderByDescending(b => b.EventDate)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    EventDate = b.EventDate,
                    Status = b.Status,
                    TentType = b.TentType,
                    CustomerName = loginToken.Customer.FirstName + " " + loginToken.Customer.LastName,
                    CustomerEmail = loginToken.Customer.Email,
                    StripePaymentIntentId = b.StripePaymentIntentId
                })
                .ToListAsync();

            var portalData = new PortalDataDto
            {
                CustomerName = loginToken.Customer.FirstName + " " + loginToken.Customer.LastName,
                CustomerEmail = loginToken.Customer.Email,
                Bookings = customerBookings
            };

            return portalData;
        }
    }
}
