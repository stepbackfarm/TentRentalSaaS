using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

namespace TentRentalSaaS.Api.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto bookingRequest);
        Task<decimal> CalculateDeliveryFeeAsync(AddressDto address);
        Task<IEnumerable<Booking>> GetBookingsForDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}