using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_UnitTests
{
    public class UT_Logout
    {

        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _authService = new AuthService();
        }

        [Test]
        public void Logout_WhenUserIsLoggedIn_ShouldLogoutSuccessfully()
        {
            // Arrange
            _authService.Login(); // User must be logged in first

           
            _authService.Logout();

            // Assert
            Assert.That(_authService.IsLoggedIn, Is.False);
        }

        [Test]
        public void Logout_WhenUserIsNotLoggedIn_ShouldThrowInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _authService.Logout());
            Assert.That(ex.Message, Is.EqualTo("User is not logged in."));
        }
    }
}
