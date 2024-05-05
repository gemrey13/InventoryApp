using System;
using System.Data.SqlClient;

namespace InventoryApp.DAL
{
    public class DatabaseHelper
    {
        private readonly DatabaseManager _databaseManager;

        public DatabaseHelper()
        {
            _databaseManager = new DatabaseManager();
        }

        public int GetUserID(string username)
        {
            int userID = -1; // Default value if user is not found

            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "SELECT ID FROM user_account WHERE username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        userID = Convert.ToInt32(result);
                    }
                }
            }

            return userID;
        }


        public void LoggedAction(int userID, string action) 
        {
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "INSERT INTO account_history (userID, action, actionDate) VALUES (@UserID, @Action, @ActionDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@Action", action);
                    command.Parameters.AddWithValue("@ActionDate", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
