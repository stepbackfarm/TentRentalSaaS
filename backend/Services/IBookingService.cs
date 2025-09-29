namespace TentRentalSaaS.Api.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<DateTime>> GetAvailability(int tentId, DateTime startDate, DateTime endDate);
        Task<bool> CreateBooking(int tentId, DateTime startDate, DateTime endDate);
    }
}