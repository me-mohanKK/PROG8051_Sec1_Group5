﻿
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
using Org.BouncyCastle.Utilities;
using System.Data.SqlClient;
using System.Data;
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;

namespace ExpenseTrackerApp
{
    public partial class ExpenseEntry : Window
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";
        private ObservableCollection<string> expenseList = new ObservableCollection<string>();
        private ObservableCollection<string> participantList = new ObservableCollection<string>();
        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();
        public ObservableCollection<string> Participants { get; set; }
        public ObservableCollection<Participant> ETParticipants { get; set; }
        private int splitMethod;
        ExpenseLogic expenseLogic = new ExpenseLogic();

        public ExpenseEntry()
        {
            InitializeComponent();
            ExpenseListBox.ItemsSource = expenseList;
            //MainExpenseListBox.ItemsSource = Expenses;
            ParticipantListBox.ItemsSource = participantList;
            Participants = new ObservableCollection<string>();

            LoadExpenses();
            PopulateParticipantsDropdown();
            LoadExpensesFromDatabase();
            LoadParticipants();

            /*  Expenses = new ObservableCollection<Expense>
              {
                  new Expense { Date = DateTime.Now, Description = "Groceries", Amount = 150.00m, Participants = "John, Mike, Peter" },
                  new Expense { Date = DateTime.Now, Description = "Utilities", Amount = 75.00m, Participants = "Mike, Peter" }
              };*/

            /*   UserBalances = new ObservableCollection<UserBalance>
               {
                   new UserBalance { UserName = "John", Balance = 225.00m },
                   new UserBalance { UserName = "Peter", Balance = -75.00m }
               };*/

            // Bind data to ListView
            ExpensesListView.ItemsSource = Expenses;
            //BalanceListView.ItemsSource = UserBalances;
        }

        public void AddExpenseButton_Click(object sender, RoutedEventArgs e)
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

            
            string formattedExpense = expenseLogic.FormatExpense(amount, description, date.Value, payer, participantList.Count);

