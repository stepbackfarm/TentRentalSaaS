using System;
using System.ComponentModel.DataAnnotations;
using TentRentalSaaS.Api.Helpers;

namespace TentRentalSaaS.Api.DTOs
{
    public class BookingRequestDto
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s'.-]+$", ErrorMessage = "Name contains invalid characters")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(254, ErrorMessage = "Email address too long")] // RFC 5321 maximum
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s'.-]+$", ErrorMessage = "City contains invalid characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "State must be 2 characters")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "State must be 2 uppercase letters (e.g. IN, CA)")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "ZIP code is required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid ZIP code format (e.g. 12345 or 12345-6789)")]
        public string ZipCode { get; set; } = string.Empty;

        // Billing address fields (optional)
        [StringLength(200, ErrorMessage = "Billing address cannot exceed 200 characters")]
        public string? BillingAddress { get; set; }

        [StringLength(100, ErrorMessage = "Billing city cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s'.-]+$", ErrorMessage = "Billing city contains invalid characters")]
        public string? BillingCity { get; set; }

        [StringLength(2, MinimumLength = 2, ErrorMessage = "Billing state must be 2 characters")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Billing state must be 2 uppercase letters")]
        public string? BillingState { get; set; }

        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid billing ZIP code format")]
        public string? BillingZipCode { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [FutureDate] // Custom validation: must be future date, not more than 2 years ahead
        public DateTimeOffset EventDate { get; set; }

        [Required(ErrorMessage = "Event end date is required")]
        [FutureDate]
        public DateTimeOffset EventEndDate { get; set; }

        [Required(ErrorMessage = "Tent type is required")]
        [StringLength(50, ErrorMessage = "Tent type name too long")]
        public string TentType { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Special requests cannot exceed 1000 characters")]
        public string? SpecialRequests { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(100, ErrorMessage = "Payment method ID too long")]
        public string PaymentMethodId { get; set; } = string.Empty;
    }
}
