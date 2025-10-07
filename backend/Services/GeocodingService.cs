using System.Linq;
using System.Threading.Tasks;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;

namespace TentRentalSaaS.Api.Services
{
    public class GoogleMapsGeocodingService : IGeocodingService
    {
        private readonly GoogleGeocoder _geocoder;

        public GoogleMapsGeocodingService(IConfiguration configuration)
        {
            // Try environment variable first, then configuration
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY") 
                      ?? configuration["GoogleMaps:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "Google Maps API key not found. Set GOOGLE_MAPS_API_KEY environment variable or GoogleMaps:ApiKey in configuration.");
            }
            
            _geocoder = new GoogleGeocoder(apiKey);
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
        {
            var addresses = await _geocoder.GeocodeAsync(address);
            var first = addresses.FirstOrDefault();
            if (first != null)
            {
                return (first.Coordinates.Latitude, first.Coordinates.Longitude);
            }
            return (0, 0);
        }
    }
}