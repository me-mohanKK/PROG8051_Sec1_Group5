using System.Text;
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




namespace ExpenseTrackerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private string connectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";

        public MainWindow()
        {
            InitializeComponent();
            // userName.Visibility = Visibility.Hidden;
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
            //string username = username.;
            string password = passwordBox.Password;
           

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM users WHERE username=@username AND password=@password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", userName.Text);
                cmd.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 1)
                {
                    //MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExpenseEntry expenseEntryWindow = new ExpenseEntry();
                    expenseEntryWindow.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Incorrect Username or Password!", "Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text;
            string username = ExtractName(ruserName.Text);
            string password = rpasswordBox.Password;

            // Validate email format
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate alphanumeric password
            if (!IsValidPassword(password))
            {
                MessageBox.Show("Password must be at least 8 characters long, contain a capital letter, a lowercase letter, and a number.", "Invalid Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Method to validate email format
            
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
                    // MessageTextBlock.Text = "Registration successful!";
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoginSelectionButton_Click(sender, e);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Registration failed!", "Failure", MessageBoxButton.OK, MessageBoxImage.Information);

                    //if (ex.Number == 1062)
                    if (ex.Number == 2627) // Unique constraint error for SQL Server
                    {
                        MessageBox.Show("Username already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

    }
}