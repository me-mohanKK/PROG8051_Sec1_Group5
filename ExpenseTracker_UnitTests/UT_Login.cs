using ExpenseTrackerApp;
using NUnit.Framework;
using System.Data.SqlClient;
using ExpenseTracker_UnitTests;


namespace ExpenseTracker_UnitTests
{
    [TestFixture]
    public class UT_Login
    {
        private UserService _userService;

    // This method runs before each test
    [SetUp]
    public void SetUp()
    {
        _userService = new UserService();
    }

    // Test 1: Successful login with valid credentials
    [Test]
    public void LoginUser_ValidCredentials()
    {
        var email = "testuser@gmail.com";
        var password = "Password@123";

       
        var isLoginSuccessful = _userService.LoginUser(email, password);

        // Assert: Login should succeed
        Assert.That(isLoginSuccessful, Is.True, "Login should succeed with valid credentials.");
    }

    // Test 2: Login fails with invalid credentials
    [Test]
    public void LoginUser_InvalidCredentials()
    {
       
        string email = "testuser@gmail.com";
        string password = "WrongPassword";

        bool result = _userService.LoginUser(email, password);

        // Assert
        Assert.That(result, Is.False, "Login should fail with an incorrect password.");
    }

    // Test 3: Login fails with non-existent email
    [Test]
    public void LoginUser_NonExistentEmail()
    {
      
        string email = "nonexistent@gmail.com";
        string password = "Password@123";

        // Act
        bool result = _userService.LoginUser(email, password);

        // Assert
        Assert.That(result, Is.False, "Login should fail with a non-existent email.");
    }
}

    }

