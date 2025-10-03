using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Stripe;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Services;
using Xunit;

namespace TentRentalSaaS.Api.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IPaymentService> _paymentServiceMock;

        public BookingServiceTests()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _paymentServiceMock.Setup(p => p.CreatePaymentIntentAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentIntent { Id = "pi_test_123" });
        }

        private ApiDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApiDbContext(options);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateNewCustomer_WhenCustomerDoesNotExist()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var bookingService = new BookingService(dbContext, _paymentServiceMock.Object);
            var bookingRequest = new BookingRequestDto
            {
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                PaymentMethodId = "pm_card_visa"
            };

            // Act
            await bookingService.CreateBookingAsync(bookingRequest);

            // Assert
            Assert.Equal(1, dbContext.Customers.Count());
            var customer = await dbContext.Customers.FirstAsync();
            Assert.Equal("john.doe@example.com", customer.Email);
            Assert.Equal("John", customer.FirstName);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldUseExistingCustomer_WhenCustomerExists()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var existingCustomer = new TentRentalSaaS.Api.Models.Customer { CustomerId = Guid.NewGuid(), Email = "jane.doe@example.com", FirstName = "Jane" };
            dbContext.Customers.Add(existingCustomer);
            await dbContext.SaveChangesAsync();

            var bookingService = new BookingService(dbContext, _paymentServiceMock.Object);
            var bookingRequest = new BookingRequestDto
            {
                CustomerName = "Jane Doe",
                CustomerEmail = "jane.doe@example.com",
                PaymentMethodId = "pm_card_visa"
            };

            // Act
            await bookingService.CreateBookingAsync(bookingRequest);

            // Assert
            Assert.Equal(1, dbContext.Customers.Count()); // Should not add a new customer
            Assert.Equal(1, dbContext.Bookings.Count());
            var booking = await dbContext.Bookings.Include(b => b.Customer).FirstAsync();
            Assert.Equal(existingCustomer.CustomerId, booking.CustomerId);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldReturnCorrectlyMappedDto()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var bookingService = new BookingService(dbContext, _paymentServiceMock.Object);
            var bookingRequest = new BookingRequestDto
            {
                CustomerName = "Sam Smith",
                CustomerEmail = "sam.smith@example.com",
                TentType = "Large",
                NumberOfTents = 2,
                PaymentMethodId = "pm_card_visa"
            };

            // Act
            var resultDto = await bookingService.CreateBookingAsync(bookingRequest);

            // Assert
            Assert.NotNull(resultDto);
            Assert.Equal("Sam Smith", resultDto.CustomerName);
            Assert.Equal("sam.smith@example.com", resultDto.CustomerEmail);
            Assert.Equal("Large", resultDto.TentType);
            Assert.Equal(2, resultDto.NumberOfTents);
            Assert.Equal("Confirmed", resultDto.Status);
            Assert.Equal("pi_test_123", resultDto.StripePaymentIntentId);
        }
    }
}