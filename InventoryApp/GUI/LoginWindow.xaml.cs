using InventoryApp.DAL;
using System;
using System.Data.SqlClient;
using System.Windows;

namespace InventoryApp.GUI
{
    public partial class LoginWindow : Window
    {
        private MainWindow mainWindow;
        private readonly DatabaseManager _databaseManager;
        private DatabaseHelper databaseHelper;

        public LoginWindow()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager();
            databaseHelper = new DatabaseHelper();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = textUsername.Text;
            string password = pwdPassword.Password;

            if (AuthenticateUser(username, password, out User currentUser))
            {
                InsertLoginHistory(currentUser.Username);
                mainWindow = new MainWindow(currentUser);
                this.Close();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        private bool AuthenticateUser(string username, string password, out User user)
        {
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "SELECT ID, lastName, firstName, email, account_type FROM user_account WHERE username = @Username AND [password] = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new User
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            LastName = reader["lastName"].ToString(),
                            FirstName = reader["firstName"].ToString(),
                            Email = reader["email"].ToString(),
                            AccountType = Convert.ToInt32(reader["account_type"]),
                            Username = username
                        };
                        reader.Close();
                        return true;
                    }
                }
            }
            user = null;
            return false;
        }
        private void InsertLoginHistory(string username)
        {
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "INSERT INTO account_history (userID, action, actionDate) VALUES (@UserID, @Action, @ActionDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", databaseHelper.GetUserID(username));
                    command.Parameters.AddWithValue("@Action", username + " logged in.");
                    command.Parameters.AddWithValue("@ActionDate", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }

    public class User
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public int AccountType { get; set; }
        public string Username { get; set; }
    }
}
