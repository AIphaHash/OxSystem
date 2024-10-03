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
        public double CalculateWorkedHours(int userId, DateTime selectedMonth)
        {
            double totalWorkedHours = 0;
            try
            {
                // Query to calculate the total worked hours for the selected user in the selected month
                string query = $@"SELECT 
            SUM(ABS(DATEDIFF(HOUR, shift.shift_start, shift.shfit_end))) AS totalHoursWorked
        FROM shifts shift
        INNER JOIN users_info userInfo ON CHARINDEX(userInfo.user_name, shift.shift_too) > 0
        INNER JOIN statehistroy state ON state.userid = userInfo.id
        WHERE userInfo.id = {userId}
        AND MONTH(state.statedate) = {selectedMonth.Month}
        AND YEAR(state.statedate) = {selectedMonth.Year}
        AND state.state = 'upseen'";

                // Assuming conn is your database connection object and getData is a method to retrieve data
                DataSet ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // Check if the result is DBNull (i.e., no hours calculated)
                    if (ds.Tables[0].Rows[0]["totalHoursWorked"] != DBNull.Value)
                    {
                        // Get the total worked hours from the query result
                        totalWorkedHours = Convert.ToDouble(ds.Tables[0].Rows[0]["totalHoursWorked"]);
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    // Handle case when no rows are returned by the query (e.g., no matching data found)
                    MessageBox.Show("No data found for this user in the selected month.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while calculating worked hours: {ex.Message}");
            }

            return totalWorkedHours;
        }

        public double CalculateExpectedHours(int userId, DateTime selectedMonth)
        {
            double totalExpectedHours = 0;
            try
            {
                // Query to get the total expected hours for the selected user in the selected month
                string query = $@"
        SELECT shift.shift_start, shift.shfit_end
        FROM shifts shift
        INNER JOIN statehistroy state
        ON shift.shid = state.sthid
        WHERE state.userid = {userId}
        AND MONTH(state.statedate) = {selectedMonth.Month}
        AND YEAR(state.statedate) = {selectedMonth.Year}
        AND state.state = 'upseen'";

                // Assuming conn is your database connection object and getData is a method to retrieve data
                DataSet ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        // Parse the shift start and end times
                        DateTime shiftStart = DateTime.Parse(row["shift_start"].ToString());
                        DateTime shiftEnd = DateTime.Parse(row["shfit_end"].ToString());

                        // Calculate the hours worked for this shift
                        double hoursWorked = (shiftEnd - shiftStart).TotalHours;

                        // Add the hours worked for this shift to the total
                        totalExpectedHours += hoursWorked;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while calculating expected hours: {ex.Message}");
            }

            return totalExpectedHours;
        }

        public void AddLabelsToSelectedAccount(int userId, DateTime selectedMonth)
        {
            // Clear existing children
            Selectedaccount.Child = null;

            // Calculate worked hours and expected hours
            double workedHours = CalculateWorkedHours(userId, selectedMonth);
            double expectedHours = CalculateExpectedHours(userId, selectedMonth);

            // Create a Grid to hold the labels
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition()); // For worked hours
            grid.RowDefinitions.Add(new RowDefinition()); // For expected hours

            // Create and configure the first label (worked hours)
            Label workedHoursLabel = new Label();
            workedHoursLabel.Content = $"Worked Hours: {workedHours:N2}";
            workedHoursLabel.FontSize = 25;
            workedHoursLabel.FontWeight = FontWeights.Bold;
            workedHoursLabel.HorizontalAlignment = HorizontalAlignment.Center;
            workedHoursLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(workedHoursLabel, 0);

            // Create and configure the second label (expected hours)
            Label expectedHoursLabel = new Label();
            expectedHoursLabel.Content = $"Expected Hours: {expectedHours:N2}";
            expectedHoursLabel.FontSize = 20;
            expectedHoursLabel.HorizontalAlignment = HorizontalAlignment.Center;
            expectedHoursLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(expectedHoursLabel, 1);

            // Add labels to the grid
            grid.Children.Add(workedHoursLabel);
            grid.Children.Add(expectedHoursLabel);

            // Add the grid to the Border
            Selectedaccount.Child = grid;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem != null)
            {
                // Assuming the DataGrid is bound to a DataTable and each row is a DataRowView
                DataRowView selectedRow = DataGrid.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    // Clear previous labels from UsernameGrid
                    UsernameGrid.Children.Clear();

                    // Get the full name and role from the selected row
                    string fullName = selectedRow["fullname"].ToString();
                    string role = selectedRow["role"].ToString();
                    int userId = Convert.ToInt32(selectedRow["id"]);

                    // Create the first label for full name
                    Label fullNameLabel = new Label
                    {
                        Content = fullName,
                        FontSize = 25,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    // Create the second label for role
                    Label roleLabel = new Label
                    {
                        Content = role,
                        FontSize = 18, // Smaller font size
                        FontWeight = FontWeights.Normal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 30, 0, 0) // To position it below the first label
                    };

                    // Add both labels to the UsernameGrid
                    UsernameGrid.Children.Add(fullNameLabel);
                    UsernameGrid.Children.Add(roleLabel);

                    // Calculate and display worked and expected hours in the Selectedaccount border
                    DateTime currentMonth = DateTime.Now;
                    AddLabelsToSelectedAccount(userId, currentMonth);
                }
            }
        }


    }
}
