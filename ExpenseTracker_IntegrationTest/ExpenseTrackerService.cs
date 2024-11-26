using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExpenseTrackerApp.ExpenseEntry;

namespace ExpenseTracker_IntegrationTest
{
    public class ExpenseTrackerService : IUserAuthenticationService
    {
        private readonly IDbHelper _dbHelper;
        private readonly IStripePaymentService _stripePaymentService;
        private bool _isLoggedIn;
        private string _loggedInUser;
        private List<Expense> _expenses = new List<Expense>();




        public ExpenseTrackerService(IDbHelper databaseHelper, IStripePaymentService stripePaymentService)
        {
            _dbHelper = databaseHelper;
            _stripePaymentService = stripePaymentService;
            _isLoggedIn = false;
            _loggedInUser = string.Empty;
        }

        // Mock database of users for demonstration purposes
        private readonly Dictionary<string, string> _userDatabase = new Dictionary<string, string>
    {
        { "user1", "password123" },
        { "user2", "password456" }
    };

        // Mock list of pages
        private readonly List<string> _restrictedPages = new List<string> { "Expense History", "Settings" };

        // Method to add an expense
        public int AddExpense(Expense expense)
        {
            _expenses.Add(expense);
            return _dbHelper.AddExpense(expense);
        }

             // Method to process payment and update the status
        public bool ProcessPayment(int expenseId, decimal amount)
        {
            var paymentProcessed = _stripePaymentService.ProcessPayment(amount, "paymentDetails", "userStripeToken");
            if (paymentProcessed)
            {
                _dbHelper.UpdateExpenseStatus(expenseId, "settled");
                return true;
            }
            return false;
        }

        // Method to get expense status
        public string GetExpenseStatus(int expenseId)
        {
            return _dbHelper.GetExpenseStatus(expenseId);
        }

        public void DeleteGroup(int groupId)
        {
            // Step 1: Remove all expenses associated with the group
            _dbHelper.CleanupTestData(groupId);

            // Step 2: Delete the group from the database
            _dbHelper.DeleteGroup(groupId);
        }

        public bool Login(string userName, string password)
        {
            // Simulate login: Check if user exists and password is correct
            if (_userDatabase.ContainsKey(userName) && _userDatabase[userName] == password)
            {
                _isLoggedIn = true;
                _loggedInUser = userName;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _isLoggedIn = false;
            _loggedInUser = string.Empty;
        }

        public bool CanAccessPage(string page)
        {
            if (string.IsNullOrEmpty(page))
            {
                throw new ArgumentException("Page name cannot be null or empty.", nameof(page));
            }

            // If the user is logged in, they can access the restricted page
            if (_isLoggedIn)
            {
                // User can access if the page is in the restricted list
                return _restrictedPages != null && _restrictedPages.Contains(page);
            }

            // If the user is not logged in, they can't access any restricted page
            return false;
        }

        public ExpenseGraphData GetExpenseGraphData()
        {
            // Ensure _expenses is not null before proceeding
            if (_expenses == null)
            {
                _expenses = new List<Expense>(); // Initialize if null
            }

            // Logic to retrieve the graphical data, e.g., total and breakdown of expenses
            var totalAmount = _expenses.Sum(e => e.Amount);
            var breakdown = _expenses
                .GroupBy(e => e.Description)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            return new ExpenseGraphData
            {
                TotalAmount = totalAmount,
                ExpenseBreakdown = breakdown
            };
        }

        public List<Expense> GetExpenseHistory()
        {
            // Ensure the user is logged in before retrieving expenses
            if (!_isLoggedIn)
            {
                throw new InvalidOperationException("User must be logged in to view expense history.");
            }

            // Return the list of expenses
            return _expenses;
        }

    }

}
