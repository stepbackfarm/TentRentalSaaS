using System.Threading.Tasks;
using Stripe;
using System;

namespace TentRentalSaaS.Api.Services
{
    public class StripePaymentService : IPaymentService
    {
        public StripePaymentService()
        {
            var apiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "Stripe secret key not found. Set the STRIPE_SECRET_KEY environment variable.");
            }
            StripeConfiguration.ApiKey = apiKey;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency, string paymentMethodId, string returnUrl)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                PaymentMethod = paymentMethodId,
                ConfirmationMethod = "manual",
                Confirm = true,
                ReturnUrl = returnUrl,
            });
            return paymentIntent;
        }
    }
}
