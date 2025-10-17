using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Helpers;

using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public BookingService(ApiDbContext dbContext, IPaymentService paymentService, IGeocodingService geocodingService, IEmailService emailService, ILogger<BookingService> logger, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _paymentService = paymentService;
            _geocodingService = geocodingService;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<DateTime>> GetAvailabilityAsync(DateTime startDate, DateTime endDate)
        {
            var overlappingBookings = await _dbContext.Bookings
                .Where(b => b.Status == BookingStatus.Confirmed && b.EventDate < endDate && b.EventEndDate > startDate)
                .ToListAsync();

            var unavailableDates = new List<DateTime>();
            foreach (var booking in overlappingBookings)
            {
                for (var date = booking.EventDate.Date; date <= booking.EventEndDate.Date; date = date.AddDays(1))
                {
                    unavailableDates.Add(date);
                }
            }

            return unavailableDates.Distinct();
        }

        public async Task<QuoteResponseDto> GetQuoteAsync(QuoteRequestDto quoteRequest)
        {
            _logger.LogInformation("GetQuoteAsync called with StartDate: {StartDate}, EndDate: {EndDate}", quoteRequest.StartDate, quoteRequest.EndDate);

            decimal deliveryFee;
            try
            {
                deliveryFee = await CalculateDeliveryFeeAsync(quoteRequest.Address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate delivery fee for quote.");
                deliveryFee = 25.00m; // Default delivery fee
            }

            var rentalDays = (quoteRequest.EndDate.Date - quoteRequest.StartDate.Date).Days;
            _logger.LogInformation("GetQuoteAsync calculated rentalDays: {RentalDays}", rentalDays);
            if (rentalDays < 2) {
                rentalDays = 2;
            }

            var rentalFee = 400 + (rentalDays > 2 ? (rentalDays - 2) * 100 : 0);
            var securityDeposit = 100;
            var totalPrice = rentalFee + securityDeposit + deliveryFee;

            return new QuoteResponseDto
            {
                RentalFee = rentalFee,
                DeliveryFee = deliveryFee,
                SecurityDeposit = securityDeposit,
                TotalPrice = totalPrice,
                RentalDays = rentalDays
            };
        }

        public async Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto bookingRequest)
        {
            _logger.LogInformation("CreateBookingAsync called with EventDate: {EventDate}, EventEndDate: {EventEndDate}", bookingRequest.EventDate, bookingRequest.EventEndDate);

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

            // Create a quote request from the booking request to get the official price
            var quoteRequest = new QuoteRequestDto
            {
                StartDate = bookingRequest.EventDate,
                EndDate = bookingRequest.EventEndDate,
                Address = new AddressDto { Address = customer.Address, City = customer.City, State = customer.State, ZipCode = customer.ZipCode }
            };
            var quote = await GetQuoteAsync(quoteRequest);

            var paymentIntent = await _paymentService.CreatePaymentIntentAsync(
                (long)quote.TotalPrice * 100, // Convert to cents
                "usd",
                bookingRequest.PaymentMethodId,
                "https://localhost:3000/confirmation"
            );

            var booking = new Booking
            {
                EventDate = bookingRequest.EventDate,
                EventEndDate = bookingRequest.EventEndDate,
                TentType = bookingRequest.TentType,
                NumberOfTents = bookingRequest.NumberOfTents,
                SpecialRequests = bookingRequest.SpecialRequests,
                BookingDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                StripePaymentIntentId = paymentIntent.Id,
                Status = BookingStatus.Confirmed,
                Customer = customer,
                RentalFee = quote.RentalFee,
                SecurityDeposit = quote.SecurityDeposit,
                DeliveryFee = quote.DeliveryFee,
                TotalPrice = quote.TotalPrice
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
                + "<p>Have questions? Email us at info@stepbackfarm.com</p>";

            try
            {
                await _emailService.SendEmailAsync(customer.Email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email.");
                throw; // Re-throw the exception to stop the process
            }

            // Send a notification to the admin
            try
            {
                var adminEmail = _configuration["ADMIN_EMAIL"] ?? "david@stepbackfarm.com";
                var adminSubject = $"New Tent Rental Booking: {customer.FirstName} {customer.LastName}";
                var adminBody = $"<h1>New Booking Received</h1>"
                    + $"<p><strong>Customer:</strong> {customer.FirstName} {customer.LastName}</p>"
                    + $"<p><strong>Email:</strong> {customer.Email}</p>"
                    + $"<p><strong>Event Date:</strong> {booking.EventDate:D}</p>"
                    + $"<p><strong>Total Price:</strong> {booking.TotalPrice:C}</p>";
                await _emailService.SendEmailAsync(adminEmail, adminSubject, adminBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin notification email.");
                // Do not re-throw; failing to send the admin email should not fail the customer's booking.
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

        public async Task<IEnumerable<Booking>> GetBookingsForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Bookings
                .Include(b => b.Customer)
                .Where(b => startDate <= b.EventEndDate && endDate >= b.EventDate)
                .ToListAsync();
        }
    }
}
