using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TentRentalSaaS.Api.Services;
using Xunit;

namespace TentRentalSaaS.Api.Tests
{
    public class GoogleMapsGeocodingServiceTests
    {
        private readonly IConfiguration _configuration;

        public GoogleMapsGeocodingServiceTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }

        [Fact]
        public async Task GetCoordinatesAsync_ShouldReturnCorrectCoordinates_ForValidAddress()
        {
            var apiKey = _configuration["GOOGLE_MAPS_API_KEY"];
            if (string.IsNullOrEmpty(apiKey))
            {
                Assert.True(true, "Skipping integration test because GOOGLE_MAPS_API_KEY is not set.");
                return;
            }

            var geocodingService = new GoogleMapsGeocodingService(_configuration);
            var (latitude, longitude) = await geocodingService.GetCoordinatesAsync("1600 Amphitheatre Parkway, Mountain View, CA");

            Assert.InRange(latitude, 37.4, 37.5);
            Assert.InRange(longitude, -122.1, -122.0);
        }
    }
}