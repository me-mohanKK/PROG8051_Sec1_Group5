using System.Configuration;
using System.Data;
using System.Windows;

namespace ExpenseTrackerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DatabaseManager dbSetup = new DatabaseManager();
            dbSetup.CreateUsersTable();
        }
    }

}
