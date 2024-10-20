using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_UnitTests
{
    internal class AuthService
    {

        private bool _isLoggedIn = false;

        public bool IsLoggedIn => _isLoggedIn;

        public void Login()
        {
            _isLoggedIn = true;
        }

        public void Logout()
        {
            if (!_isLoggedIn)
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            _isLoggedIn = false;
        }
    }
}
