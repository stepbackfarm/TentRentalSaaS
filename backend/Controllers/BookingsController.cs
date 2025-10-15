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
        public async Task<IActionResult> GetAvailability([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var unavailableDates = await _bookingService.GetAvailabilityAsync(startDate, endDate);
            return Ok(unavailableDates);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] BookingRequestDto bookingRequest)
        {
            var createdBooking = await _bookingService.CreateBookingAsync(bookingRequest);
            return StatusCode(201, createdBooking);
        }

        [HttpPost("delivery-fee")]
        public async Task<ActionResult<decimal>> GetDeliveryFee([FromBody] AddressDto address)
        {
            var deliveryFee = await _bookingService.CalculateDeliveryFeeAsync(address);
            return Ok(deliveryFee);
        }
    }
}