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
            var dateDifference = quoteRequest.EndDate.Date - quoteRequest.StartDate.Date;
            _logger.LogInformation("DateDifference TimeSpan: {TimeSpan}", dateDifference);

            var rentalDays = dateDifference.Days;
            if (rentalDays < 2) {
                rentalDays = 2;
            }

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

            // Use decimal math throughout to avoid precision issues
            var rentalFee = 400m + (rentalDays > 2 ? (rentalDays - 2) * 100m : 0m);
            var securityDeposit = 100m;
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

            var customerAddress = bookingRequest.BillingAddress ?? bookingRequest.Address;
            var customerCity = bookingRequest.BillingCity ?? bookingRequest.City;
            var customerState = bookingRequest.BillingState ?? bookingRequest.State;
            var customerZipCode = bookingRequest.BillingZipCode ?? bookingRequest.ZipCode;

            if (customer != null)
            {
                // Customer exists, check if address needs updating
                if (customer.Address != customerAddress ||
                    customer.City != customerCity ||
                    customer.State != customerState ||
                    customer.ZipCode != customerZipCode)
                {
                    customer.Address = customerAddress;
                    customer.City = customerCity;
                    customer.State = customerState;
                    customer.ZipCode = customerZipCode;
                    customer.LastModifiedDate = DateTime.UtcNow;
                }
            }
            else
            {
                // Create new customer
                var nameParts = bookingRequest.CustomerName.Split(' ', 2);
                var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = bookingRequest.CustomerEmail,
                    Address = customerAddress,
                    City = customerCity,
                    State = customerState,
                    ZipCode = customerZipCode,
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
                Address = new AddressDto { Address = bookingRequest.Address, City = bookingRequest.City, State = bookingRequest.State, ZipCode = bookingRequest.ZipCode }
            };
            var quote = await GetQuoteAsync(quoteRequest);

            // Safe decimal to cents conversion for Stripe
            var amountInCents = Convert.ToInt64(Math.Round(quote.TotalPrice * 100m, 0, MidpointRounding.AwayFromZero));

            var paymentIntent = await _paymentService.CreatePaymentIntentAsync(
                amountInCents,
                "usd",
                bookingRequest.PaymentMethodId,
                "https://localhost:3000/confirmation"
            );

            var booking = new Booking
            {
                EventAddress = bookingRequest.Address,
                EventCity = bookingRequest.City,
                EventState = bookingRequest.State,
                EventZipCode = bookingRequest.ZipCode,
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
                    + $"<p><strong>Event Date:</strong> {booking.EventDate:D} - {booking.EventEndDate:D}</p>"
                    + $"<p><strong>Event Location:</strong> {booking.EventAddress}, {booking.EventCity}, {booking.EventState} {booking.EventZipCode}</p>"
                    + $"<p><strong>Tent Type:</strong> {booking.TentType}</p>"
                    + $"<p><strong>Number of Tents:</strong> {booking.NumberOfTents}</p>"
                    + $"<h2>Pricing Breakdown</h2>"
                    + $"<ul>"
                    + $"<li><strong>Rental Fee ({quote.RentalDays} days):</strong> {booking.RentalFee:C}</li>"
                    + $"<li><strong>Delivery Fee:</strong> {booking.DeliveryFee:C}</li>"
                    + $"<li><strong>Security Deposit:</strong> {booking.SecurityDeposit:C}</li>"
                    + $"</ul>"
                    + $"<p><strong>Total Price:</strong> {booking.TotalPrice:C}</p>"
                    + $"<p><strong>Stripe Charge:</strong> {amountInCents / 100m:C} ({amountInCents} cents)</p>"
                    + $"<p><strong>Stripe Payment Intent ID:</strong> {booking.StripePaymentIntentId}</p>";
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
                
                var fullCustomerAddress = $"{address.Address}, {address.City}, {address.State} {address.ZipCode}";
                var customerCoords = await _geocodingService.GetCoordinatesAsync(fullCustomerAddress);
                
                // DistanceCalculator returns miles (verified by EarthRadiusMiles = 3959)
                var distanceInMiles = DistanceCalculator.CalculateDistance(darlingtonCoords.Latitude, darlingtonCoords.Longitude, customerCoords.Latitude, customerCoords.Longitude);
                
                // Business rule: $2.00 per mile delivery fee with minimum charge logic
                const decimal ratePerMile = 2.00m;
                const decimal baseFee = 0.00m; // No base fee currently
                const decimal minFee = 5.00m; // Minimum delivery fee
                
                var calculatedFee = baseFee + ((decimal)distanceInMiles * ratePerMile);
                var deliveryFee = Math.Max(calculatedFee, minFee);
                deliveryFee = Math.Round(deliveryFee, 2, MidpointRounding.AwayFromZero);
                
                // Sanity check - if delivery fee is unreasonably high, log error and use default
                if (deliveryFee > 500m)
                {
                    _logger.LogError("DELIVERY FEE ANOMALY DETECTED: fee={Fee:C} is too high for distance={Distance} miles. Using default fee.", deliveryFee, distanceInMiles);
                    return 25.00m;
                }
                
                return deliveryFee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate delivery fee for address: {Address}", $"{address.Address}, {address.City}, {address.State} {address.ZipCode}");
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
