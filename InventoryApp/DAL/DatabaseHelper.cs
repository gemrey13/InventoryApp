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
    }
}
