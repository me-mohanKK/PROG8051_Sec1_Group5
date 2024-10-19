using ExpenseTrackerApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace ExpenseTracker_UnitTests
{
    [TestFixture]
    public class UT_AddExpense
    {
        private ExpenseLogic expenseLogic;

        [SetUp]
        public void Setup()
        {
            expenseLogic = new ExpenseLogic();
        }

        [Test]
        public void FormatExpense_ShouldReturnCorrectlyFormattedString()
        {
            // Arrange
            decimal amount = 100;
            string description = "Dinner";
            DateTime date = new DateTime(2024, 10, 10);
            string payer = "Mahi";
            int div = 2; // Total participants excluding payer

            string expectedOutput = "$100.00 - Dinner on 2024-10-10 by Mahi split to 3 person(s) $33.33 each (including payer)";

            string result = expenseLogic.FormatExpense(amount, description, date, payer, div);

            // Assert
            ClassicAssert.AreEqual(expectedOutput, result);
        }

        [Test]
        public void FormatExpense_NegativeAmount_ShouldThrowArgumentException()
        {
            // Arrange
            decimal amount = -100;
            string description = "Dinner";
            DateTime date = DateTime.Now;
            string payer = "Mahi";
            int div = 2;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                expenseLogic.FormatExpense(amount, description, date, payer, div));
            Assert.That(ex.Message, Is.EqualTo("Amount must be positive."), "Negative amount should throw an exception.");
        }

        [Test]
        public void FormatExpense_EmptyDescription_ShouldThrowArgumentException()
        {
            // Arrange
            decimal amount = 100;
            string description = "";
            DateTime date = DateTime.Now;
            string payer = "Mahi";
            int div = 2;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                expenseLogic.FormatExpense(amount, description, date, payer, div));
            Assert.That(ex.Message, Is.EqualTo("Description cannot be empty."), "Empty description should throw an exception.");
        }
    }
}
