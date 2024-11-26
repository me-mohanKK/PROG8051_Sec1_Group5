using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using ExpenseTrackerApp.Payments;
using static ExpenseTrackerApp.ExpenseEntry;

namespace ExpenseTracker_IntegrationTest
{

    [TestFixture]
        public class ExpensePaymentTests
        {
            private Mock<IDbHelper> _mockDbHelper;
            private Mock<IStripePaymentService> _mockStripeService;
            private ExpenseTrackerService _expenseTrackerService;

            [SetUp]
            public void SetUp()
            {
                _mockDbHelper = new Mock<IDbHelper>();
                _mockStripeService = new Mock<IStripePaymentService>();
                _expenseTrackerService = new ExpenseTrackerService(_mockDbHelper.Object, _mockStripeService.Object);
            }

            [Test]
            public void Test_AddExpense_And_Payment_Confirmation()
            {
                // Arrange: Mock data for the expense
                var expense = new Expense
                {
                    Amount = 100,
                    Description = "Test Expense with Payment",
                    Payer = "user1",
                    Participants = "user1, user2"
                };

                // Mock the database method to add an expense
                _mockDbHelper.Setup(db => db.AddExpense(It.IsAny<Expense>())).Returns(1); // Simulate returning expense ID

                // Mock the Stripe payment service to simulate a successful payment
                _mockStripeService.Setup(s => s.ProcessPayment(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(true); // Simulate successful payment

                // Act: Add expense and process payment
                var expenseId = _expenseTrackerService.AddExpense(expense);
                var paymentSuccessful = _expenseTrackerService.ProcessPayment(expenseId, expense.Amount);

                // Mock the status check in the database and confirm "settled" status after payment
                _mockDbHelper.Setup(db => db.GetExpenseStatus(expenseId)).Returns("settled");

                // Assert: Verify that payment was successful and expense status is updated
                Assert.That(paymentSuccessful, Is.True, "Payment should be successful.");
                Assert.That(_expenseTrackerService.GetExpenseStatus(expenseId), Is.EqualTo("settled"), "Expense status should be 'settled' after payment.");
            }

        [Test]
        public void Test_AddExpense_And_Payment_Failure()
        {
            // Arrange: Mock data for the expense
            var expense = new Expense
            {
                Amount = 100,
                Description = "Test Expense with Payment Failure",
                Payer = "user1",
                Participants = "user1, user2"
            };

            // Mock the database method to add an expense
            _mockDbHelper.Setup(db => db.AddExpense(It.IsAny<Expense>())).Returns(2); // Simulate returning expense ID

            // Mock the Stripe payment service to simulate a failed payment
            _mockStripeService.Setup(s => s.ProcessPayment(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false); // Simulate failed payment

            // Act: Add expense and process payment
            var expenseId = _expenseTrackerService.AddExpense(expense);
            var paymentSuccessful = _expenseTrackerService.ProcessPayment(expenseId, expense.Amount);

            // Mock the status check in the database and confirm status remains "pending" after payment failure
            _mockDbHelper.Setup(db => db.GetExpenseStatus(expenseId)).Returns("pending");

            // Assert: Verify that payment was unsuccessful and expense status is not updated to "settled"
            Assert.That(paymentSuccessful, Is.False, "Payment should fail.");
            Assert.That(_expenseTrackerService.GetExpenseStatus(expenseId), Is.EqualTo("pending"), "Expense status should remain 'pending' after payment failure.");
        }
    }
}


    
