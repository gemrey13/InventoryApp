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
            showData();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();

            SearchItems(searchKeyword);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ClientWindow clientWindow = new ClientWindow(currentUser);
            clientWindow.Show();

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

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Request submitted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to submit request.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item.");
            }
        }
    }
}
