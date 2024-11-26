using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_IntegrationTest
{
    public interface IUserAuthenticationService
    {
        bool Login(string userName, string password);
        void Logout();
        bool CanAccessPage(string page);
    }
}
