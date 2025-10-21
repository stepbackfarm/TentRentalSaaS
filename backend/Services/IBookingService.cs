using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

namespace TentRentalSaaS.Api.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<DateTimeOffset>> GetAvailabilityAsync(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto bookingRequest);
        Task<decimal> CalculateDeliveryFeeAsync(AddressDto address);
        Task<IEnumerable<Booking>> GetBookingsForDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<QuoteResponseDto> GetQuoteAsync(QuoteRequestDto quoteRequest);
    }
}