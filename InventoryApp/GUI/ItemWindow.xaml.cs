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
        }



        private bool AddItem(string itemName, string brand, string description, int cost, string status, string color, byte[] imageBytes)
        {
            try
            {
                using (SqlConnection connection = _databaseManager.GetConnection())
                {
                    string query = @"INSERT INTO item (name, image, brand, description, dateAdded, cost, status, color) 
                                    VALUES (@Name, @Image, @Brand, @Description, @DateAdded, @Cost, @Status, @Color)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", itemName);
                    command.Parameters.AddWithValue("@Image", imageBytes);
                    command.Parameters.AddWithValue("@Brand", brand);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@DateAdded", DateTime.Now); // Assuming current date/time
                    command.Parameters.AddWithValue("@Cost", cost);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Color", color);

                    connection.Open();
                    command.ExecuteNonQuery();
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
            string color = txtColor.Text;

            if (AddItem(itemName, brand, description, cost, status, color))
            {
                MessageBox.Show("Item added successfully.");
                txtItemName.Clear();
                txtBrand.Clear();
                txtDescription.Clear();
                intCost.Clear();
                txtStatus.SelectedIndex = -1;
                txtColor.SelectedIndex = -1;
            }

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void uploadImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openImageDialog = new OpenFileDialog();
            openImageDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            bool? response = openImageDialog.ShowDialog();

            if (response == true)
            {
                string filepath = openImageDialog.FileName;

                // Read the image file and convert it to a byte array
                byte[] imageBytes = File.ReadAllBytes(filepath);

                // Call the AddItem method with the image byte array
                if (AddItem(txtItemName.Text, txtBrand.Text, txtDescription.Text, int.Parse(intCost.Text), txtStatus.Text, txtColor.Text, imageBytes))
                {
                    MessageBox.Show("Item added successfully.");
                    txtItemName.Clear();
                    txtBrand.Clear();
                    txtDescription.Clear();
                    intCost.Clear();
                    txtStatus.SelectedIndex = -1;
                    txtColor.SelectedIndex = -1;
                }
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
