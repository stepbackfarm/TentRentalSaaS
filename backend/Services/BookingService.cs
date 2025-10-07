using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Helpers;

using Microsoft.Extensions.Logging;

namespace TentRentalSaaS.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApiDbContext _dbContext;
        private readonly IPaymentService _paymentService;
        private readonly IGeocodingService _geocodingService;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingService> _logger;

        public BookingService(ApiDbContext dbContext, IPaymentService paymentService, IGeocodingService geocodingService, IEmailService emailService, ILogger<BookingService> logger)
        {
            _dbContext = dbContext;
            _paymentService = paymentService;
            _geocodingService = geocodingService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto bookingRequest)
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == bookingRequest.CustomerEmail);

            if (customer == null)
            {
                var nameParts = bookingRequest.CustomerName.Split(' ', 2);
                var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = bookingRequest.CustomerEmail,
                    Address = bookingRequest.Address,
                    City = bookingRequest.City,
                    State = bookingRequest.State,
                    ZipCode = bookingRequest.ZipCode,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };
                _dbContext.Customers.Add(customer);
            }

            decimal deliveryFee;
            try
            {
                var darlingtonCoords = await _geocodingService.GetCoordinatesAsync("Darlington, Indiana");
                var customerCoords = await _geocodingService.GetCoordinatesAsync($"{customer.Address}, {customer.City}, {customer.State} {customer.ZipCode}");
                var distance = DistanceCalculator.CalculateDistance(darlingtonCoords.Latitude, darlingtonCoords.Longitude, customerCoords.Latitude, customerCoords.Longitude);
                deliveryFee = (decimal)distance * 2.0m;
            }
            catch (Exception)
            {
                // Log the exception
                deliveryFee = 25.00m; // Default delivery fee
            }

            var rentalFee = 400;
            var securityDeposit = 100;
            var totalPrice = rentalFee + securityDeposit + deliveryFee;

            var paymentIntent = await _paymentService.CreatePaymentIntentAsync(
                (long)totalPrice * 100, // Convert to cents
                "usd",
                bookingRequest.PaymentMethodId,
                "https://localhost:3000/confirmation"
            );

            var booking = new Booking
            {
                EventDate = bookingRequest.EventDate,
                TentType = bookingRequest.TentType,
                NumberOfTents = bookingRequest.NumberOfTents,
                SpecialRequests = bookingRequest.SpecialRequests,
                BookingDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                StripePaymentIntentId = paymentIntent.Id,
                Status = BookingStatus.Confirmed,
                Customer = customer,
                RentalFee = rentalFee,
                SecurityDeposit = securityDeposit,
                DeliveryFee = deliveryFee,
                TotalPrice = totalPrice
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            var emailSubject = "Your Tent Rental Booking is Confirmed!";
            var emailBody = $"<h1>Booking Confirmed!</h1>"
                + "<p>Your tent is booked. We\'ve sent a confirmation to your email.</p>"
                + "<h2>What to Expect Next</h2>"
                + "<ul>"
                + "<li>You will receive a confirmation email shortly.</li>"
                + "<li>We will contact you 2-3 days before your event to confirm setup details.</li>"
                + "<li>Our team will arrive on the day of your event to set up the tent.</li>"
                + "<li>After your event, we will return to take down the tent.</li>"
                + "</ul>"
                + "<h2>Contact Us</h2>"
                + "<p>Have questions? Email us at contact@tentrentalsaas.com</p>";

            try
            {
                await _emailService.SendEmailAsync(customer.Email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email.");
                throw; // Re-throw the exception to stop the process
            }

            var bookingResponse = new BookingResponseDto
            {
                Id = booking.Id,
                EventDate = booking.EventDate,
                Status = booking.Status,
                TentType = booking.TentType,
                NumberOfTents = booking.NumberOfTents,
                CustomerName = customer.FirstName + " " + customer.LastName,
                CustomerEmail = customer.Email,
                StripePaymentIntentId = booking.StripePaymentIntentId
            };

            return bookingResponse;
        }

        public async Task<decimal> CalculateDeliveryFeeAsync(AddressDto address)
        {
            try
            {
                var darlingtonCoords = await _geocodingService.GetCoordinatesAsync("Darlington, Indiana");
                var customerCoords = await _geocodingService.GetCoordinatesAsync($"{address.Address}, {address.City}, {address.State} {address.ZipCode}");
                var distance = DistanceCalculator.CalculateDistance(darlingtonCoords.Latitude, darlingtonCoords.Longitude, customerCoords.Latitude, customerCoords.Longitude);
                var deliveryFee = (decimal)distance * 2.0m;
                return deliveryFee;
            }
            catch (Exception)
            {
                // Log the exception
                return 25.00m; // Default delivery fee
            }
        }
    }
}
