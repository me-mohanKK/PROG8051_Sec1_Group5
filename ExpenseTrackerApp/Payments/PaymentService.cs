using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Stripe;
using Newtonsoft.Json;
using System.Net.Http;
using Stripe.Checkout;


namespace ExpenseTrackerApp.Payments
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService()
        {
            _httpClient = new HttpClient();
            // Set the base address of the API endpoint
            _httpClient.BaseAddress = new Uri("https://api.stripe.com/v1/payment_intents"); // Replace with your server's URL
            _httpClient.DefaultRequestHeaders.Add("Authorization", "sk_test_51QGmMKKGIcfOeSNZJp3rrfZkhVtI8Bnh6GZw8BboWjgkrdhgeRWEfHKSAMe8q8FUw4u8ShcghjYYJ3AYWlZFO56b000TEQS02j"); // Set your Stripe secret key
        }

        public class PaymentResult
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class PaymentData
        {
            public string CardNumber { get; set; }
            public string ExpiryDate { get; set; }
            public string CVC { get; set; }
        }

        public async Task<string> CreateCheckoutSession(long amount)
        {
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
                        Name = "Expense Payment", // Customize as needed
                    },
                    UnitAmount = amount,
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = "https://localhost/ExpensePaymentStatus/success.html", // Update with your success URL
                CancelUrl = "https://localhost/ExpensePaymentStatus/cancel.html",   // Update with your cancel URL

                // Change URLs to invoke methods instead of HTTP endpoints
                //SuccessUrl = "success",
                // CancelUrl = "cancel",
            };

            var service = new SessionService();
            try
            {
                Session session = await service.CreateAsync(options);
                return session.Url; // This is the URL to redirect to
            }
            catch (StripeException ex)
            {
                MessageBox.Show($"Stripe error: {ex.StripeError.Message}");
                return null;
            }
        }
        public async Task<PaymentIntent> CreatePaymentIntent(long amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd", // or your preferred currency
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            try
            {
                PaymentIntent paymentIntent = await service.CreateAsync(options);
                return paymentIntent;
            }
            catch (StripeException ex)
            {
                MessageBox.Show($"Stripe error: {ex.StripeError.Message}");
                return null;
            }
        }

        public async Task<PaymentResult> ProcessPayment(PaymentData paymentData)
        {
            var json = JsonConvert.SerializeObject(paymentData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("payment/intent", content); // Replace with your API route
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaymentResult>(resultContent);
            }

            return new PaymentResult { IsSuccess = false, ErrorMessage = "Payment failed." };
        }

        public PaymentIntent ConfirmPayment(string paymentIntentId, string paymentMethodId)
        {
            try
            {
                var options = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethodId,
                };

                var service = new PaymentIntentService();
                PaymentIntent paymentIntent = service.Confirm(paymentIntentId, options);
                return paymentIntent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming payment intent: {ex.Message}");
                return null;
            }
        }
    }
}
