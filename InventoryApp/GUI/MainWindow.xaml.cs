using InventoryApp.DAL;
using InventoryApp.GUI;
using System.Data.SqlClient;
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
using System.Windows.Threading;

namespace InventoryApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User currentUser;
        private readonly DatabaseManager _databaseManager;
        private DatabaseHelper databaseHelper;
        DispatcherTimer clockTimer = new DispatcherTimer();

        public MainWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            txtTitle.Text = "HI! " + currentUser.FirstName;
            _databaseManager = new DatabaseManager();
            databaseHelper = new DatabaseHelper();

            // format date and time
            txtDateDisplay.Text = DateTime.Now.ToString("dddd MMMM dd, yyyy");
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += ClockTimerEngine;
            clockTimer.Start();
        }

        private void ClockTimerEngine(object? sender, EventArgs e)
        {
            txtTimeDisplay.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "INSERT INTO account_history (userID, action, actionDate) VALUES (@UserID, @Action, @ActionDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", databaseHelper.GetUserID(currentUser.Username));
                    command.Parameters.AddWithValue("@Action", currentUser.Username + " logged out.");
                    command.Parameters.AddWithValue("@ActionDate", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            Application.Current.Shutdown();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ItemWindow itemWindow = new ItemWindow(currentUser);
            itemWindow.Show();

            //Application.Current.Shutdown();
            //registerVoter.Activate();
            //registerVoter.Show();
        }

        private void btnAccountHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AccountWindow accountWindow = new AccountWindow(currentUser);
            accountWindow.Show();
        }

        private void btnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("In the Analytics");
            Application.Current.Shutdown();

            //registercandidate candidate = new registercandidate();
            //candidate.show();
        }

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("In the Requested item");
            Application.Current.Shutdown();
        }
    }
}