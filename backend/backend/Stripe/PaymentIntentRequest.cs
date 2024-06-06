namespace backend.Stripe
{
    public class PaymentIntentRequest
    {
        public long Amount { get; set; }
        public string Currency { get; set; }
        public Guid OrderId { get; set; }
    }
}
