using System;
using System.ComponentModel.DataAnnotations;

namespace TentRentalSaaS.Api.DTOs
{
    public class BookingRequestDto
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MinLength(1)]
        public string TentType { get; set; } = string.Empty;

        [Range(1, 100)]
        public int NumberOfTents { get; set; }

        public string? SpecialRequests { get; set; }

        [Required]
        public string PaymentMethodId { get; set; } = string.Empty;
    }
}