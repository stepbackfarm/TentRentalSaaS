using System.ComponentModel.DataAnnotations;

namespace TentRentalSaaS.Api.Helpers
{
    /// <summary>
    /// Validates that a date is in the future and not too far in advance
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTimeOffset dateValue)
            {
                if (dateValue <= DateTimeOffset.UtcNow)
                {
                    return new ValidationResult("Event date must be in the future");
                }
                
                // Don't allow bookings more than 2 years in advance
                if (dateValue > DateTimeOffset.UtcNow.AddYears(2))
                {
                    return new ValidationResult("Event date cannot be more than 2 years in advance");
                }
            }
            return ValidationResult.Success!;
        }
    }
}
