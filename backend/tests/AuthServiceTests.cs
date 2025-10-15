using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Services;
using Xunit;

namespace TentRentalSaaS.Api.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;

        public AuthServiceTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
        }

        private ApiDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApiDbContext(options);
        }

        [Fact]
        public async Task RequestLoginLinkAsync_WithValidCustomer_CreatesTokenAndSendsEmail()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var customer = new Customer { CustomerId = Guid.NewGuid(), Email = "test@example.com" };
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();

            var authService = new AuthService(dbContext, _emailServiceMock.Object);

            // Act
            await authService.RequestLoginLinkAsync("test@example.com");

            // Assert
            var token = await dbContext.LoginTokens.FirstOrDefaultAsync();
            Assert.NotNull(token);
            Assert.Equal(customer.CustomerId, token.CustomerId);
            _emailServiceMock.Verify(m => m.SendEmailAsync("test@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RequestLoginLinkAsync_WithInvalidCustomer_DoesNothing()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var authService = new AuthService(dbContext, _emailServiceMock.Object);

            // Act
            await authService.RequestLoginLinkAsync("nonexistent@example.com");

            // Assert
            var tokenCount = await dbContext.LoginTokens.CountAsync();
            Assert.Equal(0, tokenCount);
            _emailServiceMock.Verify(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task VerifyLoginTokenAsync_WithValidToken_ReturnsPortalDataAndMarksTokenAsUsed()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var customer = new Customer { CustomerId = Guid.NewGuid(), Email = "test@example.com", FirstName = "Test", LastName = "User" };
            var token = new LoginToken { Token = "valid_token", Customer = customer, ExpiryDate = DateTime.UtcNow.AddHours(1), IsUsed = false };
            dbContext.Customers.Add(customer);
            dbContext.LoginTokens.Add(token);
            await dbContext.SaveChangesAsync();

            var authService = new AuthService(dbContext, _emailServiceMock.Object);

            // Act
            var result = await authService.VerifyLoginTokenAsync("valid_token");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test User", result.CustomerName);
            var updatedToken = await dbContext.LoginTokens.FirstAsync();
            Assert.True(updatedToken.IsUsed);
        }

        [Fact]
        public async Task VerifyLoginTokenAsync_WithInvalidToken_ReturnsNull()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var authService = new AuthService(dbContext, _emailServiceMock.Object);

            // Act
            var result = await authService.VerifyLoginTokenAsync("invalid_token");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task VerifyLoginTokenAsync_WithExpiredToken_ReturnsNull()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var customer = new Customer { CustomerId = Guid.NewGuid(), Email = "test@example.com" };
            var token = new LoginToken { Token = "expired_token", Customer = customer, ExpiryDate = DateTime.UtcNow.AddMinutes(-1), IsUsed = false };
            dbContext.Customers.Add(customer);
            dbContext.LoginTokens.Add(token);
            await dbContext.SaveChangesAsync();

            var authService = new AuthService(dbContext, _emailServiceMock.Object);

            // Act
            var result = await authService.VerifyLoginTokenAsync("expired_token");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task VerifyLoginTokenAsync_WithUsedToken_ReturnsNull()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var customer = new Customer { CustomerId = Guid.NewGuid(), Email = "test@example.com" };
            var token = new LoginToken { Token = "used_token", Customer = customer, ExpiryDate = DateTime.UtcNow.AddHours(1), IsUsed = true };
            dbContext.Customers.Add(customer);
            dbContext.LoginTokens.Add(token);
            await dbContext.SaveChangesAsync();

            var authService = new AuthService(dbContext, _emailServiceMock.Object);

            // Act
            var result = await authService.VerifyLoginTokenAsync("used_token");

            // Assert
            Assert.Null(result);
        }
    }
}
