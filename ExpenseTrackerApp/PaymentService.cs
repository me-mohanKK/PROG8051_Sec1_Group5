using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Stripe;


namespace ExpenseTrackerApp
{
    public class PaymentService
    {


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
