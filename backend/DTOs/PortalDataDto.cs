using System.Collections.Generic;

namespace TentRentalSaaS.Api.DTOs
{
    public class PortalDataDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public IEnumerable<BookingResponseDto> Bookings { get; set; } = Enumerable.Empty<BookingResponseDto>();
    }
}
