using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

namespace TentRentalSaaS.Api.Services
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(BookingRequestDto bookingRequest);
    }
}