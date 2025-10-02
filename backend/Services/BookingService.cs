using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

namespace TentRentalSaaS.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApiDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public BookingService(ApiDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Booking> CreateBookingAsync(BookingRequestDto bookingRequest)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = 1000, // Example: $10.00
                Currency = "usd",
                PaymentMethod = bookingRequest.PaymentMethodId,
                ConfirmationMethod = "manual",
                Confirm = true,
                ReturnUrl = "https://localhost:3000/confirmation", // This should be your frontend confirmation URL
            });

            // For now, let's just create a dummy booking model and return it.
            // In a real application, you would save this to your database.
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerName = bookingRequest.CustomerName,
                CustomerEmail = bookingRequest.CustomerEmail,
                EventDate = bookingRequest.EventDate,
                TentType = bookingRequest.TentType,
                NumberOfTents = bookingRequest.NumberOfTents,
                SpecialRequests = bookingRequest.SpecialRequests,
                BookingDate = DateTime.UtcNow,
                StripePaymentIntentId = paymentIntent.Id,
                Status = "Confirmed" // Assuming direct confirmation for now
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            return booking;
        }
    }
}
