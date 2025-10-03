using System;

namespace TentRentalSaaS.Api.DTOs
{
    public class BookingResponseDto
    {
        public Guid Id { get; set; }
        public DateTime EventDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TentType { get; set; } = string.Empty;
        public int NumberOfTents { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string StripePaymentIntentId { get; set; } = string.Empty;
    }
}
