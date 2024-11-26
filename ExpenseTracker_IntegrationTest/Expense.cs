using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_IntegrationTest
{
    public class Expense
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Payer { get; set; }
        public string Participants { get; set; }
        public int GroupId { get; set; }
    }
}
