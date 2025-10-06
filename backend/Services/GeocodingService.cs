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
            _geocoder = new GoogleGeocoder(configuration["GoogleMaps:ApiKey"]);
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