            // Use formattedExpense as needed
            MessageTextBlock.Text = formattedExpense;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO expenses (amount, description, date, payer, participants, split_method, split_details) VALUES (@amount, @description, @date, @payer, @participants, @split_method, @split_details)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@date", date.Value);
                cmd.Parameters.AddWithValue("@payer", payer);
                cmd.Parameters.AddWithValue("@participants", participants);
                cmd.Parameters.AddWithValue("@split_method", participantList.Count.ToString());
                cmd.Parameters.AddWithValue("@split_details", "");

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageTextBlock.Text = "Expense added successfully.";
                    MessageTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    LoadExpenses();
                }
                catch (SqlException ex)
                {
                    MessageTextBlock.Text = "An error occurred: " + ex.Message;
                }
            }
        }

       /* public string FormatExpense(decimal amount, string description, DateTime date, string payer, int div)
        {
            *//* // Calculate the number of participants from div
             int numberOfPersons = div > 0 ? div : 1; // Use div directly if it's more than 0, else fallback to 1
             decimal splitAmount = amount / numberOfPersons; // Calculate the split amount
             return $"{amount:C} - {description} on {date.ToShortDateString()} by {payer} split to {numberOfPersons} person(s) {splitAmount:F2} equally";
         *//*
            // Calculate total participants including the payer
            int totalParticipants = Math.Max(1, div + 1); // Include the payer in the count
            decimal splitAmount = amount / totalParticipants; // Calculate each participant's share

            return $"{amount:C} - {description} on {date.ToShortDateString()} by {payer} split to {totalParticipants} person(s) {splitAmount:F2} each (including payer)";
        }*/


            public void LoadExpenses()
        {
            expenseList.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT amount, description, date, payer, participants, split_method FROM expenses";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal amount = reader.GetDecimal(reader.GetOrdinal("amount"));
                    string description = reader.GetString(reader.GetOrdinal("description"));
                    DateTime date = reader.GetDateTime(reader.GetOrdinal("date"));
                    string payer = reader.GetString(reader.GetOrdinal("payer"));

                    // Get the split method
                    int splitMethodValue = reader.IsDBNull(reader.GetOrdinal("split_method")) ? 1 : reader.GetInt32(reader.GetOrdinal("split_method"));
                    int div = splitMethodValue; // Directly assign splitMethodValue

                    // Use the extracted method for formatting
                    string expense = expenseLogic.FormatExpense(amount, description, date, payer, div); // Use the calculator instance
                    expenseList.Add(expense);
                }
            }
        }

        private void LoadParticipants()
        {
            // Assuming you have a list of participants
             ETParticipants = new ObservableCollection<Participant>
    {
       new Participant { Name = "John", TotalOwed = 200, TotalOwes = 50 },
        new Participant { Name = "Mike", TotalOwed = 100, TotalOwes = 75 },
        new Participant { Name = "Peter", TotalOwed = 50, TotalOwes = 100 }
    };

    ParticipantsListBox.ItemsSource = ETParticipants;
        }

        private void LoadExpensesFromDatabase()
        {
            // Clear existing data
            Expenses.Clear();

            // SQL query to fetch expenses from the database
            string query = "SELECT date, description, amount, participants FROM expenses";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        // Create a new Expense instance for each record retrieved
                        var expense = new Expense
                        {
                            Date = reader.GetDateTime(reader.GetOrdinal("date")),
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                            Participants = reader.GetString(reader.GetOrdinal("participants"))
                        };

                        // Add the expense to the ObservableCollection
                        Expenses.Add(expense);
                    }
                }
            }

            // Bind the data to the ListView or ListBox (if necessary)
            ExpensesListView.ItemsSource = Expenses;  // ListView binding
            //MainExpenseListBox.ItemsSource = Expenses;  // ListBox binding
        }

        public class Participant
        {
            public string Name { get; set; }
            public decimal TotalOwed { get; set; }
            public decimal TotalOwes { get; set; }
        }

        private void PopulateParticipantsDropdown()
        {

            // Example participants list; this can be fetched from the database
            List<string> participants = new List<string> { "John", "Mike", "Peter" };

            ParticipantsComboBox.ItemsSource = participants;       

            
        }
        private void ParticipantsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ParticipantsListBox.SelectedItem is Participant selectedParticipant)
            {
                // Access the participant's Name property
                string participantName = selectedParticipant.Name;

                // Pass the participant name to display the corresponding balance
                DisplayParticipantBalance(participantName);
            }
        }

        private void DisplayParticipantBalance(string participant)
        {
            // Hardcoded balance data for demonstration purposes
            decimal totalOwed = 0;
            decimal totalOwes = 0;

            switch (participant)
            {
                case "John":
                    totalOwed = 200;  // John is owed $200
                    totalOwes = 50;   // John owes $50
                    break;
                case "Mike":
                    totalOwed = 100;  // Mike is owed $100
                    totalOwes = 75;   // Mike owes $75
                    break;
                case "Peter":
                    totalOwed = 50;   // Peter is owed $50
                    totalOwes = 100;  // Peter owes $100
                    break;
                default:
                    totalOwed = 0;    // Default case if participant not found
                    totalOwes = 0;
                    break;
            }

            // Update the pie chart with the hardcoded values
            UpdatePieChart(totalOwed, totalOwes);
        }

        private void UpdatePieChart(decimal totalOwed, decimal totalOwes)
        {
            // Clear previous chart data
            BalancePieChart.Series.Clear();

            // Create series for the pie chart
            PieSeries seriesOwed = new PieSeries
            {
                Title = "Owed to Others",
                Values = new ChartValues<decimal> { totalOwes },
                DataLabels = true,
                LabelPoint = point => $"{point.Y:C}"
            };

            PieSeries seriesOwes = new PieSeries
            {
                Title = "Owed by Others",
                Values = new ChartValues<decimal> { totalOwed },
                DataLabels = true,
                LabelPoint = point => $"{point.Y:C}"
            };

            // Update pie chart with new data
            BalancePieChart.Series.Add(seriesOwed);
            BalancePieChart.Series.Add(seriesOwes);
        }

        private void ParticipantsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle the selection change
            if (ParticipantsComboBox.SelectedItem != null)
            {
                string selectedParticipant = ParticipantsComboBox.SelectedItem.ToString();

                // Check if the participant is not already in the list
                if (!participantList.Contains(selectedParticipant))
                {
                    // Add the selected participant to the ObservableCollection
                    participantList.Add(selectedParticipant);
                }
                // Update split method after adding participant
                UpdateSplitMethod();
            }
        }

        private void UpdateSplitMethod()
        {
            int numberOfParticipants = participantList.Count;

            // Assuming splitMethod represents the number of participants minus one
            // If no participants, set to zero
            splitMethod = (numberOfParticipants > 1) ? numberOfParticipants - 1 : 0;

            // Debugging log to confirm the split method value
            Debug.WriteLine($"Updated split method: {splitMethod}");
        }

        private void ExpensesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExpensesListView.SelectedItem is Expense selectedExpense)
            {
                MessageBox.Show($"Details for {selectedExpense.Description}: Amount: {selectedExpense.Amount}, Participants: {selectedExpense.Participants}",
                                "Expense Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        public class Expense
        {
            public DateTime Date { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public string Participants { get; set; }
            public int ExpenseID { get; set; }
        }


        /*  private void RemoveButton_Click(object sender, SelectionChangedEventArgs e)
          {
              //Participants.Remove();
          }*/


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the application?", "Confirm Exit", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
            else if (result == MessageBoxResult.No)
            {
                // Do nothing, just close the message box
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Get the participant from the button's CommandParameter
            var button = sender as FrameworkElement;
            var participant = button?.DataContext as string;

            if (participant != null)
            {
                // Remove the participant from the collection
                Participants.Remove(participant);
                participantList.Remove(participant);
            }
        }

         private void SaveGroup_Click(object sender, RoutedEventArgs e)
        {
            string groupName = GroupNameTextBox.Text.Trim();
            string groupMembers = GroupMembersTextBox.Text.Trim();

            if (string.IsNullOrEmpty(groupName) || groupName == "Group Name" ||
                string.IsNullOrEmpty(groupMembers) || groupMembers == "Enter member names, separated by commas")
            {
                MessageBox.Show("Please enter a valid group name and members.");
                return;
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Groups (GroupName, Members) VALUES (@GroupName, @Members)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupName", groupName);
                cmd.Parameters.AddWithValue("@Members", groupMembers);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Group created successfully.");
                    LoadGroups();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void RemovePlaceholderText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Foreground == Brushes.Gray)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void AddPlaceholderText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Foreground = Brushes.Gray;

                if (textBox == GroupNameTextBox)
                    textBox.Text = "Group Name";
                else if (textBox == GroupMembersTextBox)
                    textBox.Text = "Enter member names, separated by commas";
            }
        }



        private void LoadGroups()
        {
            GroupsListBox.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT GroupName FROM Groups";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    GroupsListBox.Items.Add(reader["GroupName"].ToString());
                }
            }
        }

        private void DeleteExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedExpense = ExpensesListView.SelectedItem as Expense; // Replace `Expense` with your actual type
            if (selectedExpense == null)
            {
                MessageBox.Show("Please select an expense to delete.");
                return;
            }

            // Confirm deletion with the user
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // Perform database operation to delete the selected expense
                using (SqlConnection conn = new SqlConnection(connectionString)) // Ensure you have a valid connection string
                {
                    conn.Open();
                    string query = "DELETE FROM Expenses WHERE ExpenseId = @ExpenseId"; // Adjust the query according to your table schema
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ExpenseId", selectedExpense.ExpenseID); // Replace `Id` with your expense identifier

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Expense deleted successfully.");

                        // Refresh the ListBox or data source
                        LoadExpenses(); // Method to reload or refresh the ListBox
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            
        }

        private void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            string selectedGroup = GroupsListBox.SelectedItem as string;
            if (selectedGroup == null)
            {
                MessageBox.Show("Please select a group to delete.");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the group?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Groups WHERE GroupName = @GroupName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@GroupName", selectedGroup);

                    try
                    {

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Group deleted successfully.");
                        LoadGroups();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        private void GroupsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedGroup = GroupsListBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedGroup))
            {
                GroupDetailsTextBlock.Text = string.Empty;
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string selectGroupDetailsQuery = "SELECT Members FROM Groups WHERE GroupName = @GroupName";
                SqlCommand cmd = new SqlCommand(selectGroupDetailsQuery, conn);
                cmd.Parameters.AddWithValue("@GroupName", selectedGroup);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string groupMembers = reader.GetString(0);
                    GroupDetailsTextBlock.Text = $"Group Name: {selectedGroup}\nMembers: {groupMembers}";
                }
                else
                {
                    GroupDetailsTextBlock.Text = string.Empty;
                }
            }
        }

        private void EditGroup_Click(object sender, RoutedEventArgs e)
        {
            if (GroupsListBox.SelectedItem != null)
            {
                if (GroupsListBox.SelectedItem is DataRowView selectedRow)
                {
                    GroupNameTextBox.Text = selectedRow["GroupName"].ToString();
                    GroupMembersTextBox.Text = selectedRow["GroupMembers"].ToString();
                    GroupIdTextBlock.Text = selectedRow["GroupId"].ToString();
                }
            }
            else
            {
                MessageBox.Show("Please select a group to edit.");
            }
        }
    }
}
