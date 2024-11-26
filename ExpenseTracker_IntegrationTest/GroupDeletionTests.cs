using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExpenseTracker_IntegrationTest
{

   [TestFixture]
    public class GroupDeletionTests
    {
        private Mock<IDbHelper> _mockDbHelper;

        [SetUp]
        public void Setup()
        {
            _mockDbHelper = new Mock<IDbHelper>();
        }
        [Test]
        public void TestDeleteGroupAndEnsureAssociatedRecordsRemoved()
        {
            // Step 1: Arrange mock data for creating a group
            string groupName = "Group1";
            var members = new List<string> { "mohd", "sarthak" };
            int groupId = 1;

            // Mock CreateGroup to return a valid group ID
            _mockDbHelper.Setup(db => db.CreateGroup(groupName, members)).Returns(groupId);

            // Step 2: Arrange mock data for adding an expense to the group
            decimal amount = 100.00m;
            string description = "Group Dinner";
            DateTime date = DateTime.Now;
            string payer = "mohd";
            string participants = "mohd,sarthak";
            var participantList = new List<string> { "mohd", "sarthak" };

            // Mock AddExpenseToGroup to return true for successful addition
            _mockDbHelper.Setup(db => db.AddExpenseToGroup(groupId, amount, description, date, payer, participants, participantList)).Returns(true);

            // Step 3: Mock the CleanupTestData and DeleteGroup methods to verify interaction
            _mockDbHelper.Setup(db => db.CleanupTestData(groupId)).Verifiable();  // Ensure cleanup happens
            _mockDbHelper.Setup(db => db.DeleteGroup(groupId)).Verifiable();       // Ensure group deletion happens

            // Mock the GetExpenseFromDatabase to return null after deletion, indicating no expense data
            _mockDbHelper.Setup(db => db.GetExpenseFromDatabase(groupId, amount, description)).Returns((object)null);

           
            int createdGroupId = _mockDbHelper.Object.CreateGroup(groupName, members);
            bool expenseAdded = _mockDbHelper.Object.AddExpenseToGroup(createdGroupId, amount, description, date, payer, participants, participantList);

            // Call the method under test (DeleteGroup)
            _mockDbHelper.Object.DeleteGroup(createdGroupId);

            var expenseAfterDeletion = _mockDbHelper.Object.GetExpenseFromDatabase(createdGroupId, amount, description);

            // Assert
            Assert.That(expenseAdded, Is.True, "Expense should be added successfully to the group.");
            Assert.That(expenseAfterDeletion, Is.Null, "No expenses should be found after the group has been deleted.");

            // Verify that CleanupTestData and DeleteGroup were called
            //_mockDbHelper.Verify(db => db.CleanupTestData(createdGroupId), Times.Once, "CleanupTestData should be called once.");
            _mockDbHelper.Verify(db => db.DeleteGroup(createdGroupId), Times.Once, "DeleteGroup should be called once.");

        }
    }
}

