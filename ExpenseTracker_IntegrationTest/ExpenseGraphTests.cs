using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_IntegrationTest
{

    [TestFixture]
    public class ExpenseGraphTests
    {
        private Mock<IDbHelper> _mockDbHelper;
        private Mock<IStripePaymentService> _mockPaymentService;
        private ExpenseTrackerService _expenseTrackerService;

        [SetUp]
        public void SetUp()
        {
            // Mock the dependencies
            _mockDbHelper = new Mock<IDbHelper>();
            _mockPaymentService = new Mock<IStripePaymentService>();

            // Initialize the service with mocked dependencies
            _expenseTrackerService = new ExpenseTrackerService(_mockDbHelper.Object, _mockPaymentService.Object);
        }

        [Test]
        public void TestMultipleExpensesReflectedInGraph()
        {
            // Arrange: Add multiple expenses
            var expenses = new List<Expense>
            {
                new Expense { Amount = 50.00m, Description = "Lunch", Date = DateTime.Now.AddDays(-2) },
                new Expense { Amount = 100.00m, Description = "Dinner", Date = DateTime.Now.AddDays(-1) },
                new Expense { Amount = 30.00m, Description = "Coffee", Date = DateTime.Now }
            };

           // Act: Add expenses
            foreach (var expense in expenses)
            {
                _expenseTrackerService.AddExpense(expense);
            }

            // Get the graphical data after expenses are added
            var chartData = _expenseTrackerService.GetExpenseGraphData();

            //Assert: Verify the chart data contains the correct breakdown and totals
           Assert.That(chartData.TotalAmount, Is.EqualTo(180.00m), "Total amount is incorrect.");
            Assert.That(chartData.ExpenseBreakdown.Count, Is.EqualTo(3), "Number of categories is incorrect.");
            Assert.That(chartData.ExpenseBreakdown["Lunch"], Is.EqualTo(50.00m), "Lunch expense amount is incorrect.");
            Assert.That(chartData.ExpenseBreakdown["Dinner"], Is.EqualTo(100.00m), "Dinner expense amount is incorrect.");
            Assert.That(chartData.ExpenseBreakdown["Coffee"], Is.EqualTo(30.00m), "Coffee expense amount is incorrect.");
        }
    }
}
