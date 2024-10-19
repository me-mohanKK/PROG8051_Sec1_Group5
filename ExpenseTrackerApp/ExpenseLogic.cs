using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerApp
{
    public class ExpenseLogic
    {
        public string FormatExpense(decimal amount, string description, DateTime date, string payer, int div)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");

            // Calculate total participants including the payer
            int totalParticipants = Math.Max(1, div + 1); // Include the payer in the count
            decimal splitAmount = amount / totalParticipants; // Calculate each participant's share

            return $"{amount:C} - {description} on {date.ToShortDateString()} by {payer} split to {totalParticipants} person(s) ${splitAmount:F2} each (including payer)";
        }
    }
}
