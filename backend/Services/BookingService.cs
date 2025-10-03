using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

namespace TentRentalSaaS.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApiDbContext _dbContext;
        private readonly IPaymentService _paymentService;

        public BookingService(ApiDbContext dbContext, IPaymentService paymentService)
        {
            _dbContext = dbContext;
            _paymentService = paymentService;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto bookingRequest)
        {
            var paymentIntent = await _paymentService.CreatePaymentIntentAsync(
                1000, // Example: $10.00
                "usd",
                bookingRequest.PaymentMethodId,
                "https://localhost:3000/confirmation" // This should be your frontend confirmation URL
            );

            // Find existing customer or create a new one
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == bookingRequest.CustomerEmail);

            if (customer == null)
            {
                // A simple way to split name into first and last
                var nameParts = bookingRequest.CustomerName.Split(' ', 2);
                var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = bookingRequest.CustomerEmail,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };
            }

            var booking = new Booking
            {
                CustomerName = bookingRequest.CustomerName,
                CustomerEmail = bookingRequest.CustomerEmail,
                EventDate = bookingRequest.EventDate,
                TentType = bookingRequest.TentType,
                NumberOfTents = bookingRequest.NumberOfTents,
                SpecialRequests = bookingRequest.SpecialRequests,
                BookingDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                StripePaymentIntentId = paymentIntent.Id,
                Status = "Confirmed", // Assuming direct confirmation for now
                Customer = customer // Associate the customer with the booking
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            // Map the booking entity to the response DTO
            var bookingResponse = new BookingResponseDto
            {
                Id = booking.Id,
                EventDate = booking.EventDate,
                Status = booking.Status,
                TentType = booking.TentType,
                NumberOfTents = booking.NumberOfTents,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                StripePaymentIntentId = booking.StripePaymentIntentId
            };

            return bookingResponse;
        }
    }
}