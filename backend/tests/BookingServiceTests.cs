using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.Extensions.Logging;
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
        private readonly Mock<IGeocodingService> _geocodingServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ILogger<BookingService>> _loggerMock;
        private readonly IConfiguration _configuration;

        public BookingServiceTests()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _paymentServiceMock.Setup(p => p.CreatePaymentIntentAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentIntent { Id = "pi_test_123" });

            _geocodingServiceMock = new Mock<IGeocodingService>();
            _geocodingServiceMock.Setup(g => g.GetCoordinatesAsync(It.IsAny<string>()))
                .ReturnsAsync((39.9612, -86.1581));

            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<BookingService>>();

            var inMemorySettings = new Dictionary<string, string> {
                {"GoogleMaps:ApiKey", "***REMOVED***"},
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
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
            var bookingService = new BookingService(dbContext, _paymentServiceMock.Object, _geocodingServiceMock.Object, _emailServiceMock.Object, _loggerMock.Object, _configuration);
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
            var bookingService = new BookingService(dbContext, _paymentServiceMock.Object, _geocodingServiceMock.Object, _emailServiceMock.Object, _loggerMock.Object, _configuration);
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
            var bookingService = new BookingService(dbContext, _paymentServiceMock.Object, _geocodingServiceMock.Object, _emailServiceMock.Object, _loggerMock.Object, _configuration);
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

        [Fact]
        public async Task CreateBookingAsync_ShouldSetEventLocation()
        {
            // Arrange
            await using var dbContext = GetDbContext();
            var bookingService = new BookingService(dbContext, _paymentServiceMock.Object, _geocodingServiceMock.Object, _emailServiceMock.Object, _loggerMock.Object, _configuration);
            var bookingRequest = new BookingRequestDto
            {
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                PaymentMethodId = "pm_card_visa",
                Address = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345"
            };

            // Act
            await bookingService.CreateBookingAsync(bookingRequest);

            // Assert
            var booking = await dbContext.Bookings.FirstAsync();
            Assert.Equal("123 Main St, Anytown, CA 12345", booking.EventLocation);
        }
    }
}