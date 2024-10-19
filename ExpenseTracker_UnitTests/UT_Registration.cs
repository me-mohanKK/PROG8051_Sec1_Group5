using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ExpenseTracker_UnitTests
{
    internal class UT_Registration
    {
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userService = new UserService();
        }

        // Test 1: Successful registration with valid user details
        [Test]
        public void RegisterUser_ValidUser()
        {
            
            var email = "newuser@gmail.com";
            var username = "james123";
            var password = "Password@123";

            
            var result = _userService.RegisterUser(email, username, password);

            // Assert: Registration should succeed
            Assert.That(result, Is.True, "Registration should succeed for a valid user.");
        }

        // Test 2: Registration fails with an invalid email format
        [Test]
        public void RegisterUser_InvalidEmail()
        {
          
            var email = "invalid-email";
            var username = "james123";
            var password = "Password@123";

            // Act & Assert: Expect ArgumentException for invalid email
            var ex = Assert.Throws<ArgumentException>(() => _userService.RegisterUser(email, username, password));
            Assert.That(ex.Message, Is.EqualTo("Invalid email format"));
        }

        // Test 3: Registration fails if the username already exists
        [Test]
        public void RegisterUsernameAlreadyExists()
        {
            // Arrange
            var email = "user1@example.com"; // Assume this email is new
            var username = "existinguser"; // Assume this username already exists
            var password = "Password@123";

            // Pre-register the user
            _userService.RegisterUser("existinguser@example.com", username, password); // Ensure this registers correctly

            // Act & Assert: Expect ArgumentException for existing username
            var ex = Assert.Throws<ArgumentException>(() => _userService.RegisterUser(email, username, password));
            Assert.That(ex.Message, Is.EqualTo("Username already exists"));
        }

        // Test 4: Registration fails with a weak password
        [Test]
        public void ValidatePassword_WeakPassword()
        {
          
            var email = "weakuser@example.com";
            var username = "weakuser";
            var password = "weak"; // Weak password

            // Act & Assert: Expect ArgumentException for weak password
            var ex = Assert.Throws<ArgumentException>(() => _userService.RegisterUser(email, username, password));
            Assert.That(ex.Message, Is.EqualTo("Password does not meet complexity requirements"));
        }


    }
}
