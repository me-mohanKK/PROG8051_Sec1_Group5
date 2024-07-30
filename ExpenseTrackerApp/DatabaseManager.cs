﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerApp
{
    class DatabaseManager
    {
            //private string connectionString = "Server=localhost:3306;Database=ExpenseTracker;";
        private string connectionString = "Server=localhost;Port=3306;Database=ExpenseTracker;Uid=root;";


        // private string connectionString = "Server=localhost:3306;Database=ExpenseTracker;Uid=yourusername;Pwd=yourpassword;";

        public void CreateUsersTable()
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS users (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        username VARCHAR(50) NOT NULL UNIQUE,
                        password VARCHAR(255) NOT NULL
                    );";

                    MySqlCommand cmd = new MySqlCommand(createTableQuery, conn);

                string createExpenseTableQuery = @"CREATE TABLE IF NOT EXISTS expenses (
    id INT AUTO_INCREMENT PRIMARY KEY,
    amount DECIMAL(10, 2) NOT NULL,
    description VARCHAR(255) NOT NULL,
    date DATETIME NOT NULL,
    payer VARCHAR(50) NOT NULL,
    participants VARCHAR(255) NOT NULL
);";

                MySqlCommand expensecmd = new MySqlCommand(createExpenseTableQuery, conn);

                try
                    {
                        cmd.ExecuteNonQuery();
                    expensecmd.ExecuteNonQuery();
                        Console.WriteLine("Table 'users' created successfully or already exists.");
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("An error occurred while creating the table: " + ex.Message);
                    }
                }
        }



}



}