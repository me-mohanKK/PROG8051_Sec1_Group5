using Moq; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_IntegrationTest
{

    [TestFixture]
    public class GroupAndExpenseIntegrationTests
    {
        private Mock<IDbHelper> _mockDbHelper;

        [SetUp]
        public void Setup()
        {
            _mockDbHelper = new Mock<IDbHelper>();
        }

        [Test]
        public void TestCreateGroupAndAddExpense()
        {
            // Step 1: Define expected behavior for CreateGroup and AddExpenseToGroup using Moq
            string groupName = "Group1";
            var members = new List<string> { "mohd", "sarthak" };
            int expectedGroupId = 1;

            // Mock CreateGroup to return a group ID (1)
            _mockDbHelper.Setup(db => db.CreateGroup(groupName, members)).Returns(expectedGroupId);

            decimal amount = 100.00m;
            string description = "Group Dinner";
            DateTime date = DateTime.Now;
            string payer = "mohd";
            string participants = "mohd,sarthak";
            var participantList = new List<string> { "mohd", "sarthak" };

            // Mock AddExpenseToGroup to return true (indicating success)
            _mockDbHelper.Setup(db => db.AddExpenseToGroup(expectedGroupId, amount, description, date, payer, participants, participantList)).Returns(true);

            // Mock GetExpenseFromDatabase to return mock expense data
            var mockExpense = new { GroupId = expectedGroupId, Amount = amount, Description = description };
            _mockDbHelper.Setup(db => db.GetExpenseFromDatabase(expectedGroupId, amount, description)).Returns(mockExpense);

            // Step 2: Create the group
            int groupId = _mockDbHelper.Object.CreateGroup(groupName, members);

            // Step 3: Add an expense to the group
            bool expenseAdded = _mockDbHelper.Object.AddExpenseToGroup(groupId, amount, description, date, payer, participants, participantList);

            // Step 4: Retrieve the expense from the database
            var expense = _mockDbHelper.Object.GetExpenseFromDatabase(groupId, amount, description);

            // Assertions using Assert.That
            Assert.That(expenseAdded, Is.True, "Expense was not added successfully.");
            Assert.That(expense, Is.Not.Null, "Expense data was not found.");
            Assert.That(expense.GroupId, Is.EqualTo(groupId), "Group ID does not match.");
            Assert.That(expense.Amount, Is.EqualTo(amount), "Expense amount does not match.");
            Assert.That(expense.Description, Is.EqualTo(description), "Expense description does not match.");
        }

        [Test]
        public void AddExpenseToGroupWithNoMembersTest()
        {
            var amount = 100.00m;
            var description = "No Members";
            var date = DateTime.Now;
            var payer = "user1";
            var participants = "mohd";
            List<string> participantList = new List<string>(); // No participants

            var expenseAdded = _mockDbHelper.Object.AddExpenseToGroup(1, amount, description, date, payer, participants, participantList);
            Assert.That(expenseAdded, Is.False, "Expense should not be added if there are no members in the group.");
        }
    }
}
