using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Helpers;

namespace TentRentalSaaS.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApiDbContext dbContext, IEmailService emailService, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        // Backward-compatible constructor for tests or simple scenarios
        public AuthService(ApiDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            _logger = new LoggerFactory().CreateLogger<AuthService>(); // Fallback for tests
        }

        public async Task RequestLoginLinkAsync(string email)
        {
            _logger.LogInformation("RequestLoginLinkAsync called for email: {Email}", email);

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);

            if (customer != null)
            {
                // Invalidate all other unused tokens for this user
                var existingTokens = await _dbContext.LoginTokens
                    .Where(t => t.CustomerId == customer.CustomerId && !t.IsUsed)
                    .ToListAsync();

                foreach (var existingToken in existingTokens)
                {
                    existingToken.IsUsed = true;
                    _logger.LogInformation("Invalidating old token: {Token}", existingToken.Token);
                }

                var loginToken = new LoginToken
                {
                    Token = Guid.NewGuid().ToString("n"), // Generate a secure, random token
                    CustomerId = customer.CustomerId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    ExpiryDate = DateTimeOffset.UtcNow.AddMinutes(15), // Token is valid for 15 minutes
                    IsUsed = false
                };

                _dbContext.LoginTokens.Add(loginToken);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("New login token created and saved. Token: {Token}, CreatedDate: {CreatedDate}, ExpiryDate: {ExpiryDate}",
                    loginToken.Token, loginToken.CreatedDate, loginToken.ExpiryDate);

                var frontendBaseUrl = _configuration["FRONTEND_BASE_URL"];
                var loginUrl = $"{frontendBaseUrl}/portal/login?token={loginToken.Token}";

                var subject = "Your Secure Login Link";
                var body = $"<h1>Login to Your Account</h1>"
                         + $"<p>Click the link below to securely log in to your account. This link is valid for 15 minutes.</p>"
                         + $"<a href=\"{loginUrl}\">Click here to log in</a>";

                await _emailService.SendEmailAsync(customer.Email, subject, body);
                _logger.LogInformation("Login link email sent to {Email}", email);
            }
            else
            {
                _logger.LogInformation("Login link requested for non-existent email: {Email}", email);
            }
        }

        public async Task<PortalDataDto> VerifyLoginTokenAsync(string token)
        {
            _logger.LogInformation("VerifyLoginTokenAsync called with token prefix: {TokenPrefix}***", 
                token?.Substring(0, Math.Min(8, token?.Length ?? 0)));

            // Get all active tokens and perform constant-time comparison
            // to prevent timing attacks that could leak token information
            var activeTokens = await _dbContext.LoginTokens
                .Include(t => t.Customer)
                .Where(t => !t.IsUsed && t.ExpiryDate > DateTimeOffset.UtcNow)
                .ToListAsync();

            var loginToken = activeTokens.FirstOrDefault(t => 
                SecurityHelper.SecureStringEquals(t.Token, token));

            if (loginToken == null)
            {
                _logger.LogWarning("SECURITY: Failed login attempt - invalid or expired token");
                return null;
            }

            _logger.LogInformation("Login token found. Token: {Token}, IsUsed: {IsUsed}, ExpiryDate: {ExpiryDate}",
                loginToken.Token, loginToken.IsUsed, loginToken.ExpiryDate);

            // Re-add the original checks for debugging purposes
            if (loginToken.IsUsed)
            {
                _logger.LogWarning("Login token {Token} is already used.", token);
                return null;
            }

            if (loginToken.ExpiryDate <= DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Login token {Token} has expired. ExpiryDate: {ExpiryDate}, UtcNow: {UtcNow}",
                    token, loginToken.ExpiryDate, DateTimeOffset.UtcNow);
                return null;
            }

            // Mark the token as used so it cannot be used again.
            loginToken.IsUsed = true;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Login token {Token} successfully verified and marked as used.", token);

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
                    StripePaymentIntentId = b.StripePaymentIntentId ?? string.Empty
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
