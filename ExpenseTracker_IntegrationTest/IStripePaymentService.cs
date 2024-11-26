using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_IntegrationTest
{
    public interface IStripePaymentService
    {

        bool ProcessPayment(decimal amount, string paymentDetails, string stripeToken);

    }
}
