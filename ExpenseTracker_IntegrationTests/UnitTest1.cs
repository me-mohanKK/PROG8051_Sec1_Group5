using NUnit.Framework;
using ExpenseTrackerApp.DataAccess; // Adjust this based on actual namespace of DatabaseManager
using System;

namespace ExpenseTracker_IntegrationTests
{
    [TestFixture]
    public class UserRegistrationAndLoginTests
    {
        private DatabaseManager _dbManager;

        [SetUp]
        public void SetUp()
        {
            _dbManager = new DatabaseManager(); // Ensure this matches the actual constructor
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the test data, like removing the test user
            string testEmail = "testuser@example.com";
            string testUserName = "testuser123";
            RemoveTestUser(testEmail, testUserName);
        }

        [Test]
        public void Test_FullUserRegistrationLoginAndDashboardAccess()
        {
            // Define test user details
            string email = "testuser@example.com";
            string username = "testuser123";
            string password = "Password@123";

            // Step 1: Register a new user
            var registrationResult = _dbManager.RegisterUser(email, username, password);
            Assert.IsTrue(registrationResult.Success, "Registration failed: " + registrationResult.Message);

            // Step 2: Verify the user can log in with newly created credentials
            bool loginSuccess = _dbManager.LoginUser(email, password);
            Assert.IsTrue(loginSuccess, "Login failed for registered user.");

            // Step 3: Simulate dashboard access (This depends on your app; typically, this would involve loading user data)
            bool userExists = _dbManager.ValidateUser(username, password);
            Assert.IsTrue(userExists, "Dashboard access failed; user does not exist.");
        }

        // Utility method to clean up test data
        private void RemoveTestUser(string email, string username)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(_dbManager.ConnectionString))
            {
                conn.Open();
                string query = "DELETE FROM users WHERE email = @Email AND username = @Username";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}