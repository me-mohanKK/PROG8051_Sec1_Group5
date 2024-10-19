using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpenseTracker_UnitTests
{
    internal class UserService
    {
        private HashSet<string> existingUsernames = new HashSet<string>();
        

        public bool RegisterUser(string email, string username, string password)
        {
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format");
            }

            if (!IsValidPassword(password))
            {
                throw new ArgumentException("Password does not meet complexity requirements");
            }

            // Check if the username already exists
            if (DoesUsernameExist(username))
            {
                throw new ArgumentException("Username already exists");
            }

            // Simulate registering the user by adding the username to the collection
            existingUsernames.Add(username);

            // Assume other registration logic occurs here (like saving email/password)
            return true; // Indicate success
        }

        private bool DoesUsernameExist(string username)
        {
            return existingUsernames.Contains(username);
        }

        public bool IsValidEmail(string email)
        {
            // Implement email validation logic
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }

        public bool IsValidPassword(string password)
        {
            // Implement password complexity checks (e.g., length, special characters)
            return password.Length >= 8; // Example check
        }

        public bool LoginUser(string email, string password)
        {
            // Validate email format
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format");
            }

            // Dummy logic for validation (replace this with actual credential checks)
            // Here, we assume a simple check for the purpose of this example
            if (email == "testuser@gmail.com" && password == "Password@123")
            {
                return true; // Login successful
            }

            return false; // Login failed
        }
    }
}
