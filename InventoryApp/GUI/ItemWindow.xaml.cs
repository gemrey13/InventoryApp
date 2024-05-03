﻿using System;
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

namespace InventoryApp.GUI
{
    /// <summary>
    /// Interaction logic for ItemWindow.xaml
    /// </summary>
    public partial class ItemWindow : Window
    {
        private readonly DatabaseManager _databaseManager;
        public ItemWindow()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager();
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
                    command.Parameters.AddWithValue("@DateAdded", DateTime.Now); // Assuming current date/time
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
                // Get the selected DataRowView from the DataGrid
                DataRowView selectedItem = (DataRowView)itemsList.SelectedItem;

                // Get the values of the item's properties from the TextBoxes and ComboBox
                int id = Convert.ToInt32(selectedItem["id"]); // Assuming 'id' is the primary key
                string itemName = txtItemName.Text;
                string brand = txtBrand.Text;
                string description = txtDescription.Text;
                int cost = int.Parse(intCost.Text);
                string status = txtStatus.Text;
                string highlight = txtHighlight.Text;

                // Construct the UPDATE query
                string query = @"UPDATE item
                         SET name = @name, brand = @brand, highlight = @highlight, status = @status, cost = @cost, description = @description
                         WHERE id = @id";

                // Execute the UPDATE query
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

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
                // Perform the search based on the searchKeyword
                DataTable dt = new DataTable();
                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    string query = @"SELECT id, name, brand, highlight, status, dateAdded, cost, description 
                             FROM item
                             WHERE name COLLATE Latin1_General_CS_AS LIKE @keyword
                                OR highlight COLLATE Latin1_General_CS_AS LIKE @keyword";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }

                // Update the DataGrid with the search results
                itemsList.ItemsSource = dt.DefaultView;
            }
            else
            {
                // If the search keyword is empty, show all items
                showData();
            }
        }




        private void intCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        // Helper method to check if a string contains only numeric characters
        private bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

    }

   
}
