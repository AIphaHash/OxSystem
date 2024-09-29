using iText.Kernel.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OxSystem
{
   
    public partial class salary : UserControl
    {
        string idsString;
        double salamount;
        string saldate;
        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public salary()
        {
            InitializeComponent();
        }

        private void header_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                query = "select * from users_info";
                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
                }
                else
                {
                    MessageBox.Show("No data found or an error occurred.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        List<int> idsInDataGridCopy = new List<int>();

        private void PopulateIdsList()
        {
            // Clear the list first to avoid duplicates
            idsInDataGridCopy.Clear();

            // Retrieve the DataTable from DataGrid_Copy
            DataTable copyDataTable = ((DataView)DataGrid1.ItemsSource)?.Table;

            if (copyDataTable != null && copyDataTable.Rows.Count > 0)
            {
                // Loop through each row in the DataGrid_Copy
                foreach (DataRow row in copyDataTable.Rows)
                {
                    // Assuming the ID column is named "ID" and is of type int
                    int id = Convert.ToInt32(row["ID"]);  // Adjust the "ID" column name if necessary

                    // Add the ID to the list
                    idsInDataGridCopy.Add(id);
                }
            }
        }

        private void PrintIdsInList()
        {
            if (idsInDataGridCopy.Count > 0)
            {
                Console.WriteLine("IDs in DataGrid_Copy:");

                // Print each ID in the list
                foreach (int id in idsInDataGridCopy)
                {
                    Console.WriteLine(id);
                }
            }
            else
            {
                Console.WriteLine("No IDs found in DataGrid_Copy.");
            }

            // Join the int list into a single string, converting each int to a string
            idsString = string.Join(",", idsInDataGridCopy.Select(id => id.ToString()));
            Console.WriteLine($"Joined IDs: {idsString}");  // Output the final string
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if an item is selected
            if (DataGrid.SelectedItem != null)
            {
                // Get the selected row as a DataRowView
                DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;

                // Create a new DataTable to hold the copied data (or retrieve the existing one)
                DataTable copyDataTable = ((DataView)DataGrid1.ItemsSource)?.Table;

                // If the second DataGrid (DataGrid_Copy) does not have a data source, create a new table with the same structure as the original
                if (copyDataTable == null)
                {
                    copyDataTable = ((DataView)DataGrid.ItemsSource).Table.Clone(); // Clone structure of the original table
                }

                // Extract the ID of the selected medic (assuming the ID column is named 'id')
                int selectedMedicId = Convert.ToInt32(selectedRow["id"]);

                // Check if the item already exists in the DataGrid_Copy
                bool existsInDataGrid = copyDataTable.AsEnumerable().Any(row =>
                    Convert.ToInt32(row["id"]) == selectedMedicId); // Compare the medic ID

                // Check if the ID is already in the list
                bool existsInList = idsInDataGridCopy.Contains(selectedMedicId);

                // If the item does not already exist in the DataGrid_Copy and the ID is not in the list, add it
                if (!existsInDataGrid && !existsInList)
                {
                    // Add the selected row to the second DataTable (DataGrid_Copy)
                    copyDataTable.ImportRow(selectedRow.Row);

                    // Set the updated DataTable as the ItemsSource for DataGrid_Copy
                    DataGrid1.ItemsSource = copyDataTable.DefaultView;

                    // Add the selected medic's ID to the list
                    idsInDataGridCopy.Add(selectedMedicId);

                    // Print the updated list of IDs
                    PrintIdsInList();
                }
                else
                {
                    MessageBox.Show("This item already exists in the list.", "Duplicate Item", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Assuming salaryamount.Text contains the value you want to convert to decimal
            if (decimal.TryParse(salaryamount.Text, out decimal salamount))
            {
                // Format salamount to ensure it has two digits before the dot and two after
                salamount = Math.Round(salamount, 2); // Ensures two digits after the dot

                // Example: Formatting it as a string with two digits before the dot and two after
                string formattedAmount = salamount.ToString("00.00");

                // Output or further processing with salamount as a decimal
                Console.WriteLine($"Formatted Salary Amount: {formattedAmount}");
            }
            else
            {
                MessageBox.Show("Invalid salary amount entered.");
            }

            saldate = salarydate.Text;
            query = "insert into salary values('"+salamount+"', '"+idsString+"' , '" + saldate + "')";
            conn.setData(query);


        }
    }
}
