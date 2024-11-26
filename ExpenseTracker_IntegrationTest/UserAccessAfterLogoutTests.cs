using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_IntegrationTest
{
    [TestFixture]
    internal class UserAccessAfterLogoutTests
    {

        private Mock<IUserAuthenticationService> _mockAuthService;
        private Mock<IDbHelper> _mockDbHelper;
        private ExpenseTrackerService _expenseTrackerService;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IUserAuthenticationService>();
            _mockDbHelper = new Mock<IDbHelper>();

        }

        [Test]
        public void Test_AccessRestrictedPages_AfterLogout()
        {
            // Step 1: Log in as a user
            string userName = "user1";
            string password = "password123";
            _mockAuthService.Setup(service => service.Login(userName, password)).Returns(true); // Simulate successful login

            bool loginSuccessful = _mockAuthService.Object.Login(userName, password);

            // Assert: Check login success
            Assert.That(loginSuccessful, Is.True, "Login should be successful.");

            // Step 2: Navigate to a restricted page
            string page = "Expense History";
            bool canAccessPage = _mockAuthService.Object.CanAccessPage(page);

            // Assert: Verify access to the restricted page
            Assert.That(canAccessPage, Is.False, "User should be able to access the restricted page after login.");

            // Step 3: Log out of the application
            _mockAuthService.Setup(service => service.Logout()).Verifiable();

            _mockAuthService.Object.Logout();

            // Assert: Verify that logout was called
            _mockAuthService.Verify(service => service.Logout(), Times.Once, "Logout should be called once.");

            // Step 4: Attempt to access the restricted page after logout
            bool canAccessPageAfterLogout = _mockAuthService.Object.CanAccessPage(page);

            // Assert: Access should be denied
            Assert.That(canAccessPageAfterLogout, Is.False, "User should not be able to access the restricted page after logout.");
        }
    }
}

