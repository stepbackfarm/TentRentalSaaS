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
        public DateTime EventDate { get; set; }

        [Required]
        [MinLength(1)]
        public string TentType { get; set; } = string.Empty;

        [Range(1, 100)]
        public int NumberOfTents { get; set; }

        public string? SpecialRequests { get; set; }
    }
}