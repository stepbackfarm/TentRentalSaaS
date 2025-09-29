using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

namespace TentRentalSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        [HttpGet("availability")]
        public IActionResult GetAvailability([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Service call to get booked dates will be implemented here.
            // For now, return an empty list.
            return Ok(new List<DateTime>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto bookingRequest)
        {
            // Placeholder for booking and payment processing service
            var createdBooking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerName = bookingRequest.CustomerName,
                CustomerEmail = bookingRequest.CustomerEmail,
                EventDate = bookingRequest.EventDate,
                TentType = bookingRequest.TentType,
                NumberOfTents = bookingRequest.NumberOfTents,
                SpecialRequests = bookingRequest.SpecialRequests,
                Status = "Confirmed", // Placeholder status
                TotalPrice = bookingRequest.NumberOfTents * 100 // Placeholder price
            };

            return StatusCode(201, createdBooking);
        }
    }
}