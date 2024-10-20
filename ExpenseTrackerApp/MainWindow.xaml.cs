﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;




namespace ExpenseTrackerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DatabaseManager databaseManager;   
        private string connectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";

        public MainWindow()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();    
            // Access the StackPanel
           registerStackView.Visibility = Visibility.Hidden;
            loginStackView.Visibility= Visibility.Hidden;
        }


        private void LoginSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            loginStackView.Visibility = Visibility.Visible;
            registerStackView.Visibility = Visibility.Hidden;
            selectionPanel.Visibility = Visibility.Hidden;
        }

        private void registerSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            loginStackView.Visibility = Visibility.Hidden;
            registerStackView.Visibility = Visibility.Visible;
            selectionPanel.Visibility = Visibility.Hidden;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            string password = passwordBox.Password;
            bool isValidUser = databaseManager.ValidateUser(userName.Text, password);

            if (isValidUser)
            {
                ExpenseEntry expenseEntryWindow = new ExpenseEntry();
                expenseEntryWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Username or Password!", "Failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text;
            string username = ExtractName(ruserName.Text);
            string password = rpasswordBox.Password;

            // Validate email and password (not shown for brevity)

            RegistrationResult result = databaseManager.RegisterUser(email, username, password);

            if (result.Success)
            {
                MessageBox.Show(result.Message, "Registration Successful!!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginSelectionButton_Click(sender, e);  // Redirect to login after successful registration
            }
            else
            {
                MessageBox.Show(result.Message, "Failed to Register", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Method to validate password strength
        private bool IsValidPassword(string password)
        {
            // Password must be at least 8 characters long, contain one uppercase letter,
            // one lowercase letter, one digit, and one special character
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        private string ExtractName(string input)
        {
            string prefix = "System.Windows.Controls.TextBox: ";
            if (input.StartsWith(prefix))
            {
                return input.Substring(prefix.Length).Trim();
            }
            return input;
        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            forgotPasswordPopup.IsOpen = true;
        }


        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string email = resetEmailTextBox.Text;
            string newPassword = newPasswordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            if (!UserExists(email))
            {
                MessageBox.Show("Email not found.");
                return;
            }

            UpdatePassword(email, newPassword);
            MessageBox.Show("Password has been reset.");
            forgotPasswordPopup.IsOpen = false;
        }

        private bool UserExists(string email)
        {
         
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void UpdatePassword(string email, string newPassword)
        {
        
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Users SET Password = @Password WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.ExecuteNonQuery();
                }
            }
        }

            private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

    }
}