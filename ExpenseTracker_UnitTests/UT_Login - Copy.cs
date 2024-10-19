using ExpenseTrackerApp;
using NUnit.Framework;
using System.Data.SqlClient;
using ExpenseTracker_UnitTests;


namespace ExpenseTracker_UnitTests
{
    [TestFixture]
    public class UT_Login
    {
        private DatabaseManager databaseManager;
        private string connectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";

        // This method runs before each test
        [SetUp]
        public void SetUp()
        {
            databaseManager = new DatabaseManager();
            SetUpTestData();
        }

        [TearDown]
        public void TearDown()
        {
            CleanUpTestData();
        }

        // Test 1: Successful login with valid credentials
        [Test]
        public void LoginUser_ValidCredentials()
        {
            // Arrange: Valid credentials
            var email = "testuser@gmail.com";
            var password = "Password@123";

            // Act: Call the LoginUser method
            var isLoginSuccessful = databaseManager.LoginUser(email, password);

            // Assert: Login should succeed
            Assert.That(isLoginSuccessful, Is.True, "Login should succeed with valid credentials.");

        }

        // Test 2: Login fails with invalid credentials
        [Test]
        public void LoginUser_InvalidCredentials()
        {
            // Arrange
            string email = "testuser@gmail.com";
            string password = "WrongPassword";

            // Act
            bool result = databaseManager.LoginUser(email, password);

            // Assert
            Assert.That(result, Is.False, "Login should fail with an incorrect password.");
        }

        // Test 3: Login fails with non-existent email
        [Test]
        public void LoginUser_NonExistentEmail()
        {
            // Arrange
            string email = "nonexistent@gmail.com";
            string password = "Password@123";

            // Act
            bool result = databaseManager.LoginUser(email, password);

            // Assert
            Assert.That(result, Is.False, "Login should fail with an incorrect password.");
        }

        // Helper method to set up test data
        private void SetUpTestData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Insert a test user into the database
                string query = "INSERT INTO users (Email, Username, Password) VALUES ('testuser@gmail.com', 'test123', 'Password@123')";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        // Helper method to clean up test data
        private void CleanUpTestData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Delete the test user from the database
                string query = "DELETE FROM users WHERE Email = 'testuser@gmail.com'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }


    }
}
