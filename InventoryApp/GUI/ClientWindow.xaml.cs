﻿using System;
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

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {
            RequestItem requestItem = new RequestItem(currentUser);
            requestItem.Show();
            this.Close();
        }


    }
}
