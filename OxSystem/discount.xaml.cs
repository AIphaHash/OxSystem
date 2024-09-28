using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OxSystem
{


    public partial class discount : UserControl
    {
        string idsString;
        string discountN;
        string discountS;
        string discountE;
        string discountA;
        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public discount()
        {
            InitializeComponent();
          
           

        }

        private void adddiscount_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                query = "select * from medicinfo WHERE exdate > GETDATE();";
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

        private void searchBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "🔍  Type to search ")
            {
                searchBox1.Text = "";
                searchBox1.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (searchBox1.Text == "")
            {
                searchBox1.Text = "";
                searchBox1.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void searchBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "")
            {
                searchBox1.Text = "🔍  Type to search ";
                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
            }
        }

        private async void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox1.Text == "" || searchBox1.Text == "🔍  Type to search ")
            {
                query = "select * from medicinfo";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }

           /* else if (comboBoxText == "ID")
            {
                query = "select * from medicinfo where mid like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Medic Name")
            {
                query = "select * from medicinfo where mname like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Buy Price")
            {
                query = "select * from medicinfo where bprice like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Sell Price")
            {
                query = "select * from medicinfo where email like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Expire Date")
            {
                query = "select * from medicinfo where exdate like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Manufacture Date")
            {
                query = "select * from medicinfo where madate like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Medic Num")
            {
                query = "select * from medicinfo where nummedic like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Storage Name")
            {
                query = "select * from medicinfo where sname like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }



            else
            {

                query = "select * from medicinfo where mname like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }*/
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
        List<int> idsInDataGridCopy = new List<int>();

        private void PopulateIdsList()
        {
            // Clear the list first to avoid duplicates
            idsInDataGridCopy.Clear();

            // Retrieve the DataTable from DataGrid_Copy
            DataTable copyDataTable = ((DataView)DataGrid_Copy.ItemsSource)?.Table;

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
                DataTable copyDataTable = ((DataView)DataGrid_Copy.ItemsSource)?.Table;

                // If the second DataGrid (DataGrid_Copy) does not have a data source, create a new table with the same structure as the original
                if (copyDataTable == null)
                {
                    copyDataTable = ((DataView)DataGrid.ItemsSource).Table.Clone(); // Clone structure of the original table
                }

                // Extract the ID of the selected medic (assuming the ID column is named 'id')
                int selectedMedicId = Convert.ToInt32(selectedRow["mid"]);

                // Check if the item already exists in the DataGrid_Copy
                bool existsInDataGrid = copyDataTable.AsEnumerable().Any(row =>
                    Convert.ToInt32(row["mid"]) == selectedMedicId); // Compare the medic ID

                // Check if the ID is already in the list
                bool existsInList = idsInDataGridCopy.Contains(selectedMedicId);

                // If the item does not already exist in the DataGrid_Copy and the ID is not in the list, add it
                if (!existsInDataGrid && !existsInList)
                {
                    // Add the selected row to the second DataTable (DataGrid_Copy)
                    copyDataTable.ImportRow(selectedRow.Row);

                    // Set the updated DataTable as the ItemsSource for DataGrid_Copy
                    DataGrid_Copy.ItemsSource = copyDataTable.DefaultView;

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
            discountN = discname.Text;
            discountS = discstart.Text;
            discountE = discend.Text;
            discountA = discamount.Text;    

            query = "insert into discount values('"+idsString+"', '"+discountN+"' , '"+discountS+"' , '"+discountE+"' ,'"+discountA+"')";
            conn.setData(query);

        }
    }
}