using ExpenseTrackerApp;
using NUnit.Framework;
using System.Data.SqlClient;
using ExpenseTracker_UnitTests;


namespace ExpenseTracker_UnitTests
{
    [TestFixture]
    public class UT_Registration
    {
        private DatabaseManager databaseManager;
        private string connectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";

        // This method runs before each test
        [SetUp]
        public void Setup()
        {
            // Initialize the UserService before each test
            databaseManager = new DatabaseManager();

            // Clear the users table before each test for isolation
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM users", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void RegisterUser_ValidUser()
        {
            // Arrange: Prepare user registration details
            var email = "james123@gmail.com";
            var username = "james";
            var password = "Password@123";  // Valid password

            // Act: Call the RegisterUser method
            var result = databaseManager.RegisterUser(email, username, password);

            // Assert: Verify the registration was successful
            Assert.That(result.Success, Is.True, "User registration should succeed.");
            Assert.That(result.Message, Is.EqualTo("Registration successful"));
        }

        [Test]
        public void RegisterUser_InvalidEmail()
        {
            // Arrange: Prepare invalid email details
            var email = "invalid-email"; // Invalid email
            var username = "james123";
            var password = "Password@123";  // Valid password

            // Act: Call the RegisterUser method
            var result = databaseManager.RegisterUser(email, username, password);

            // Assert: Verify registration fails due to invalid email
            Assert.That(result.Success, Is.False, "User registration should fail due to invalid email.");
            Assert.That(result.Message, Is.EqualTo("Invalid email format"));
        }

        [Test]
        public void RegisterUser_UsernameAlreadyExists()
        {
            // Arrange: Register a user first
            var existingUserEmail = "matt@gmail.com";
            var existingUsername = "matt123";
            var existingPassword = "Password@123";
            databaseManager.RegisterUser(existingUserEmail, existingUsername, existingPassword);

            // Act: Attempt to register the same user again
            var duplicateUserEmail = "matt@gmail.com";
            var duplicateUsername = "matt123"; // Same username
            var duplicatePassword = "NewPassword@123";
            var result = databaseManager.RegisterUser(duplicateUserEmail, duplicateUsername, duplicatePassword);

            // Assert: Verify registration fails due to existing username
            Assert.That(result.Success, Is.False, "User registration should fail due to existing username.");
            Assert.That(result.Message, Is.EqualTo("Username or email already exists"));
        }

        [Test]
        public void ValidatePassword_WeakPassword()
        {
            
            var weakPassword = "12345"; // Example of a weak password

           
            var isValid = databaseManager.ValidatePassword(weakPassword);

            // Assert: Verify that the password validation fails
            Assert.That(isValid, Is.False, "Password validation should fail for weak passwords.");
        }


       

        
    }
}
