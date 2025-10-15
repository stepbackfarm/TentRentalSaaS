using System;
using System.ComponentModel.DataAnnotations;

namespace TentRentalSaaS.Api.Models
{
    public class LoginToken
    {
        [Key]
        public Guid TokenId { get; set; }
        public Guid CustomerId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedDate { get; set; }

        // Navigation property
        public Customer Customer { get; set; } = null!;
    }
}