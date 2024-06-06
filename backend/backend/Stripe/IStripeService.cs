using Stripe;

namespace backend.Stripe
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntent(long amount, string currency, Dictionary<string, string> metadata);
    }
}
