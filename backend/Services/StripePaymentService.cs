using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace TentRentalSaaS.Api.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;

        public StripePaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
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
