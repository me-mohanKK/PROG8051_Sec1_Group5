using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpenseTrackerApp.Payments
{
    /// <summary>
    /// Interaction logic for PaymentPage.xaml
    /// </summary>
    public partial class PaymentPage : UserControl
    {
        public PaymentPage()
        {
            InitializeComponent();
        }

       /* private async void SettleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (decimal.TryParse(SettlementAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) && amount > 0)
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long)(amount * 100), // Amount in cents
                        Currency = "usd", // Specify your currency
                        PaymentMethodData = new PaymentMethodDataOptions
                        {
                            Card = new PaymentMethodCardOptions
                            {
                                Number = CardNumberTextBox.Text,
                                Cvc = CvvTextBox.Text,
                                ExpMonth = 12, // You need to get this dynamically or add a field for it
                                ExpYear = 2025, // You need to get this dynamically or add a field for it
                            },
                            BillingDetails = new BillingDetailsOptions
                            {
                                Name = CardholderNameTextBox.Text,
                            },
                        },
                    };

                    var service = new PaymentIntentService();
                    var paymentIntent = await service.CreateAsync(options);

                    if (paymentIntent.Status == "succeeded")
                    {
                        MessageBox.Show("Payment successful!");
                    }
                    else
                    {
                        MessageBox.Show("Payment not successful.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid amount.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }*/
    }

}
