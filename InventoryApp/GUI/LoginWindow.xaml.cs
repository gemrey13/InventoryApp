using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using InventoryApp.DAL;

namespace InventoryApp.GUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        MainWindow mainWindow = new MainWindow();

        private readonly DatabaseManager _databaseManager;
        public LoginWindow()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = textUsername.Text;
            string password = pwdPassword.Password;

            if (AuthenticateUser(username, password))
            {
                this.Close();

                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM [user_account] WHERE [username] = @Username AND [password] = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}
