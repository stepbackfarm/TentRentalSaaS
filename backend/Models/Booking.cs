using System;
using System.ComponentModel.DataAnnotations;

namespace TentRentalSaaS.Api.Models
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }
        public Guid Id => BookingId; // Alias for compatibility
        public Guid CustomerId { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }
        public decimal RentalFee { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Status { get; set; } // e.g., "Pending", "Confirmed", "Cancelled"
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        
        // Additional properties for API compatibility
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string TentType { get; set; } = string.Empty;
        public int NumberOfTents { get; set; }
        public string SpecialRequests { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }

        // Navigation property
        public Customer Customer { get; set; }
    }
}