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
    /// Interaction logic for RequestItem.xaml
    /// </summary>
    public partial class RequestItem : Window
    {
        private readonly User currentUser;
        private readonly DatabaseManager _databaseManager;
        private DatabaseHelper databaseHelper;
        public string SelectedHighlight { get; set; }
        public RequestItem(User user)
        {
            InitializeComponent();
            currentUser = user;
            _databaseManager = new DatabaseManager();
            databaseHelper = new DatabaseHelper();
            showData();
            showRequestItem();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            SearchItems(searchKeyword);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow(currentUser);
            clientWindow.Show();
            this.Close();
        }

        private void SearchItems(string searchKeyword)
        {
            // Ensure that searchKeyword is not empty
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                DataTable dt = new DataTable();
                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    string query = @"SELECT id, name, brand, highlight, status, dateAdded, cost, description 
                             FROM item
                             WHERE name COLLATE Latin1_General_CS_AS LIKE @keyword
                                OR brand COLLATE Latin1_General_CS_AS LIKE @keyword";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }

                itemsList.ItemsSource = dt.DefaultView;
            }
            else
            {
                showData();
            }
        }

        private void showData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = @"SELECT id, name, brand, highlight, status, dateAdded, cost, description FROM item";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
            }
            itemsList.ItemsSource = dt.DefaultView;
        }

        private void showRequestItem()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = @"SELECT r.ID, i.name, i.brand, i.highlight, i.status AS itemStatus, i.dateAdded, i.cost, i.description, r.status AS requestStatus
                FROM item i
                JOIN request r ON i.ID = r.itemID
                WHERE r.employeeID = @UserID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", currentUser.ID);

                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
            }
            requestedList.ItemsSource = dt.DefaultView;
        }

        private void highlightFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string? selectedHighlight = (highlightFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();

                DataTable dt = new DataTable();
                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    string query = @"SELECT id, name, brand, highlight, status, dateAdded, cost, description 
                             FROM item
                             WHERE (@selectedHighlight IS NULL OR highlight = @selectedHighlight)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@selectedHighlight", string.IsNullOrEmpty(selectedHighlight) ? DBNull.Value : (object)selectedHighlight);

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }

                itemsList.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while filtering items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortByName_Click(object sender, RoutedEventArgs e)
        {
            if (itemsList.ItemsSource is DataView dataView)
            {
                dataView.Sort = "name ASC";
            }
        }

        private void SortByDate_Click(object sender, RoutedEventArgs e)
        {
            if (itemsList.ItemsSource is DataView dataView)
            {
                dataView.Sort = "dateAdded ASC";
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            showData();
            highlightFilter.SelectedIndex = -1;
            txtSearch.Clear();
        }

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {
            if (itemsList.SelectedItem != null)
            {
                DataRowView selectedItem = (DataRowView)itemsList.SelectedItem;
                int itemID = Convert.ToInt32(selectedItem["id"]);
                string itemName = selectedItem["name"].ToString(); // Assuming the column name for the item name is "name"
                string itemColor = selectedItem["highlight"].ToString();

                // Check if the item has already been requested by the current user
                if (IsItemAlreadyRequested(itemID, currentUser.ID))
                {
                    MessageBox.Show("You have already requested this item.");
                    return;
                }

                int employeeID = currentUser.ID;
                string status = "Pending"; // Default status for a new request
                string description = $"Requesting {itemName} in {itemColor}.";
                DateTime creationDate = DateTime.Now;

                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    string query = @"INSERT INTO request (employeeID, status, description, creationDate, itemID)
                     VALUES (@EmployeeID, @Status, @Description, @CreationDate, @ItemID)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@CreationDate", creationDate);
                    command.Parameters.AddWithValue("@ItemID", itemID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Request submitted successfully.");
                            databaseHelper.LoggedAction(currentUser.ID, currentUser.Username + " request an item " + itemName);
                            showRequestItem();
                        }
                        else
                        {
                            MessageBox.Show("Failed to submit request.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item.");
            }
        }
        private bool IsItemAlreadyRequested(int itemID, int employeeID)
        {
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM request WHERE itemID = @ItemID AND employeeID = @EmployeeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemID", itemID);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                connection.Open();
                int requestCount = (int)command.ExecuteScalar();
                return requestCount > 0;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (requestedList.SelectedItem != null)
            {
                DataRowView selectedRequest = (DataRowView)requestedList.SelectedItem;
                int requestID = Convert.ToInt32(selectedRequest["id"]);

                if (MessageBox.Show("Are you sure you want to cancel this request?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DeleteRequest(requestID);
                }
            }
            else
            {
                MessageBox.Show("Please select a request to cancel.");
            }
        }

        private void DeleteRequest(int requestID)
        {
            DataRowView selectedItem = (DataRowView)requestedList.SelectedItem;
            string itemName = selectedItem["name"].ToString();

            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = "DELETE FROM request WHERE ID = @RequestID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RequestID", requestID);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Request canceled successfully.");
                        databaseHelper.LoggedAction(currentUser.ID, currentUser.Username + " cancel a request item " + itemName);
                        showRequestItem(); // Refresh the request list
                    }
                    else
                    {
                        MessageBox.Show("Failed to cancel request. No rows affected.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
