using InventoryApp.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private readonly User currentUser;
        private readonly DatabaseManager _databaseManager;

        public AccountWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            _databaseManager = new DatabaseManager();
            txtUsername.Text = currentUser.Username;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow mainWindow = new MainWindow(currentUser);
            mainWindow.Show();

        }

        private void intCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }


        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = txtOldPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (currentUser.Password != oldPassword)
            {
                MessageBox.Show("Old password is incorrect.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New password and confirm password do not match.");
                return;
            }

            currentUser.Password = newPassword;

            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "UPDATE user_account SET [password] = @NewPassword WHERE ID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewPassword", newPassword);
                    command.Parameters.AddWithValue("@UserID", currentUser.ID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Password changed successfully.");
                        txtOldPassword.Password = "";
                        txtNewPassword.Password = "";
                        txtConfirmPassword.Password = "";
                    }
                    else
                    {
                        MessageBox.Show("Failed to change password.");
                    }
                }
            }
        }

    }
}
