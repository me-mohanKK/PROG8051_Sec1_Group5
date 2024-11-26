using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ExpenseTracker_IntegrationTest
{
    public class DatabaseHelper : IDbHelper
    {
        private readonly string _connectionString;
        private readonly IDbHelper _dbHelper;

        

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DatabaseHelper(string connectionString, IDbHelper dbHelper)
        {
            _connectionString = connectionString;
            _dbHelper = dbHelper;
        }

        // Method to register a user directly via the database (optional for testing)
        public void RegisterUser(string email, string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO users (email, username, password) VALUES (@Email, @Username, @Password)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.ExecuteNonQuery();
            }
        }

        // Method to check if a user exists
        public bool UserExists(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM users WHERE username = @Username AND password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 1;
            }
        }

        // Method to clean up test user
        public void RemoveTestUser(string email, string username)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM users WHERE email = @Email AND username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.ExecuteNonQuery();
            }
        }


        public int AddExpense(Expense expense)
        {
            int expenseId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Expenses (Amount, Description, Date, Payer, Participants) " +
                               "OUTPUT INSERTED.ExpenseId " + // Assuming ExpenseId is auto-generated
                               "VALUES (@Amount, @Description, @Date, @Payer, @Participants)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Amount", expense.Amount);
                cmd.Parameters.AddWithValue("@Description", expense.Description);
                cmd.Parameters.AddWithValue("@Date", expense.Date);
                cmd.Parameters.AddWithValue("@Payer", expense.Payer);
                cmd.Parameters.AddWithValue("@Participants", expense.Participants);

                try
                {
                    expenseId = (int)cmd.ExecuteScalar(); // Retrieve auto-generated ExpenseId
                }
                catch (SqlException ex)
                {
                    // Handle exception or log error
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return expenseId; // Return the ExpenseId of the newly added expense
        }

        public int CreateGroup(string groupName, List<string> members)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "INSERT INTO Groups (GroupName, Members) OUTPUT INSERTED.GroupId VALUES (@GroupName, @Members)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupName", groupName);
                cmd.Parameters.AddWithValue("@Members", string.Join(",", members));

                try
                {
                    // Execute the command and return the GroupId of the newly created group
                    return (int)cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"An error occurred while creating the group: {ex.Message}");
                    return -1; // Return -1 in case of error
                }
            }
        }

        public void DeleteGroup(int groupId)
        {
            // Step 1: Remove all expenses associated with the group
            CleanupTestData(groupId);

            // Step 2: Delete the group from the database
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Groups WHERE GroupId = @GroupId", connection);
                command.Parameters.AddWithValue("@GroupId", groupId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public bool AddExpenseToGroup(int groupId, decimal amount, string description, DateTime date, string payer, string participants, List<string> participantList)
        {

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "INSERT INTO Expenses (GroupId, Amount, Description, Date, Payer, Participants, SplitMethod, SplitDetails) " +
                               "VALUES (@GroupId, @Amount, @Description, @Date, @Payer, @Participants, @SplitMethod, @SplitDetails)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupId", groupId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Payer", payer);
                cmd.Parameters.AddWithValue("@Participants", participants);
                cmd.Parameters.AddWithValue("@SplitMethod", participantList.Count);  // You can modify split method logic as needed
                cmd.Parameters.AddWithValue("@SplitDetails", string.Join(",", participantList));

                try
                {
                    // Execute the command, return true if successful
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"An error occurred while adding the expense: {ex.Message}");
                    return false; // Return false in case of error
                }
            }
        }

        public dynamic GetExpenseFromDatabase(int groupId, decimal amount, string description)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Expenses WHERE GroupId = @GroupId AND Amount = @Amount AND Description = @Description";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupId", groupId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Description", description);

                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new
                            {
                                GroupId = reader["GroupId"],
                                Amount = reader["Amount"],
                                Description = reader["Description"],
                                Date = reader["Date"],
                                Payer = reader["Payer"],
                                Participants = reader["Participants"]
                            };
                        }
                        else
                        {
                            return null; // Return null if no matching expense is found
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"An error occurred while retrieving the expense: {ex.Message}");
                    return null; // Return null in case of error
                }
            }
        }

        public void CleanupTestData(int groupId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Start a transaction to ensure both deletion operations are atomic
                SqlTransaction transaction = conn.BeginTransaction();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = transaction;

                try
                {
                    // Delete expenses associated with the group
                    cmd.CommandText = "DELETE FROM Expenses WHERE GroupId = @GroupId";
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.ExecuteNonQuery();

                    // Delete the group itself
                    cmd.CommandText = "DELETE FROM Groups WHERE GroupId = @GroupId";
                    cmd.ExecuteNonQuery();

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    // Rollback transaction in case of error
                    transaction.Rollback();
                    Console.WriteLine($"An error occurred while cleaning up test data: {ex.Message}");
                }
            }
        }

        public void UpdateExpenseStatus(int expenseId, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Expenses SET Status = @Status WHERE ExpenseId = @ExpenseId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ExpenseId", expenseId);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        public string GetExpenseStatus(int expenseId)
        {
            string status = string.Empty;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Status FROM Expenses WHERE ExpenseId = @ExpenseId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ExpenseId", expenseId);

                try
                {
                    status = (string)cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return status;
        }


    }
}
