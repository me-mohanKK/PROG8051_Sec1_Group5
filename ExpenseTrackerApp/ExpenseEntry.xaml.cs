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
using ExpenseTrackerApp;

namespace ExpenseTrackerApp
{
    public partial class ExpenseEntry : Window
    {
        private string connectionString = "Server=localhost;Port=3306;Database=ExpenseTracker;Uid=root;";
        private ObservableCollection<string> expenseList = new ObservableCollection<string>();

        public ExpenseEntry()
        {
            InitializeComponent();
            ExpenseListBox.ItemsSource = expenseList;
            LoadExpenses();
            PopulateParticipantsDropdown();

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
            if (!date.HasValue)
            {
                MessageTextBlock.Text = "Please select a date.";
                return;
            }
            string payer = PayerTextBox.Text;
            string participants = ParticipantsComboBox.Text;

            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(payer) || string.IsNullOrEmpty(participants))
            {
                MessageTextBlock.Text = "All fields must be filled out.";
                return;
            }

            string splitMethod = (SplitMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (splitMethod == "By Percentage")
            {
                // Validate and parse percentages
                string[] percentages = PercentagesTextBox.Text.Split(',').Select(p => p.Trim()).ToArray();
                if (percentages.Length != participants.Split(',').Length)
                {
                    MessageTextBlock.Text = "The number of percentages must match the number of participants.";
                    return;
                }

                decimal totalPercentage = percentages.Sum(p => decimal.Parse(p));
                if (totalPercentage != 100)
                {
                    MessageTextBlock.Text = "Total percentage must equal 100.";
                    return;
                }
            }
            else if (splitMethod == "By Custom Amounts")
            {
                // Validate and parse custom amounts
                string[] customAmounts = CustomAmountsTextBox.Text.Split(',').Select(a => a.Trim()).ToArray();
                if (customAmounts.Length != participants.Split(',').Length)
                {
                    MessageTextBlock.Text = "The number of custom amounts must match the number of participants.";
                    return;
                }
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO expenses (amount, description, date, payer, participants, split_method, split_details) VALUES (@amount, @description, @date, @payer, @participants, @split_method, @split_details)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@date", date.Value);
                cmd.Parameters.AddWithValue("@payer", payer);
                cmd.Parameters.AddWithValue("@participants", participants);
                cmd.Parameters.AddWithValue("@split_method", splitMethod);
                cmd.Parameters.AddWithValue("@split_details", splitMethod == "By Percentage" ? PercentagesTextBox.Text : CustomAmountsTextBox.Text);

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
                string query = "SELECT amount, description, date, payer, participants, split_method, split_details FROM expenses";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal amount = reader.GetDecimal("amount");
                    string description = reader.GetString("description");
                    DateTime date = reader.GetDateTime("date");
                    string payer = reader.GetString("payer");
                    string participants = reader.GetString("participants");

                    // Check for NULL values before calling GetString
                    string splitMethod = reader.IsDBNull(reader.GetOrdinal("split_method")) ? "N/A" : reader.GetString("split_method");
                    string splitDetails = reader.IsDBNull(reader.GetOrdinal("split_details")) ? "N/A" : reader.GetString("split_details");

                    string expense = $"{amount:C} - {description} on {date.ToShortDateString()} by {payer} for {participants}. Split method: {splitMethod} ({splitDetails})";
                    expenseList.Add(expense);
                }
            }
        }

        private void PopulateParticipantsDropdown()
        {
            // Example participants list; this can be fetched from the database
            List<string> participants = new List<string> { "John", "Mike", "Peter" };

            ParticipantsComboBox.ItemsSource = participants;
        }

        private void ParticipantsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle the selection change
            if (ParticipantsComboBox.SelectedItem != null)
            {
                string selectedParticipant = ParticipantsComboBox.SelectedItem.ToString();
                // Do something with the selected participant
                MessageBox.Show($"Selected participant: {selectedParticipant}");
            }
        }

        private void SplitMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if SplitOptionsPanel is initialized
            if (SplitOptionsPanel != null)
            {
                if (SplitMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    string selectedMethod = selectedItem.Content.ToString();
                    SplitOptionsPanel.Visibility = Visibility.Visible;

                    // Set visibility of panels based on selected split method
                    PercentageSplitPanel.Visibility = selectedMethod == "By Percentage" ? Visibility.Visible : Visibility.Collapsed;
                    CustomAmountsPanel.Visibility = selectedMethod == "By Custom Amounts" ? Visibility.Visible : Visibility.Collapsed;
                    EqualSplitPanel.Visibility = selectedMethod == "Equally" ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    // Handle case when SelectedItem is null
                    SplitOptionsPanel.Visibility = Visibility.Collapsed;
                    PercentageSplitPanel.Visibility = Visibility.Collapsed;
                    CustomAmountsPanel.Visibility = Visibility.Collapsed;
                    EqualSplitPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                // Handle case where SplitOptionsPanel is null
                MessageBox.Show("SplitOptionsPanel is not initialized.");
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

            Confirmation_PopUp_Click(sender,e);
            // Close the current window (ExpenseEntry)
            //this.Close();

            // Show the login window
           // MainWindow loginWindow = new MainWindow();
            //loginWindow.Show();
        }

        private void NewExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            ExpenseEntry newExpenseWindow = new ExpenseEntry();
            newExpenseWindow.Show();

            // Close the current window (optional)
            this.Close();
        }

        private void ExpenseHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle Expense History button click
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle Groups button click
        }

        private void Confirmation_PopUp_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the application?", "Confirm Exit", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
               
                this.Close();

                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
            }
            else if (result == MessageBoxResult.No)
            {
                // Do nothing, just close the message box
            }
        }
    }
}
