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
        DispatcherTimer clockTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
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
            Application.Current.Shutdown();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("In the inventory");
            Application.Current.Shutdown();

            //registerVoter.Activate();
            //registerVoter.Show();
        }

        private void btnUserManagement_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("In the User Management");
            Application.Current.Shutdown();
            //if (txtTemp.Text.Trim() == "1")
            //{
            //    this.Hide();
            //    MessageBox.Show("In the User Management");
            //    Application.Current.Shutdown();
            //    RegisterUser userWindow = new RegisterUser();
            //    userWindow.Show();
            //}
            //else
            //{
            //    txtUserManagement.Text = "Action not authorized";
            //    imgHouseKeeping.Source = new BitmapImage(new Uri(@"/images/notallowed.png", UriKind.Relative));
            //}
        }

        private void btnAccountHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("In the ACcount History");
            Application.Current.Shutdown();

            //if (txtTemp.Text.Trim() == "1")
            //{
            //    this.Hide();
            //    MessageBox.Show("In the ACcount History");
            //    Application.Current.Shutdown();
                // RegisterUser userWindow = new RegisterUser();
                // userWindow.Show();
                // this.Hide();
            //}
            //else
            //{
            //    txtVotingProper.Text = "Action not authorized";
            //    imgVotingProper.Source = new BitmapImage(new Uri(@"/images/notallowed.png", UriKind.Relative));
            //}
        }

        private void btnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("In the Analytics");
            Application.Current.Shutdown();

            //registercandidate candidate = new registercandidate();
            //candidate.show();
        }
    }
}