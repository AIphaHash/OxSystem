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
        public static string shname;
        public static string shstart;
        public static string shend;

        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public discount()
        {
            InitializeComponent();
            DateTime currentDate = DateTime.Now;
            int currentYear = currentDate.Year;
            int currentMonth = currentDate.Month;

            // Populate the days in the current month by default
            PopulateDaysInMonth(currentYear, currentMonth);
            LoadUserStates();
            PopulateUserCards();

        }
        public class StateHistory
        {
            public string UserId { get; set; }
            public DateTime StateDate { get; set; }
            public string State { get; set; }
        }




        private void selectedmonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedmonth.SelectedDate.HasValue)
            {
                // Get the selected date's month and year
                DateTime selectedDate = selectedmonth.SelectedDate.Value;

                // Call the method to populate days using the selected month and year
                PopulateDaysInMonth(selectedDate.Year, selectedDate.Month);
            }
        }

        private void PopulateDaysInMonth(int year, int month)
        {
            // Clear previous content in the dayGrid UniformGrid
            dayGrid.Children.Clear();

            // Query to get the full names and user IDs from the users_info table
            string query = "SELECT fullname, id FROM users_info";
            DataSet userData = conn.getData(query);

            int numberOfUsers = userData.Tables[0].Rows.Count;

            // Get the number of days in the selected month and year
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Set the number of columns in the UniformGrid based on the number of days
            dayGrid.Columns = daysInMonth;
            dayGrid.Rows = numberOfUsers + 1;

            // Add a Border for each day of the selected month in the first row
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(year, month, day);
                string dayOfWeekAbbr = date.ToString("ddd").ToUpper();

                TextBlock dayText = new TextBlock
                {
                    Text = $"{dayOfWeekAbbr} {day}",
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    FontSize = 14,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Border dayBorder = new Border
                {
                    Width = 100,
                    Height = 50,
                    Background = new SolidColorBrush(Color.FromRgb(128, 128, 128)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0),
                    Child = dayText
                };

                dayGrid.Children.Add(dayBorder);
            }

            List<StateHistory> stateHistoryList = new List<StateHistory>();

            query = $"SELECT userid, statedate, state FROM statehistroy WHERE MONTH(statedate) = {month} AND YEAR(statedate) = {year}";
            DataSet ds = conn.getData(query);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                stateHistoryList.Add(new StateHistory
                {
                    UserId = row["userid"].ToString(),
                    StateDate = Convert.ToDateTime(row["statedate"]),
                    State = row["state"].ToString()
                });
            }

            for (int userIndex = 0; userIndex < numberOfUsers; userIndex++)
            {
                string userId = userData.Tables[0].Rows[userIndex]["id"].ToString();

                for (int day = 1; day <= daysInMonth; day++)
                {
                    string displayText = "unseen";

                    var stateHistoryRecord = stateHistoryList.FirstOrDefault(sh => sh.UserId == userId && sh.StateDate.Date == new DateTime(year, month, day));

                    if (stateHistoryRecord != null)
                    {
                        displayText = stateHistoryRecord.State;
                    }

                    TextBlock userDayTextBlock = new TextBlock
                    {
                        Text = displayText,
                        Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        FontSize = 12,
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    SolidColorBrush backgroundBrush = displayText == "unseen"
                        ? new SolidColorBrush(Color.FromRgb(128, 128, 128))
                        : new SolidColorBrush(Color.FromRgb(0, 255, 0));

                    Border userDayCard = new Border
                    {
                        Width = 100,
                        Height = 50,
                        Background = backgroundBrush,
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(0),
                        Child = userDayTextBlock
                    };

                    int columnIndex = day - 1;
                    int rowIndex = userIndex + 1;

                    dayGrid.Children.Add(userDayCard);
                    Grid.SetRow(userDayCard, rowIndex);
                    Grid.SetColumn(userDayCard, columnIndex);
                }
            }

            string monthName = new DateTime(year, month, 1).ToString("MMMM");
            int firstDay = 1;
            int lastDay = DateTime.DaysInMonth(year, month);

            monthname.Content = $"{monthName} {firstDay} - {monthName} {lastDay}";
        }






        private void PopulateUserCards()
        {
            // Clear any existing cards in the UniformGrid
            usergrid.Children.Clear();

            // Query to get the full names from the users_info table
            string query = "SELECT fullname FROM users_info";
            DataSet userData = conn.getData(query);

            // Check if the DataSet contains any tables and rows
            if (userData.Tables.Count > 0 && userData.Tables[0].Rows.Count > 0)
            {
                int numberOfUsers = userData.Tables[0].Rows.Count;

                // Set the number of rows in the user grid based on user count
                usergrid.Columns = 1; // Set this to 1 for vertical stacking
                usergrid.Rows = numberOfUsers; // Set rows based on user count

                // Populate user cards
                foreach (DataRow row in userData.Tables[0].Rows)
                {
                    string fullName = row["fullname"].ToString();

                    // Create a TextBlock for the user's full name
                    TextBlock nameTextBlock = new TextBlock
                    {
                        Text = fullName,
                        Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), // White text color
                        FontSize = 14,
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    // Create a Border to act as a card
                    Border userCard = new Border
                    {
                        Width = 200, // Set your desired width
                        Height = 50, // Set your desired height
                        Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)), // Gray background
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)), // Optional: black border color
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(0, 0, 0, 0), // Adjust the margin for spacing between cards
                        Child = nameTextBlock
                    };

                    // Add the card to the user grid
                    usergrid.Children.Add(userCard);
                }

                // Optional: If PopulateDaysInMonth should be called after user cards are populated
                // DateTime selectedDate = selectedmonth.SelectedDate ?? DateTime.Now; // Get selected date or default to current date
                // PopulateDaysInMonth(selectedDate.Year, selectedDate.Month); // Call with selected year and month
            }
            else
            {
                MessageBox.Show("No user data found.", "Data Retrieval", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }









        private Border CreateUserCard(string userName, string state)
        {
            // Create a Border for the user card
            Border card = new Border
            {
                Width = 710,
                Height = 110,
                CornerRadius = new CornerRadius(10),
                Background = Brushes.White,
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };

            // Create a StackPanel to hold the text
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            // Add user name
            TextBlock userNameTextBlock = new TextBlock
            {
                Text = userName,
                FontWeight = FontWeights.Bold,
                FontSize = 16
            };

            // Add state
            TextBlock stateTextBlock = new TextBlock
            {
                Text = state,
                FontSize = 14,
                Foreground = Brushes.Gray
            };

            // Add the text blocks to the StackPanel
            stackPanel.Children.Add(userNameTextBlock);
            stackPanel.Children.Add(stateTextBlock);

            // Add the StackPanel to the card
            card.Child = stackPanel;

            return card;
        }
        private void LoadUserStates()
        {
            // Fetch user states
            string query = @"
                SELECT u.fullname, s.state 
                FROM state s 
                JOIN users_info u ON s.userid = u.id";

            DataSet ds = conn.getData(query);

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string userName = row["fullname"].ToString();
                    string state = row["state"].ToString();

                    // Create a card for each user
                    Border userCard = CreateUserCard(userName, state);

                    // Add the card to the panel
                    UserCardsPanel.Children.Add(userCard);
                }
            }
            else
            {
                // Handle the case where no user states are found
                MessageBox.Show("No user states found.");
            }
        }


        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Clicks(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Clickr(object sender, RoutedEventArgs e)
        {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard shiftOutStoryboard = (Storyboard)this.FindResource("ShiftOut");
            shiftOutStoryboard.Begin();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Storyboard shiftBackStoryboard = (Storyboard)this.FindResource("ShiftBack");
            shiftBackStoryboard.Begin();
        }

        private void shifts_Loaded(object sender, RoutedEventArgs e)
        {
            query = "SELECT DISTINCT user_name FROM users_info";
            ds = conn.getData(query);
            PopulateDropdownWithCheckBoxes();
            query = "select * from shifts";
            ds = conn.getData(query);
            DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void PermsButton_Click(object sender, RoutedEventArgs e)
        {
            permsPopup.IsOpen = !permsPopup.IsOpen;
        }

        private void mname_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void mname_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void mname_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void mname_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void mname_GotFocus(object sender, RoutedEventArgs e)
        {

        }
        private List<string> checkedItems = new List<string>();

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.IsChecked == true)
            {
                // Add the item to the list if it's checked
                checkedItems.Add(checkBox.Content.ToString());
            }
            else
            {
                // Remove the item if it's unchecked
                checkedItems.Remove(checkBox.Content.ToString());
            }

            // You can log or use the checkedItems list as needed

        }
        private void PopulateDropdownWithCheckBoxes()
        {
            StackPanel stackPanel = new StackPanel();

            // List of static values to add to the dropdown
            string[] staticRoles = { "All", "Pharm", "Admin", "Accountant" };

            // First, add the static CheckBoxes for predefined roles
            foreach (string role in staticRoles)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = role,
                    IsChecked = false
                };

                // Attach the event handler for each CheckBox
                checkBox.Click += CheckBox_Click;

                // Add the CheckBox to the StackPanel
                stackPanel.Children.Add(checkBox);
            }

            // Now, iterate through the DataSet and add dynamic CheckBoxes to the StackPanel
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string userName = row["user_name"].ToString();

                // Create a new CheckBox for each user from the DataSet
                CheckBox checkBox = new CheckBox
                {
                    Content = userName,
                    IsChecked = false
                };

                // Attach the event handler for each CheckBox
                checkBox.Click += CheckBox_Click;

                // Add the dynamic CheckBox to the StackPanel
                stackPanel.Children.Add(checkBox);
            }

            // Set the content of the Popup to the dynamically created StackPanel
            permsPopup.Child = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                Background = System.Windows.Media.Brushes.White,
                Child = stackPanel
            };
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Get shift details
            shname = supname.Text;
            shstart = supname1.Text;
            shend = supname2.Text;
            string selectedRole = String.Join(",", checkedItems);

            // Insert into shifts table
            query = "INSERT INTO shifts (shift_name, shift_start, shfit_end, shift_too) VALUES ('" + shname + "', '" + shstart + "', '" + shend + "', '" + selectedRole + "')";
            conn.setData(query);

            // Retrieve the last inserted shift ID
            query = "SELECT TOP 1 shid FROM shifts ORDER BY shid DESC;";
            ds = conn.getData(query);
            int shiftId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

            // Function to check if a record already exists in the state table
            bool IsStateEntryExists(int userId, int currentShiftId)
            {
                query = "SELECT COUNT(*) FROM state WHERE userid = " + userId + " AND shiftid = " + currentShiftId;
                DataSet result = conn.getData(query);
                return Convert.ToInt32(result.Tables[0].Rows[0][0]) > 0; // Return true if record exists
            }

            // Check if "All" is in checkedItems
            if (checkedItems.Contains("All"))
            {
                // Fetch all users from users_info
                query = "SELECT id FROM users_info";
                ds = conn.getData(query);

                // Insert into state table for each user
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int userId = Convert.ToInt32(row["id"]);

                    // Check if the entry already exists before inserting
                    if (!IsStateEntryExists(userId, shiftId))
                    {
                        query = "INSERT INTO state (userid, shiftid, state) VALUES (" + userId + ", " + shiftId + ", 'unseen')";
                        conn.setData(query);
                    }
                }
            }
            else
            {
                // Roles to check for
                var rolesToCheck = new List<string> { "Admin", "Pharm", "Accountant" };

                // Find if any of the checkedItems match the specified roles
                foreach (var role in rolesToCheck)
                {
                    if (checkedItems.Contains(role))
                    {
                        // Fetch users with the specified role from users_info
                        query = "SELECT id FROM users_info WHERE role = '" + role + "'";
                        ds = conn.getData(query);

                        // Insert into state table for each user with that role
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            int userId = Convert.ToInt32(row["id"]);

                            // Check if the entry already exists before inserting
                            if (!IsStateEntryExists(userId, shiftId))
                            {
                                query = "INSERT INTO state (userid, shiftid, state) VALUES (" + userId + ", " + shiftId + ", 'unseen')";
                                conn.setData(query);
                            }
                        }
                    }
                }

                // Handle specific usernames in checkedItems if there are any (excluding roles)
                foreach (var userName in checkedItems)
                {
                    // Skip role names since they're already processed
                    if (rolesToCheck.Contains(userName)) continue;

                    // Get user ID based on username
                    query = "SELECT id FROM users_info WHERE user_name = '" + userName + "'";
                    ds = conn.getData(query);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int userId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);

                        // Check if the entry already exists before inserting
                        if (!IsStateEntryExists(userId, shiftId))
                        {
                            query = "INSERT INTO state (userid, shiftid, state) VALUES (" + userId + ", " + shiftId + ", 'unseen')";
                            conn.setData(query);
                        }
                    }
                }
            }
        }




    }
}