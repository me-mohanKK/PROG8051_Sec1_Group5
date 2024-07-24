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
using MySql.Data.MySqlClient;




namespace ExpenseTrackerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private string connectionString = "Server=localhost;Port=3306;Database=ExpenseTracker;Uid=root;";

        public MainWindow()
        {
            InitializeComponent();
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //string username = username.;
            string password = passwordBox.Password;
           

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM users WHERE username=@username AND password=@password";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", userName.Text);
                cmd.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 1)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExpenseEntry expenseEntryWindow = new ExpenseEntry();
                    expenseEntryWindow.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Login Failes!", "Failed", MessageBoxButton.OK, MessageBoxImage.Information);

                  
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            //string username = username.;
            string password = passwordBox.Password;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO users (username, password) VALUES (@username, @password)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", userName.Text);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    cmd.ExecuteNonQuery();
                    // MessageTextBlock.Text = "Registration successful!";
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Registration failed!", "Failure", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (ex.Number == 1062) // Duplicate entry error code
                    {
                       // MessageTextBlock.Text = "Username already exists.";
                    }
                    else
                    {
                       // MessageTextBlock.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}