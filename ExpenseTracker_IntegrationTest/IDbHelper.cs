using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExpenseTrackerApp.ExpenseEntry;

namespace ExpenseTracker_IntegrationTest
{
    public interface IDbHelper
    {


        int CreateGroup(string groupName, List<string> members);
        bool AddExpenseToGroup(int groupId, decimal amount, string description, DateTime date, string payer, string participants, List<string> participantList);
        dynamic GetExpenseFromDatabase(int groupId, decimal amount, string description);
        void CleanupTestData(int groupId);

        int AddExpense(Expense expense);
        void UpdateExpenseStatus(int expenseId, string status);
        string GetExpenseStatus(int expenseId);

        void DeleteGroup(int groupId);
    }
}
