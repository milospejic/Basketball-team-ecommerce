using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Stripe;
using backend.Data.Repository; // Assuming you have a repository for orders

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly string _webhookSecret;
        private readonly IOrderRepository _orderRepository;

        public StripeWebhookController(IConfiguration configuration, ILogger<StripeWebhookController> logger, IOrderRepository orderRepository)
        {
            _webhookSecret = configuration["Stripe:WebhookSecret"];
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webhookSecret);
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (Guid.TryParse(paymentIntent.Metadata["orderId"], out Guid orderIdGuid))
                        {
                            // If orderId is successfully parsed, pay the order
                            await _orderRepository.PayOrder(orderIdGuid);
                            _logger.LogInformation($"PaymentIntent succeeded: {paymentIntent.Id}");
                        }
                        else
                        {
                            // Log an error if orderId cannot be parsed
                            _logger.LogError($"Invalid orderId format: {paymentIntent.Metadata["orderId"]}");
                        }
                        break;
                    case Events.PaymentIntentCreated:
                        _logger.LogInformation($"PaymentIntent was created: {stripeEvent.Id}");
                        break;
                    case Events.ChargeSucceeded:
                        var charge = stripeEvent.Data.Object as Charge;
                        _logger.LogInformation($"Charge succeeded: {charge.Id}");
                        break;
                    default:
                        _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                // Log any Stripe-related exceptions
                _logger.LogError(e, "Stripe webhook error");
                return BadRequest();
            }
            catch (Exception ex)
            {
                // Log any other unhandled exceptions
                _logger.LogError(ex, "Unhandled error in webhook handler");
                return StatusCode(500);
            }
        }

    }
}
