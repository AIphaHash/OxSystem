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
            LoadUserStates();
            CreateCards();
            CreateMonthAndDaysCards();
        }

        public void CreateMonthAndDaysCards()
        {
            // Get the current year and iterate through each month
            for (int month = 1; month <= 12; month++)
            {
                // Create a StackPanel to hold the month and its days (vertical arrangement)
                StackPanel monthStack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5)
                };

                // Create a Border for the month card
                Border monthCard = new Border
                {
                    Width = 100,
                    Height = 78,
                    Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                    Margin = new Thickness(5)
                };

                // Create a TextBlock for the month name
                TextBlock monthText = new TextBlock
                {
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                // Add the month name to the month card
                monthCard.Child = monthText;

                // Add the month card to the StackPanel
                monthStack.Children.Add(monthCard);

                // Get the number of days in the current month for the current year
                int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, month);

                // Create day cards for each day of the month
                for (int day = 1; day <= daysInMonth; day++)
                {
                    // Create a Border for each day card
                    Border dayCard = new Border
                    {
                        Width = 100,
                        Height = 30,
                        Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                        CornerRadius = new CornerRadius(3),
                        BorderThickness = new Thickness(0.5),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                        Margin = new Thickness(2)
                    };

                    // Create a TextBlock for the day number
                    TextBlock dayText = new TextBlock
                    {
                        Text = day.ToString(),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 14,
                        Foreground = new SolidColorBrush(Colors.Black)
                    };

                    // Add the day number to the day card
                    dayCard.Child = dayText;

                    // Add the day card to the StackPanel
                    monthStack.Children.Add(dayCard);
                }

                // Add the month StackPanel to the Grid (historyupbar)
                historyupbar.Children.Add(monthStack);

                // Position the monthStack horizontally in the grid
                Grid.SetColumn(monthStack, month - 1);
            }

            // Make sure the Grid has enough column definitions for each month
            EnsureGridColumns(historyupbar, 12);
        }

        // Method to ensure the grid has enough columns
        private void EnsureGridColumns(Grid grid, int columns)
        {
            grid.ColumnDefinitions.Clear();
            for (int i = 0; i < columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
            }
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

        public void CreateCards()
        {
            // Assuming you have a method to get your data from the database.
            DataTable usersInfo = GetUsersInfo(); // Method that returns DataTable with 'fullname' column

            // Set the number of rows in the grid equal to the number of fullnames
            historysidebar.RowDefinitions.Clear();

            foreach (DataRow row in usersInfo.Rows)
            {
                // Create a RowDefinition for each card (i.e., each fullname)
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(55); // Card height
                historysidebar.RowDefinitions.Add(rowDef);

                // Create the Border (card) for each fullname
                Border card = new Border
                {
                    Width = 320,
                    Height = 55,
                    Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(5), // Adds some spacing between cards
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200))
                };

                // Create the TextBlock inside the card
                TextBlock textBlock = new TextBlock
                {
                    Text = row["fullname"].ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0, 0, 0), // Padding for the text
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                // Add the TextBlock to the card
                card.Child = textBlock;

                // Place the card in the grid
                Grid.SetRow(card, historysidebar.RowDefinitions.Count - 1);
                historysidebar.Children.Add(card);
            }
        }

        // Example method to get data from the database
        private DataTable GetUsersInfo()
        {
             query = "SELECT fullname FROM users_info"; // Query to get fullnames

            ds = conn.getData(query);

            return ds.Tables[0];
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
            LoadUserStates();
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