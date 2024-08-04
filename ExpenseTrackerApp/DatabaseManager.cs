using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerApp
{
    class DatabaseManager
    {

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
