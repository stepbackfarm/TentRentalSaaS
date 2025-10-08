using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TentRentalSaaS.Api.Services;

namespace TentRentalSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IEmailService _emailService;

        public JobsController(IBookingService bookingService, IEmailService emailService)
        {
            _bookingService = bookingService;
            _emailService = emailService;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunJobs()
        {
            await Send7DayReminders();
            await Send1DayReminders();
            await SendPostEventFollowUps();

            return Ok("Jobs completed.");
        }

        private async Task Send7DayReminders()
        {
            var upcomingDate = DateTime.UtcNow.AddDays(7).Date;
            var bookings = await _bookingService.GetBookingsForDateRangeAsync(upcomingDate, upcomingDate.AddDays(1).AddTicks(-1));

            foreach (var booking in bookings)
            {
                var subject = "Reminder: Your Tent Rental is in 7 Days!";
                var body = $"<h1>Hi {booking.Customer.FirstName},</h1>"
                         + "<p>This is a friendly reminder that your tent rental is scheduled for {booking.EventDate:D}.</p>"
                         + "<p>We look forward to helping with your event!</p>"
                         + "<p>If you have any questions, please don't hesitate to contact us at info@stepbackfarm.com.</p>";
                await _emailService.SendEmailAsync(booking.Customer.Email, subject, body);
            }
        }

        private async Task Send1DayReminders()
        {
            var upcomingDate = DateTime.UtcNow.AddDays(1).Date;
            var bookings = await _bookingService.GetBookingsForDateRangeAsync(upcomingDate, upcomingDate.AddDays(1).AddTicks(-1));

            foreach (var booking in bookings)
            {
                var subject = "Reminder: Your Tent Rental is Tomorrow!";
                var body = $"<h1>Hi {booking.Customer.FirstName},</h1>"
                         + "<p>This is a final reminder that your tent rental is scheduled for tomorrow, {booking.EventDate:D}.</p>"
                         + "<p>Our team will be there to set everything up. Please ensure the area is clear.</p>"
                         + "<p>If you have any last-minute questions, please contact us at info@stepbackfarm.com.</p>";
                await _emailService.SendEmailAsync(booking.Customer.Email, subject, body);
            }
        }

        private async Task SendPostEventFollowUps()
        {
            var pastDate = DateTime.UtcNow.AddDays(-2).Date;
            var bookings = await _bookingService.GetBookingsForDateRangeAsync(pastDate, pastDate.AddDays(1).AddTicks(-1));

            foreach (var booking in bookings)
            {
                var subject = "Thank You for Renting With Us!";
                var body = $"<h1>Hi {booking.Customer.FirstName},</h1>"
                         + "<p>Thank you for choosing us for your event on {booking.EventDate:D}. We hope everything was perfect!</p>"
                         + "<p>If you have a moment, we would greatly appreciate it if you could leave us a review. Your feedback helps us improve and lets others know about our service.</p>"
                         + "<p><a href=\"https://your-review-link.com\">Click here to leave a review</a></p>"
                         + "<p>As a token of our appreciation, please use the code <strong>THANKYOU10</strong> for 10% off your next rental.</p>"
                         + "<p>We hope to see you again soon!</p>";
                await _emailService.SendEmailAsync(booking.Customer.Email, subject, body);
            }
        }
    }
}
