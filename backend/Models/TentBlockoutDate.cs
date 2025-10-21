using System;
using System.ComponentModel.DataAnnotations;

namespace TentRentalSaaS.Api.Models
{
    public class TentBlockoutDate
    {
        public int Id { get; set; }

        [Required]
        public string TentType { get; set; } // e.g., "small", "medium", "large"

        [Required]
        public DateTimeOffset Date { get; set; }

        // Optional: Reference to a specific tent instance if needed,
        // but for now, we'll track availability per tent type per day.
        // public int? TentId { get; set; }
        // public Tent Tent { get; set; }
    }
}