using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    [HttpPost("create-checkout-session")]
    public ActionResult CreateCheckoutSession([FromBody] PaymentRequest request)
    {
        StripeConfiguration.ApiKey = "sk_test_51QGmMKKGIcfOeSNZJp3rrfZkhVtI8Bnh6GZw8BboWjgkrdhgeRWEfHKSAMe8q8FUw4u8ShcghjYYJ3AYWlZFO56b000TEQS02j";

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Settlement Payment",
                    },
                    UnitAmount = request.AmountInCents,
                },
                Quantity = 1,
            },
        },
            Mode = "payment",
            SuccessUrl = "http://localhost:5000/api/payment/success", // Updated URL
            CancelUrl = "http://localhost:5000/api/payment/cancel",   // Updated URL
        };

        try
        {
            var service = new SessionService();
            Session session = service.Create(options);

            // Return the session URL for redirection
            return Ok(new { sessionUrl = session.Url }); // Updated to return session URL instead of sessionId
        }
        catch (StripeException ex)
        {
            // Handle Stripe errors gracefully
            return BadRequest(new { ErrorMessage = ex.StripeError.Message });
        }
    }
    [HttpGet("success")]
    public IActionResult Success()
    {
        // Render a simple success page
        return Content("<h1>Payment Successful</h1><p>Thank you for your payment!</p>", "text/html");
    }

    [HttpGet("cancel")]
    public IActionResult Cancel()
    {
        // Render a simple cancel page
        return Content("<h1>Payment Cancelled</h1><p>Your payment has been cancelled.</p>", "text/html");
    }
}



public class PaymentRequest
{
    public long AmountInCents { get; set; }
}
