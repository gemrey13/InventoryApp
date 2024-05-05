using System;
using Microsoft.Win32;
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
using InventoryApp.DAL;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.Globalization;
using System.ComponentModel;

namespace InventoryApp.GUI
{
    /// <summary>
    /// Interaction logic for ItemWindow.xaml
    /// </summary>
    public partial class ItemWindow : Window
    {
        private readonly User currentUser;

        private readonly DatabaseManager _databaseManager;

        private DatabaseHelper databaseHelper;

        public string SelectedHighlight { get; set; }

        public ItemWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            _databaseManager = new DatabaseManager();
            databaseHelper = new DatabaseHelper();
            showData();
        }



        private bool AddItem(string itemName, string brand, string description, int cost, string status, string highlight)
        {
            try
            {
                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    string query = @"INSERT INTO item (name, brand, description, dateAdded, cost, status, highlight) 
                                    VALUES (@Name, @Brand, @Description, @DateAdded, @Cost, @Status, @Highlight)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", itemName);
                    command.Parameters.AddWithValue("@Brand", brand);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@DateAdded", DateTime.Now); 
                    command.Parameters.AddWithValue("@Cost", cost);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Highlight", highlight);

                    connection.Open();
                    command.ExecuteNonQuery();
                    showData();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item: " + ex.Message);
                return false;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            string itemName = txtItemName.Text;
            string brand = txtBrand.Text;
            string description = txtDescription.Text;
            int cost = int.Parse(intCost.Text);
            string status = txtStatus.Text;
            string highlight = txtHighlight.Text;

            if (AddItem(itemName, brand, description, cost, status, highlight))
            {
                MessageBox.Show("Item added successfully.");
                txtItemName.Clear();
                txtBrand.Clear();
                txtDescription.Clear();
                intCost.Clear();
                txtStatus.SelectedIndex = -1;
                txtHighlight.SelectedIndex = -1;

                databaseHelper.LoggedAction(currentUser.ID, currentUser.Username + " added an item " + itemName);
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (itemsList.SelectedItem != null)
            {
                DataRowView selectedItem = (DataRowView)itemsList.SelectedItem;

                int id = Convert.ToInt32(selectedItem["id"]); 
                string itemName = txtItemName.Text;
                string brand = txtBrand.Text;
                string description = txtDescription.Text;
                int cost = int.Parse(intCost.Text);
                string status = txtStatus.Text;
                string highlight = txtHighlight.Text;

                string query = @"UPDATE item
                         SET name = @name, brand = @brand, highlight = @highlight, status = @status, cost = @cost, description = @description
                         WHERE id = @id";

                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", itemName);
                    command.Parameters.AddWithValue("@brand", brand);
                    command.Parameters.AddWithValue("@highlight", highlight);
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@cost", cost);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item updated successfully.");
                        showData();
                        databaseHelper.LoggedAction(currentUser.ID, currentUser.Username + " updated an item " + itemName);

                    }
                    else
                    {
                        MessageBox.Show("Failed to update item.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to update.");
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (itemsList.SelectedItem != null)
            {
                DataRowView selectedItem = (DataRowView)itemsList.SelectedItem;

                int id = Convert.ToInt32(selectedItem["id"]);
                string itemName = txtItemName.Text;


                string query = @"DELETE FROM item WHERE id = @id";

                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item deleted successfully.");
                        showData();
                        databaseHelper.LoggedAction(currentUser.ID, currentUser.Username + " deleted an item " + itemName);

                    }
                    else
                    {
                        MessageBox.Show("Failed to delete item.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow mainWindow = new MainWindow(currentUser);
            mainWindow.Show();

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();

            SearchItems(searchKeyword);
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


        private void intCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            showData();
            highlightFilter.SelectedIndex = -1;
            txtSearch.Clear();
        }
    }
}
