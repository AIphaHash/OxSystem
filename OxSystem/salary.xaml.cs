using iText.Kernel.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        double totalShiftHours;
        double totalExpectedHours;
        int userid = int.Parse( Login_.iduser);
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
            query = " select top 1 id from users_info";
            ds = conn.getData(query);

            userid =int.Parse( ds.Tables[0].Rows[0][0].ToString());
            UpdateBorders(userid);
            if (DataGrid.Items.Count > 0)
            {
                // Select the first item
                DataGrid.SelectedIndex = 0;
            }
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

        public double GetUserShiftHours(int userId, DateTime selectedMonth)
        {
             totalShiftHours = 0;

            try
            {
                // SQL query to get the shifts and shift_too from the shifts table for the selected month
                string query = $@"
        SELECT shift.shift_hours, shift.shift_too
        FROM shifts shift
        WHERE MONTH(shift.shift_start) = {selectedMonth.Month}
        AND YEAR(shift.shift_start) = {selectedMonth.Year}";

                // Assuming conn is your database connection object and getData is a method to retrieve data
                DataSet ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    // Loop through each shift row
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string[] shiftUsers = row["shift_too"].ToString().Split(',');

                        // Loop through each username in shift_too
                        foreach (string username in shiftUsers)
                        {
                            // SQL query to get the userId from users_info table based on the username
                            string userQuery = $@"
                    SELECT id FROM users_info WHERE user_name = '{username.Trim()}'";
                            DataSet userDs = conn.getData(userQuery);

                            if (userDs != null && userDs.Tables.Count > 0 && userDs.Tables[0].Rows.Count > 0)
                            {
                                // Get the userId from users_info
                                int dbUserId = Convert.ToInt32(userDs.Tables[0].Rows[0]["id"]);

                                // Check if the current userId matches the dbUserId
                                if (dbUserId == userId)
                                {
                                    // Get the shift hours for the specific shift
                                    double shiftHours = Convert.ToDouble(row["shift_hours"]);

                                    // Add to total shift hours for the user
                                    totalShiftHours += shiftHours;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while calculating total shift hours: {ex.Message}");
            }

            return totalShiftHours;
        }


        public double GetTotalExpectedHours(int userId, DateTime selectedMonth)
        {
             totalExpectedHours = 0;

            try
            {
                // SQL query to get the shifts and shift_too from the shifts table
                string query = $@"
        SELECT shift.shift_start, shift.shfit_end, shift.shift_too
        FROM shifts shift
        WHERE MONTH(shift.shift_start) = {selectedMonth.Month}
        AND YEAR(shift.shift_start) = {selectedMonth.Year}";

                // Assuming conn is your database connection object and getData is a method to retrieve data
                DataSet ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    // Loop through each shift row
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string[] shiftUsers = row["shift_too"].ToString().Split(',');

                        // Loop through each username in shift_too
                        foreach (string username in shiftUsers)
                        {
                            // SQL query to get the userId from users_info table based on the username
                            string userQuery = $@"
                    SELECT id FROM users_info WHERE user_name = '{username.Trim()}'";
                            DataSet userDs = conn.getData(userQuery);

                            if (userDs != null && userDs.Tables.Count > 0 && userDs.Tables[0].Rows.Count > 0)
                            {
                                // Get the userId from users_info
                                int dbUserId = Convert.ToInt32(userDs.Tables[0].Rows[0]["id"]);

                                // Check if the current userId matches the dbUserId
                                if (dbUserId == userId)
                                {
                                    // Parse shift start and end times
                                    DateTime shiftStart = DateTime.Parse(row["shift_start"].ToString());
                                    DateTime shiftEnd = DateTime.Parse(row["shfit_end"].ToString());

                                    // Handle overnight shifts
                                    if (shiftEnd < shiftStart)
                                    {
                                        shiftEnd = shiftEnd.AddDays(1);
                                    }

                                    // Calculate the shift duration in hours
                                    double shiftHours = (shiftEnd - shiftStart).TotalHours;

                                    // Add to total expected hours
                                    totalExpectedHours += shiftHours;
                                }
                            }
                        }
                    }

                    // Get the number of days in the current month
                    int daysInMonth = DateTime.DaysInMonth(selectedMonth.Year, selectedMonth.Month);

                    // Calculate average daily hours worked
                    

                    // Multiply average daily hours by the number of days in the month to get total hours for the month
                    totalExpectedHours = totalExpectedHours * daysInMonth;
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while calculating total expected hours: {ex.Message}");
            }

            return totalExpectedHours;
        }







        public void AddLabelsToSelectedAccount(int userId, DateTime selectedMonth)
        {
            // Clear existing children in the Border
            Selectedaccount.Child = null;

            // Calculate worked hours, expected hours, and total expected hours
            double workedHours = CalculateWorkedHours(userId, selectedMonth);
            double totalExpectedHoursday = GetUserShiftHours(userId, selectedMonth);
            double totalExpectedHours = GetTotalExpectedHours(userId, selectedMonth);

            // Create a Grid to hold the labels
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition()); // For worked hours
            grid.RowDefinitions.Add(new RowDefinition()); // For expected hours
            grid.RowDefinitions.Add(new RowDefinition()); // For total expected hours

            // Worked Hours label
            Label workedHoursLabel = new Label
            {
                Content = $"Worked Hours: {workedHours:N2}",
                FontSize = 25,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(workedHoursLabel, 0);

            // Expected Hours label
            Label expectedHoursLabel = new Label
            {
                Content = $"Expected Hours: {totalExpectedHoursday:N2}",
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(expectedHoursLabel, 1);

            // Total Expected Hours label
            Label totalExpectedHoursLabel = new Label
            {
                Content = $"Total Expected Hours: {totalExpectedHours:N2}",
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(totalExpectedHoursLabel, 2);

            // Add labels to the grid
            grid.Children.Add(workedHoursLabel);
            grid.Children.Add(expectedHoursLabel);
            grid.Children.Add(totalExpectedHoursLabel);

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

                    // Get the full name, role, and user ID from the selected row
                    string fullName = selectedRow["fullname"].ToString();
                    string role = selectedRow["role"].ToString();
                    int userId = Convert.ToInt32(selectedRow["id"]);

                    // Create the label for full name
                    Label fullNameLabel = new Label
                    {
                        Content = fullName,
                        FontSize = 25,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    // Create the label for role
                    Label roleLabel = new Label
                    {
                        Content = role,
                        FontSize = 18,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 30, 0, 0)
                    };

                    // Add both labels to the UsernameGrid
                    UsernameGrid.Children.Add(fullNameLabel);
                    UsernameGrid.Children.Add(roleLabel);

                    // Calculate and display worked and expected hours in the SelectedAccount border
                    DateTime currentMonth = DateTime.Now;
                    AddLabelsToSelectedAccount(userId, currentMonth);

                    // Call UpdateBorders to update the bar based on selected user
                    UpdateBorders(userId); // This will update the approved and notApproved border widths
                }
            }
        }




        private void UpdateBorders(int userId)
        {
            // Fetch login history for the current day
            List<LoginHistory> loginHistoryRows = GetLoginHistoryForToday(userId);

            // Calculate total logged-in time
            TimeSpan totalLoggedInTime = CalculateLoggedInTime(loginHistoryRows);

            // Print or display the total logged-in time
            Console.WriteLine($"User has been logged in for: {totalLoggedInTime}");
           
            hourbreakdown.Content = totalLoggedInTime;
            // Calculate green and red border widths
            double totalHours = totalLoggedInTime.TotalHours;
            double greenBorderWidth = (totalHours / totalShiftHours) * 1200; // 24-hour day mapped to 1200 max width

            // Ensure widths are not negative
            double redBorderWidth = Math.Max(0, 1200 - greenBorderWidth);
            greenBorderWidth = Math.Max(0, greenBorderWidth);

            // Update the borders
            notApproved.Width = redBorderWidth;
            approved.Width = greenBorderWidth;

            // Force the UI to update
            notApproved.InvalidateVisual();
            approved.InvalidateVisual();
        }




        // Function to fetch login history for the current day
        private List<LoginHistory> GetLoginHistoryForToday(int userId)
        {
            List<LoginHistory> loginHistory = new List<LoginHistory>();

            // SQL query to get login history for the current day
            string query = $@"
                SELECT date_time, state
                FROM loginhistory
                WHERE userid = {userId}
                AND CAST(date_time AS DATE) = CAST(GETDATE() AS DATE)";

            // Use getData method to fetch results from the database
            DataSet ds = conn.getData(query);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DateTime dateTime = Convert.ToDateTime(row["date_time"]);
                    string state = row["state"].ToString();

                    loginHistory.Add(new LoginHistory
                    {
                        DateTime = dateTime,
                        State = state
                    });
                }
            }

            return loginHistory;
        }

        // Function to calculate the total logged-in time from login history rows
        private TimeSpan CalculateLoggedInTime(List<LoginHistory> loginHistoryRows)
        {
            TimeSpan totalLoggedInTime = TimeSpan.Zero;

            for (int i = 0; i < loginHistoryRows.Count; i++)
            {
                if (loginHistoryRows[i].State == "in")
                {
                    DateTime loginTime = loginHistoryRows[i].DateTime;
                    DateTime? logoutTime = null;

                    // Find the corresponding "out" state
                    for (int j = i + 1; j < loginHistoryRows.Count; j++)
                    {
                        if (loginHistoryRows[j].State == "out")
                        {
                            logoutTime = loginHistoryRows[j].DateTime;
                            break;
                        }
                    }

                    if (logoutTime.HasValue)
                    {
                        totalLoggedInTime += logoutTime.Value - loginTime;
                    }
                }
            }

            return totalLoggedInTime;
        }
    }

    // Class to represent login history records
    public class LoginHistory
    {
        public DateTime DateTime { get; set; }
        public string State { get; set; }
    }


}

