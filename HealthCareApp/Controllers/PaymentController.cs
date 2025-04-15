using Stripe;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class PaymentController : Controller
{
    private readonly string _secretKey = "sk_test_51RDJNXHXAaOwfwNpnax4xnUeUJzWNO7HoVXCR72Mf9yCyYQcUxs2wrsaCXtb122Qe7Z6EOSWSoKI14NlqfUyGh1U002a6lJJlW"; // Replace with your secret key

    public PaymentController()
    {
        StripeConfiguration.ApiKey = _secretKey;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCharge([FromBody] PaymentRequest request)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = "usd",
                PaymentMethod = request.PaymentMethodId,
                Confirm = true,
                Description = "Payment from customer",
                // Use automatic payment methods (remove payment_method_types)
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent.Status switch
            {
                "requires_action" => Json(new
                {
                    requires_action = true,
                    client_secret = paymentIntent.ClientSecret
                }),
                "succeeded" => Json(new
                {
                    success = true,
                    payment_intent_id = paymentIntent.Id
                }),
                _ => BadRequest(new
                {
                    error = paymentIntent.LastPaymentError?.Message ?? "Payment failed"
                })
            };
        }
        catch (StripeException ex)
        {
            return StatusCode(500, new
            {
                error = ex.StripeError?.Message ?? ex.Message
            });
        }
    }

    public class PaymentRequest
    {
        [Required]
        public string PaymentMethodId { get; set; }

        [Required]
        [Range(50, 99999999)] // Minimum $0.50
        public long Amount { get; set; }
    }
}
