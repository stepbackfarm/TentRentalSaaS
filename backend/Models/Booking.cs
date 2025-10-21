using System;
using System.ComponentModel.DataAnnotations;

namespace TentRentalSaaS.Api.Models
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public DateTimeOffset EventEndDate { get; set; }
        public DateTimeOffset BookingDate { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string EventAddress { get; set; } = string.Empty;
        public string EventCity { get; set; } = string.Empty;
        public string EventState { get; set; } = string.Empty;
        public string EventZipCode { get; set; } = string.Empty;
        public decimal RentalFee { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
        public string TentType { get; set; } = string.Empty;
        public string? SpecialRequests { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation property
        public Customer Customer { get; set; } = null!;
    }
}