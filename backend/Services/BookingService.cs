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

            return booking;
        }
    }
}
