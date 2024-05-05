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
    /// Interaction logic for StatusItem.xaml
    /// </summary>
    public partial class StatusItem : Window
    {
        private readonly User currentUser;
        private readonly DatabaseManager _databaseManager;

        public StatusItem(User user)
        {
            InitializeComponent();
            currentUser = user;
            _databaseManager = new DatabaseManager();
            showRequestItem();
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
                    string query = @"SELECT r.ID, i.name, i.brand, i.highlight, i.status AS itemStatus, i.dateAdded, i.cost, i.description, r.status AS requestStatus
                             FROM item i
                             JOIN request r ON i.ID = r.itemID
                             WHERE (i.name COLLATE Latin1_General_CS_AS LIKE @keyword
                                    OR i.brand COLLATE Latin1_General_CS_AS LIKE @keyword)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }

                requestedList.ItemsSource = dt.DefaultView;
            }
            else
            {
                showRequestItem();
            }
        }
        private void showRequestItem()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = _databaseManager.GetConnection())
            {
                string query = @"SELECT r.ID, i.name, i.brand, i.highlight, i.status AS itemStatus, i.dateAdded, i.cost, i.description, r.status AS requestStatus
                FROM item i
                JOIN request r ON i.ID = r.itemID";
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
                    string query = @"SELECT r.ID, i.name, i.brand, i.highlight, i.status AS itemStatus, i.dateAdded, i.cost, i.description, r.status AS requestStatus
                FROM item i
                JOIN request r ON i.ID = r.itemID
                             WHERE (@selectedHighlight IS NULL OR highlight = @selectedHighlight)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@selectedHighlight", string.IsNullOrEmpty(selectedHighlight) ? DBNull.Value : (object)selectedHighlight);

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }

                requestedList.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while filtering items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortByName_Click(object sender, RoutedEventArgs e)
        {
            if (requestedList.ItemsSource is DataView dataView)
            {
                dataView.Sort = "name ASC";
            }
        }

        private void SortByDate_Click(object sender, RoutedEventArgs e)
        {
            if (requestedList.ItemsSource is DataView dataView)
            {
                dataView.Sort = "dateAdded ASC";
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            showRequestItem();
            highlightFilter.SelectedIndex = -1;
            txtSearch.Clear();
        }

  

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
