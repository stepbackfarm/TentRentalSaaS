using System.Threading.Tasks;

namespace TentRentalSaaS.Api.Services
{
    public interface IGeocodingService
    {
        Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address);
    }
}