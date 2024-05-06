using InventoryApp.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

namespace InventoryApp.GUI
{
    /// <summary>
    /// Interaction logic for AnalyticsWindow.xaml
    /// </summary>
    public partial class AnalyticsWindow : Window
    {
        private readonly DatabaseManager _databaseManager;
        private readonly User currentUser;

        public AnalyticsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            _databaseManager = new DatabaseManager();
            DisplayDataAnalysis();

            string fullName = $"{currentUser.FirstName.Trim()} {currentUser.LastName.Trim()}";
            txtCurrentName.Text = fullName;
            txtCurrentEmail.Text = currentUser.Email;
            txtCurrentUsername.Text = currentUser.Username;
        }
        private void DisplayDataAnalysis()
        {
            try
            {
                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    connection.Open();

                    // 1. Total Users
                    SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM user_account", connection);
                    int totalUsers = (int)command.ExecuteScalar();
                    outputTextBlock.Text += $"Total Users: {totalUsers}\n\n";

                    // 2. User Distribution by Account Type
                    command = new SqlCommand("SELECT account_type, COUNT(*) FROM user_account GROUP BY account_type", connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int accountType = reader.GetInt32(0);
                            int count = reader.GetInt32(1);
                            string type = accountType == 1 ? "Admin" : "Normal User";
                            outputTextBlock.Text += $"{type} Users: {count}\n";
                        }
                    }
                    outputTextBlock.Text += "\n";

                    // 3. Most Active User
                    command = new SqlCommand("SELECT TOP 1 firstName, lastName FROM user_account JOIN account_history ON user_account.ID = account_history.userID GROUP BY user_account.ID, firstName, lastName ORDER BY COUNT(*) DESC", connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string firstName = reader.GetString(0).Trim();
                            string lastName = reader.GetString(1).Trim();
                            string activeUser = $"{firstName} {lastName}";
                            outputTextBlock.Text += $"Most Active User: {activeUser}\n\n";
                        }
                    }

                    // 4. Average Cost of Items
                    command = new SqlCommand("SELECT AVG(CAST(cost AS DECIMAL)) FROM item", connection);
                    decimal avgCost = Convert.ToDecimal(command.ExecuteScalar());
                    outputTextBlock.Text += $"Average Cost of Items: {avgCost:C}\n\n";

                    // 5. Request Status Distribution
                    command = new SqlCommand("SELECT [status], COUNT(*) FROM request GROUP BY [status]", connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string status = reader.GetString(0);
                            int count = reader.GetInt32(1);
                            outputTextBlock.Text += $"{status} Requests: {count}\n";
                        }
                    }
                    outputTextBlock.Text += "\n";

                    // 6. Recent Activities (Assuming there's a timestamp column in account_history)
                    command = new SqlCommand("SELECT TOP 5 action, actionDate FROM account_history ORDER BY actionDate DESC", connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        outputTextBlock.Text += "Recent Activities:\n";
                        while (reader.Read())
                        {
                            string action = reader.GetString(0);
                            DateTime actionDate = reader.GetDateTime(1);
                            outputTextBlock.Text += $"{action} - {actionDate}\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(currentUser);
            mainWindow.Show();
            this.Close();
        }
    }
}
