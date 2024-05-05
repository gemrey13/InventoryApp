using InventoryApp.DAL;
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

namespace InventoryApp.GUI
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly DatabaseManager _databaseManager;

        public RegisterWindow()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager();

        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string inputCode = pwdCode.Password;

            if (!string.IsNullOrWhiteSpace(textLastName.Text) &&
                !string.IsNullOrWhiteSpace(textFirstName.Text) &&
                !string.IsNullOrWhiteSpace(textEmail.Text) &&
                !string.IsNullOrWhiteSpace(textUsername.Text) &&
                !string.IsNullOrWhiteSpace(pwdPassword.Password))
            {
                int accountType;

                // Check if the input code is "0000"
                if (!string.IsNullOrWhiteSpace(inputCode) && inputCode == "0000")
                {
                    accountType = 1;
                }
                else if (string.IsNullOrWhiteSpace(inputCode))
                {
                    accountType = 0;
                }
                else
                {
                    MessageBox.Show("Invalid code. Registration requires a valid code.");
                    return; // Exit without performing registration
                }

                // Perform registration
                if (RegisterUser(textLastName.Text, textFirstName.Text, accountType, textEmail.Text, textUsername.Text, pwdPassword.Password))
                {
                    // Clear input fields
                    textFirstName.Clear();
                    textLastName.Clear();
                    textUsername.Clear();
                    textEmail.Clear();
                    pwdCode.Clear();
                    pwdPassword.Clear();
                    MessageBox.Show("Registered Successfully.");
                    // Show login window
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to register the account. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("Please fill in all required fields.");
            }
        }

        public bool RegisterUser(string lastName, string firstName, int accountType, string email, string username, string password)
        {
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "INSERT INTO user_account (lastName, firstName, email, account_type, username, [password]) VALUES (@LastName, @FirstName, @Email, @AccountType, @Username, @Password)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Set parameters
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@AccountType", accountType);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
