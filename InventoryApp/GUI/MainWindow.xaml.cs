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
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            ItemWindow itemWindow = new ItemWindow(currentUser);
            itemWindow.Show();
            this.Close();
        }

        private void btnAccountHistory_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow accountWindow = new AccountWindow(currentUser);
            accountWindow.Show();
            this.Close();
        }

        private void btnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("In the Analytics");
            Application.Current.Shutdown();
            this.Close();
        }

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {
            StatusItem statusItem = new StatusItem(currentUser);
            statusItem.Show();
            this.Close();
        }
    }
}