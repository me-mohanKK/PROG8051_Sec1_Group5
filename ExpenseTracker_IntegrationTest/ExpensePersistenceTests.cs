using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_IntegrationTest
{
    [TestFixture]
    public class ExpensePersistenceTests
    {
        private ExpenseTrackerService _expenseTrackerService;
        private Mock<IDbHelper> _mockDbHelper;
        private Mock<IStripePaymentService> _mockStripePaymentService;

        [SetUp]
        public void SetUp()
        {
            // Mock the dependencies
            _mockDbHelper = new Mock<IDbHelper>();
            _mockStripePaymentService = new Mock<IStripePaymentService>();

            // Pass mocked dependencies to the constructor
            _expenseTrackerService = new ExpenseTrackerService(_mockDbHelper.Object, _mockStripePaymentService.Object);
        }
    

        [Test]
        public void TestExpensePersistenceAfterLogoutAndLogin()
        {
            // Arrange: Log in and add an expense
            var userName = "user1";
            var password = "password123";

            // Log in with the given credentials
            bool loginSuccess = _expenseTrackerService.Login(userName, password);
            Assert.That(loginSuccess, Is.True, "Login failed with valid credentials.");

            var expense = new Expense { Amount = 50.00m, Description = "Lunch", Date = DateTime.Now };
            _expenseTrackerService.AddExpense(expense);

            // Act: Log out, then log in again
            _expenseTrackerService.Logout();
            loginSuccess = _expenseTrackerService.Login(userName, password);
            Assert.That(loginSuccess, Is.True, "Login failed after logout.");

            // Act: Verify the added expense is still in the expense history
            var expenseHistory = _expenseTrackerService.GetExpenseHistory();
            var addedExpense = expenseHistory.FirstOrDefault(e => e.Description == "Lunch");

            // Assert: Check that the expense is present and the amount is correct
            Assert.That(addedExpense, Is.Not.Null, "The expense was not found in the history.");
            Assert.That(addedExpense.Amount, Is.EqualTo(50.00m), "The expense amount is incorrect.");
        }
    }

}
