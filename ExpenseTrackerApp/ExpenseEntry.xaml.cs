
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

namespace ExpenseTrackerApp
{
    public partial class ExpenseEntry : Window
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS19;Database=ExpenseTracker;User Id=sa;Password=Conestoga1;";
        private ObservableCollection<string> expenseList = new ObservableCollection<string>();
        private ObservableCollection<string> participantList = new ObservableCollection<string>();
        public ObservableCollection<string> Participants { get; set; }

        public ExpenseEntry()
        {
            InitializeComponent();
            ExpenseListBox.ItemsSource = expenseList;
            MainExpenseListBox.ItemsSource = expenseList;
            ParticipantListBox.ItemsSource = participantList;
            Participants = new ObservableCollection<string>();
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
                cmd.Parameters.AddWithValue("@split_method", Participants.Count().ToString());
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

        private void LoadExpenses()
        {
            expenseList.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT amount, description, date, payer, participants, split_method, split_details FROM expenses";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal amount = reader.GetDecimal("amount");
                    string description = reader.GetString("description");
                    DateTime date = reader.GetDateTime("date");
                    string payer = reader.GetString("payer");
                    string participants = reader.GetString("participants");

                    // Check for NULL values before calling GetString
                    int splitMethodValue = reader.IsDBNull(reader.GetOrdinal("split_method")) ? 1 : reader.GetInt32(reader.GetOrdinal("split_method"));
                    string splitMethod = splitMethodValue.ToString();
                    string splitDetails = reader.IsDBNull(reader.GetOrdinal("split_details")) ? "N/A" : reader.GetString("split_details");
                    int div = 1;
                   try
                    {
                        int.TryParse(splitMethod, out div);
                    } catch (Exception ex)
                    {
                        div = 1;

                    }
                    
                    //int div = int.Parse(splitMethod);
                    string expense = $"{amount:C} - {description} on {date.ToShortDateString()} by {payer} split to {(div == 0 ? 1 : div + 1)} person {(amount/ (div==0?1:div+1)).ToString("F2")} equally";
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
                if (!Participants.Contains(selectedParticipant))
                {
                    Participants.Add(selectedParticipant);
                    participantList.Add(selectedParticipant);
                }
               
                // Do something with the selected participant
                //MessageBox.Show($"Selected participant: {selectedParticipant}");
            }
        }
        

      /*  private void RemoveButton_Click(object sender, SelectionChangedEventArgs e)
        {
            //Participants.Remove();
        }*/

   
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

            Confirmation_PopUp_Click(sender,e);
            // Close the current window (ExpenseEntry)
            

            // Show the login window
           
        }

        private void Confirmation_PopUp_Click(object sender, RoutedEventArgs e)
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
    }
}
