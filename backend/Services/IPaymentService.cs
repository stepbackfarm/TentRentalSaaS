using System.Threading.Tasks;
using Stripe;

namespace TentRentalSaaS.Api.Services
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency, string paymentMethodId, string returnUrl);
    }
}
