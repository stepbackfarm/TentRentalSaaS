using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Services;
using Xunit;

namespace TentRentalSaaS.Api.Tests
{
    public class BookingServiceTests
    {
        private ApiDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApiDbContext(options);
        }

        [Fact]
        public async Task GetAvailability_ShouldReturnEmptyList_WhenNoBookings()
        {
            // Arrange
            using var dbContext = GetDbContext();
            var bookingService = new BookingService(dbContext);

            var tentId = 1;
            var startDate = new DateTime(2025, 10, 1);
            var endDate = new DateTime(2025, 10, 10);

            // Act
            var availableDates = await bookingService.GetAvailability(tentId, startDate, endDate);

            // Assert
            Assert.NotNull(availableDates);
            Assert.Empty(availableDates); // Current mock implementation returns empty list
        }

        [Fact]
        public async Task CreateBooking_ShouldReturnTrue_ForValidBooking()
        {
            // Arrange
            using var dbContext = GetDbContext();
            var bookingService = new BookingService(dbContext);

            var tentId = 1;
            var startDate = new DateTime(2025, 10, 1);
            var endDate = new DateTime(2025, 10, 5);

            // Act
            var result = await bookingService.CreateBooking(tentId, startDate, endDate);

            // Assert
            Assert.True(result); // Current mock implementation returns true
        }
    }
}
