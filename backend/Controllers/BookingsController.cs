using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Models;

using TentRentalSaaS.Api.Services;

namespace TentRentalSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("availability")]
        public IActionResult GetAvailability([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Service call to get booked dates will be implemented here.
            // For now, return an empty list.
            return Ok(new List<DateTime>());
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] BookingRequestDto bookingRequest)
        {
            var createdBooking = await _bookingService.CreateBookingAsync(bookingRequest);
            return StatusCode(201, createdBooking);
        }
    }
}