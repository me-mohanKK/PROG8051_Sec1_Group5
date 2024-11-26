using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ExpenseTrackerApp
{
    public class DatabaseManager
    {

        public bool ValidateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM users WHERE username=@username AND password=@password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 1;
            }
        }

        public RegistrationResult RegisterUser(string email, string username, string password)
        {
            // Validate email format before proceeding
            if (!IsValidEmail(email))
            {
                return new RegistrationResult
                {
                    Success = false,
                    Message = "Invalid email format"
                };
            }

            // Check if the username or email already exists
            if (UserExists(email, username))
            {
                return new RegistrationResult
                {
                    Success = false,
                    Message = "Username or email already exists"
                };
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO users (email, username, password) VALUES (@Email, @Username, @Password)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    cmd.ExecuteNonQuery();
                    return new RegistrationResult
                    {
                        Success = true,
                        Message = "Registration successful"
                    };
                }
                catch (SqlException ex) when (ex.Number == 2627) // unique constraint violation
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        Message = "Username or email already exists"
                    };
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    return new RegistrationResult
                    {
                        Success = false,
                        Message = "An error occurred: " + ex.Message
                    };
                }
            }
        }

        public bool ValidatePassword(string password)
        {
            // Password must be at least 8 characters long, contain one uppercase letter,
            // one lowercase letter, one digit, and one special character
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        // Private method to validate email format
        private bool IsValidEmail(string email)
        {
            // Use a simple regex to check email format
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Private method to check if the user already exists
        private bool UserExists(string email, string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM users WHERE email = @Email OR username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Username", username);

                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Return true if user exists
            }
        }

        public bool LoginUser(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM users WHERE Email = @Email AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                int count = (int)cmd.ExecuteScalar();
                return count == 1;
            }
        }
        // private string connectionString = "Server=localhost;Port=3306;Database=ExpenseTracker;Uid=root;";

        private string connectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";

        //private string connectionString = "Data Source=DESKTOP-TIKT6T7\\SQLEXPRESS22;Initial Catalog=DbTest;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";





























        public void CreateUsersTable()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS users (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        username VARCHAR(50) NOT NULL UNIQUE,
                        password VARCHAR(255) NOT NULL
                    );";

                SqlCommand cmd = new SqlCommand(createTableQuery, conn);

                string createExpenseTableQuery = @"CREATE TABLE IF NOT EXISTS expenses (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    amount DECIMAL(10, 2) NOT NULL,
                    description VARCHAR(255) NOT NULL,
                    date DATETIME NOT NULL,
                    payer VARCHAR(50) NOT NULL,
                    participants VARCHAR(255) NOT NULL);";

                SqlCommand expensecmd = new SqlCommand(createExpenseTableQuery, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    expensecmd.ExecuteNonQuery();
                    Console.WriteLine("Table 'users' created successfully or already exists.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred while creating the table: " + ex.Message);
                }
            }
        }

        public void CreateGroupsTable()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS users (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        username VARCHAR(50) NOT NULL UNIQUE,
                        password VARCHAR(255) NOT NULL
                    );";

                SqlCommand cmd = new SqlCommand(createTableQuery, conn);

                string createExpenseTableQuery = @"CREATE TABLE IF NOT EXISTS expenses (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    amount DECIMAL(10, 2) NOT NULL,
                    description VARCHAR(255) NOT NULL,
                    date DATETIME NOT NULL,
                    payer VARCHAR(50) NOT NULL,
                    participants VARCHAR(255) NOT NULL);";

                SqlCommand expensecmd = new SqlCommand(createExpenseTableQuery, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    expensecmd.ExecuteNonQuery();
                    Console.WriteLine("Table 'users' created successfully or already exists.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred while creating the table: " + ex.Message);
                }
            }
        }
        
    }
    

}
