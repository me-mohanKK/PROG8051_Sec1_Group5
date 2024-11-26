using ExpenseTrackerApp;
using NUnit.Framework;

    
    namespace ExpenseTracker_IntegrationTest
{
    [TestFixture]
    public class UserRegistrationAndLoginTests
    {
        private DatabaseManager _dbManager;
        private DatabaseHelper _dbHelper;
        private const string ConnectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";

        [SetUp]
        public void SetUp()
        {
            _dbManager = new DatabaseManager(); // Adjust if constructor needs parameters
            _dbHelper = new DatabaseHelper(ConnectionString);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the test data after each test
            string testEmail = "testnew21@example.com";
            string testUserName = "testnew123";
            _dbHelper.RemoveTestUser(testEmail, testUserName);
        }

        [Test]
        public void Test_FullUserRegistrationLoginAndDashboardAccess()
        {
          
            string email = "testnew21@example.com";
            string username = "testnew123";
            string password = "Password@123";

            // Step 1: Register the user
            var registrationResult = _dbManager.RegisterUser(email, username, password);
            Assert.That(registrationResult.Success, Is.True, "Registration failed: " + registrationResult.Message);

            // Step 2: Verify the user can log in with newly created credentials
            bool loginSuccess = _dbManager.LoginUser(email, password);
            Assert.That(loginSuccess, Is.True, "Login failed for registered user.");

            // Step 3: Simulate dashboard access (checking user existence)
            bool userExists = _dbHelper.UserExists(username, password);
            Assert.That(userExists, Is.True, "Dashboard access failed; user does not exist.");

        }

        [Test]
        public void RegisterUser_WithWeakPassword_ShouldFailRegistration()
        {
           
            string email = "user12@email.com";
            string username = "failedpwduser1";

            // Test with a weak password (e.g., less than 8 characters, no uppercase letter, no special character)
            string weakPassword = "weakpass";

            
            var registrationResult = _dbManager.RegisterUser(email, username, weakPassword);


            Assert.That(registrationResult.Success, Is.False, "Expected registration to fail due to weak password.");
            Assert.That(registrationResult.Message, Is.EqualTo("Password does not meet complexity requirements"),
                "Expected specific error message for weak password.");
        }
    }
}