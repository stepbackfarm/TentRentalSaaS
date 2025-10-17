using System;
using Xunit;
using TentRentalSaaS.Api.Helpers;

namespace TentRentalSaaS.Api.tests
{
    public class DeliveryFeeCalculationTest
    {
        [Fact]
        public void DistanceCalculator_ReturnsExpectedMiles()
        {
            // Test coordinates that should be approximately 7.5 miles apart
            // Darlington, IN (approximate)
            double darlingtonLat = 40.101;
            double darlingtonLon = -87.105;
            
            // A point approximately 7.5 miles away
            double testLat = 40.050;
            double testLon = -87.000;
            
            var distance = DistanceCalculator.CalculateDistance(darlingtonLat, darlingtonLon, testLat, testLon);
            
            // Should return miles, not meters or feet
            Assert.True(distance > 0.1 && distance < 100, $"Expected reasonable distance in miles, got {distance}");
        }
        
        [Fact]  
        public void DeliveryFee_CalculationLogic_ReturnsExpectedValues()
        {
            // Test the business logic for delivery fee calculation
            double distanceInMiles = 0.74; // This should result in $1.48 at $2/mile
            
            const decimal ratePerMile = 2.00m;
            const decimal baseFee = 0.00m;
            const decimal minFee = 5.00m;
            
            var calculatedFee = baseFee + ((decimal)distanceInMiles * ratePerMile);
            var deliveryFee = Math.Max(calculatedFee, minFee);
            deliveryFee = Math.Round(deliveryFee, 2, MidpointRounding.AwayFromZero);
            
            // For 0.74 miles, we expect $1.48, but minimum fee is $5.00
            Assert.Equal(5.00m, deliveryFee);
            
            // Test with a longer distance that exceeds minimum
            distanceInMiles = 7.5; // Should be $15.00 at $2/mile
            calculatedFee = baseFee + ((decimal)distanceInMiles * ratePerMile);
            deliveryFee = Math.Max(calculatedFee, minFee);
            deliveryFee = Math.Round(deliveryFee, 2, MidpointRounding.AwayFromZero);
            
            Assert.Equal(15.00m, deliveryFee);
        }
        
        [Fact]
        public void StripeConversion_HandlesDecimalsCorrectly()
        {
            // Test cases for Stripe cents conversion
            decimal totalPrice = 601.00m;
            var amountInCents = Convert.ToInt64(Math.Round(totalPrice * 100m, 0, MidpointRounding.AwayFromZero));
            Assert.Equal(60100, amountInCents);
            
            // Test edge case with fractional cents
            totalPrice = 601.485m; // Should round to 601.49
            amountInCents = Convert.ToInt64(Math.Round(totalPrice * 100m, 0, MidpointRounding.AwayFromZero));
            Assert.Equal(60149, amountInCents);
            
            // Test problematic case - if we accidentally stored 12096.825 as totalPrice
            totalPrice = 12096.825m;
            amountInCents = Convert.ToInt64(Math.Round(totalPrice * 100m, 0, MidpointRounding.AwayFromZero));
            Assert.Equal(1209683, amountInCents); // This would be $12,096.83 charged to Stripe!
        }
    }
}