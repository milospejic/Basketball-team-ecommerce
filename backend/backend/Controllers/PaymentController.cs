using backend.Stripe;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentsController : ControllerBase
    {
        private readonly IStripeService _stripeService;

        public PaymentsController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentIntentRequest request)
        {
            var metadata = new Dictionary<string, string>
            {
                { "orderId", request.OrderId.ToString() } // Convert OrderId to string
                // Add other metadata as needed
            };

            var paymentIntent = await _stripeService.CreatePaymentIntent(request.Amount, request.Currency, metadata);
            return Ok(new { ClientSecret = paymentIntent.ClientSecret });
        }
    }
}
