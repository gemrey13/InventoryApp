using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace InventoryApp.GUI
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private readonly User currentUser;

        DispatcherTimer clockTimer = new DispatcherTimer();
        public ClientWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            txtTitle.Text = "HI! " + currentUser.FirstName;

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
            ItemWindow itemWindow = new ItemWindow(currentUser);
            itemWindow.Show();
        }

        private void btnAccountHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AccountWindow accountWindow = new AccountWindow(currentUser);
            accountWindow.Show();
        }

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("In the Requested item");
            Application.Current.Shutdown();
        }


    }
}
