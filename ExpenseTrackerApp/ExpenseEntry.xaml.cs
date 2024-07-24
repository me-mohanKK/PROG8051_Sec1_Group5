using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExpenseTrackerApp
{
    /// <summary>
    /// Interaction logic for ExpenseEntry.xaml
    /// </summary>
    public partial class ExpenseEntry : Window
    {
        private string connectionString = "Server=localhost;Port=3306;Database=ExpenseTracker;Uid=root;";
        private ObservableCollection<string> expenseList = new ObservableCollection<string>();

        public ExpenseEntry()
        {
            InitializeComponent();
            ExpenseListBox.ItemsSource = expenseList;
            LoadExpenses();
        }



        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBlock.Text = "";

            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageTextBlock.Text = "Invalid amount. Please enter a positive number.";
                return;
            }

            string description = DescriptionTextBox.Text;
            DateTime? date = DatePicker.SelectedDate;
            string payer = PayerTextBox.Text;
            string participants = ParticipantsTextBox.Text;

            if (string.IsNullOrEmpty(description) || !date.HasValue || string.IsNullOrEmpty(payer) || string.IsNullOrEmpty(participants))
            {
                MessageTextBlock.Text = "All fields must be filled out.";
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO expenses (amount, description, date, payer, participants) VALUES (@amount, @description, @date, @payer, @participants)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@date", date.Value);
                cmd.Parameters.AddWithValue("@payer", payer);
                cmd.Parameters.AddWithValue("@participants", participants);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageTextBlock.Text = "Expense added successfully.";
                    MessageTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    LoadExpenses();
                }
                catch (MySqlException ex)
                {
                    MessageTextBlock.Text = "An error occurred: " + ex.Message;
                }
            }
        }

        private void LoadExpenses()
        {
            expenseList.Clear();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT amount, description, date, payer, participants FROM expenses";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal amount = reader.GetDecimal("amount");
                    string description = reader.GetString("description");
                    DateTime date = reader.GetDateTime("date");
                    string payer = reader.GetString("payer");
                    string participants = reader.GetString("participants");

                    string expense = $"{amount:C} - {description} on {date.ToShortDateString()} by {payer} for {participants}";
                    expenseList.Add(expense);
                }
            }
        }

    }
}
