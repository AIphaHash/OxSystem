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
using WpfAnimatedGif;

namespace OxSystem
{


    public partial class stateNshift : UserControl
    {
        public static string shname;
        public static string shstart;
        public static string shend;

        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public stateNshift()
        {
            InitializeComponent();
            selectedmonth.SelectedDate = DateTime.Now;
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




       

        private void PopulateDaysInMonth(int year, int month)
        {
            // Clear previous content in the dayGrid UniformGrid
            dayGrid.Children.Clear();

            // Query to get the full names and user IDs from the users_info table
            string query = "SELECT fullname, id FROM users_info";
            DataSet userData = conn.getData(query);

            int numberOfUsers = userData.Tables[0].Rows.Count;

            // Cap the rows to a maximum of 14 (1 row for days + 13 rows for users)
            int maxUserRows = 15;
            int displayedUsers = Math.Min(numberOfUsers, maxUserRows);

            // Get the number of days in the selected month and year
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Set the number of columns in the UniformGrid based on the number of days
            dayGrid.Columns = daysInMonth;
            dayGrid.Rows = displayedUsers + 1; // Add 1 for the day row

            // Add a Border for each day of the selected month in the first row
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(year, month, day);
                string dayOfWeekAbbr = date.ToString("ddd").ToUpper();

                TextBlock dayText = new TextBlock
                {
                    Text = $"{dayOfWeekAbbr} {day}",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C")),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Padding = new Thickness(20, 0, 0, 0),
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                Border dayBorder = new Border
                {
                    Width = 200,
                    Height = 50,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEEF0")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB5B5B5")),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 0, -20),
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

            for (int userIndex = 0; userIndex < displayedUsers; userIndex++)
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

                    SolidColorBrush textColor = displayText == "unseen"
                        ? new SolidColorBrush(Colors.Black)
                        : new SolidColorBrush(Colors.White);

                    TextBlock userDayTextBlock = new TextBlock
                    {
                        Text = displayText,
                        Foreground = textColor,
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Padding = new Thickness(15, 0, 0, 0)
                    };

                    SolidColorBrush backgroundBrush = displayText == "unseen"
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCBCFD6"))
                        : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF12A590"));

                    Border userDayCard = new Border
                    {
                        Width = 195,
                        Height = 65,
                        Background = backgroundBrush,
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(0, 5, 0, 0),
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

                // Limit the maximum number of user cards to 15
                int maxUsersToShow = Math.Min(numberOfUsers, 15);

                // Set the number of rows in the user grid based on user count (up to a maximum of 15)
                usergrid.Columns = 1; // Vertical stacking
                usergrid.Rows = maxUsersToShow; // Set rows based on the smaller value between the number of users and 15

                // Populate user cards
                for (int i = 0; i < maxUsersToShow; i++)
                {
                    string fullName = userData.Tables[0].Rows[i]["fullname"].ToString();

                    // Create a TextBlock for the user's full name
                    TextBlock nameTextBlock = new TextBlock
                    {
                        Text = fullName,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5F5F5F")), // Gray text color
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        Padding = new Thickness(20, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    // Create a Border to act as a card
                    Border userCard = new Border
                    {
                        Width = 300, // Set your desired width
                        Height = 70, // Set your desired height
                        Background = new SolidColorBrush(Colors.White), // White background
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB5B5B5")), // Optional: gray border color
                        BorderThickness = new Thickness(0, 2, 2, 0),
                        Margin = new Thickness(0, 0, 0, 0), // Adjust the margin for spacing between cards
                        Child = nameTextBlock
                    };

                    // Add the card to the user grid
                    usergrid.Children.Add(userCard);
                }
            }
            else
            {
            }
        }









        private Border CreateUserCard(string userName, string state, string shiftStart, string shiftEnd)
        {
            // Create a Border for the user card
            Border card = new Border
            {
                Width = 710,
                Height = 130,
                CornerRadius = new CornerRadius(0),
                Background = Brushes.White,
                Margin = new Thickness(0, 0, 0, 0),
                Padding = new Thickness(10, 5, 10, 10), // Add padding for spacing
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 0, 1.5) // Border only at the bottom
            };

            // Create a Grid to hold the GIF and the text
            Grid mainGrid = new Grid
            {
                ColumnDefinitions =
        {
            new ColumnDefinition { Width = new GridLength(130) }, // Space for the GIF image
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // Space for text and other content
        }
            };

            // Create a Grid for the GIF image
            Grid gifGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create the GIF image
            Image gifImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1414-circle-hover-pinch (3).gif")),
                Width = 120,
                Height = 120,
                Margin = new Thickness(0, 0, 10, 0) // Margin to push text away from the GIF
            };

            // Conditionally stop the GIF if the state is "unseen"
            if (state != "unseen")
            {
                ImageBehavior.SetAnimatedSource(gifImage, gifImage.Source);
                ImageBehavior.SetRepeatBehavior(gifImage, System.Windows.Media.Animation.RepeatBehavior.Forever);
            }

            // Add the GIF to the gifGrid
            gifGrid.Children.Add(gifImage);

            // Create a StackPanel for text content
            StackPanel textPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 0, 0) // Adjust margin for proper alignment
            };

            // Add user name
            TextBlock userNameTextBlock = new TextBlock
            {
                Text = userName,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 0),
                FontSize = 22
            };

            // Add state
            TextBlock stateTextBlock = new TextBlock
            {
                Text = state,
                FontSize = 18,

                Foreground = Brushes.Gray
            };

            // Add the text blocks to the text panel
            textPanel.Children.Add(userNameTextBlock);
            textPanel.Children.Add(stateTextBlock);

            // Add a colored circle to represent the state
            Ellipse stateCircle = new Ellipse
            {
                Width = 30,
                Height = 30,
                Margin = new Thickness(0, -110, 0, 0), // Adjust positioning of the circle
                HorizontalAlignment = HorizontalAlignment.Right
               
                
            };

            // Create a RadialGradientBrush for the circle fill
            RadialGradientBrush gradientBrush = new RadialGradientBrush
            {
                GradientOrigin = new Point(0.5, 0.5),
                Center = new Point(0.5, 0.5),
                RadiusX = 0.5,
                RadiusY = 0.5
            };

            // Change the circle gradient based on the state
            if (state == "unseen")
            {
                gradientBrush.GradientStops.Add(new GradientStop(Colors.Gray, 0.0));
                gradientBrush.GradientStops.Add(new GradientStop(Colors.White, 1.0));
            }
            else if (state == "upseen" || state == "active")
            {
                gradientBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.0));
                gradientBrush.GradientStops.Add(new GradientStop(Colors.White, 1.0));

                // Create a storyboard for glowing animation
                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(opacityAnimation);
                Storyboard.SetTarget(opacityAnimation, stateCircle);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
                storyboard.Begin();
            }
            else
            {
                gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
                gradientBrush.GradientStops.Add(new GradientStop(Colors.White, 1.0));
            }

            // Apply the gradient brush to the ellipse fill
            stateCircle.Fill = gradientBrush;

            // Add shift information at the bottom of the card
            TextBlock shiftTextBlock = new TextBlock
            {
                Text = $"{shiftStart} - {shiftEnd}",
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,

                Foreground = Brushes.DarkGray,
                Margin = new Thickness(400, -10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add the shift info and state circle to the text panel
            textPanel.Children.Add(shiftTextBlock);
            textPanel.Children.Add(stateCircle);

            // Place gifGrid and textPanel in the mainGrid
            mainGrid.Children.Add(gifGrid);
            Grid.SetColumn(gifGrid, 0); // Set the gifGrid in the first column
            mainGrid.Children.Add(textPanel);
            Grid.SetColumn(textPanel, 1);
         

            // Add the mainGrid to the card
            card.Child = mainGrid;

            return card;
        }





        private void LoadUserStates()
        {
            // Fetch user states including shift_start and shift_end from the shifts table
            string query = @"
    SELECT u.fullname, s.state, sh.shift_start, sh.shfit_end 
    FROM state s 
    JOIN users_info u ON s.userid = u.id 
    JOIN shifts sh ON s.shiftid = sh.shid";

            DataSet ds = conn.getData(query);

            // Clear existing user cards before loading new ones
            UserCardsPanel.Children.Clear();

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string userName = row["fullname"].ToString();
                    string state = row["state"].ToString();
                    string shiftStart = row["shift_start"].ToString();
                    string shiftEnd = row["shfit_end"].ToString();

                    // Create a card for each user with shift details
                    Border userCard = CreateUserCard(userName, state, shiftStart, shiftEnd);

                    // Add the card to the panel
                    UserCardsPanel.Children.Add(userCard);
                }
            }
            else
            {
               
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
            DateTime currentDate = DateTime.Now;

            // Parse shift start and end times
            DateTime shiftStart;
            DateTime shiftEnd;

            // Assuming the input is in a proper format (e.g., "hh:mm tt")
            if (!DateTime.TryParse(shstart, out shiftStart) || !DateTime.TryParse(shend, out shiftEnd))
            {
                MessageBox.Show("Invalid shift start or end time format.");
                return; // Exit the method if parsing fails
            }

            // Calculate total worked hours
            double totalHours = CalculateTotalHours(shiftStart, shiftEnd);

            // Insert into shifts table, including totalHours
            query = "INSERT INTO shifts (shift_name, shift_start, shfit_end, shift_too , shift_date , shift_hours) " +
                    $"VALUES ('{shname}', '{shiftStart}', '{shiftEnd}', '{selectedRole}', '{currentDate}', {totalHours})";
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

        // Method to calculate total hours from shift start and shift end
        private double CalculateTotalHours(DateTime shiftStart, DateTime shiftEnd)
        {
            double totalHours = 0;

            // Calculate the total worked hours, taking into account shifts that cross midnight
            if (shiftEnd < shiftStart)
            {
                // If the shift ends before it starts, it means it crosses midnight
                totalHours = (24 - shiftStart.TimeOfDay.TotalHours) + shiftEnd.TimeOfDay.TotalHours;
            }
            else
            {
                // Regular case
                totalHours = (shiftEnd - shiftStart).TotalHours;
            }

            return totalHours;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (search.Text == "" || search.Text == "Search Employe...")
            {
                search.Foreground = new SolidColorBrush(Colors.Black);

                search.Text = "";
            }
           
        }

        private void search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (search.Text == "" || search.Text == "Search Employe...")
            {
                search.Foreground = new SolidColorBrush(Colors.Gray);
                search.Text = "Search Employe...";
            }
           
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Assume this label will show the next grid of users after the first 15
            PopulateAdditionalUserCards();
            
        }

        private void PopulateAdditionalUserCards()
        {
            // Clear any existing children from the additional user grid (assuming you have a second grid like usergrid2 for this purpose)
            usergrid2.Children.Clear();

            // Query to get the full names from the users_info table
            string query = "SELECT fullname FROM users_info";
            DataSet userData = conn.getData(query);

            if (userData.Tables.Count > 0 && userData.Tables[0].Rows.Count > 0)
            {
                int numberOfUsers = userData.Tables[0].Rows.Count;

                // Start populating from the 16th user onward
                if (numberOfUsers > 15)
                {
                    // Calculate remaining users to show in the next grid
                    int remainingUsers = numberOfUsers - 15;

                    // Set rows in the usergrid2 based on the number of remaining users (limit to 15 again)
                    int maxUsersToShow = Math.Min(remainingUsers, 15);

                    // Set up a vertical grid for the additional users
                    usergrid2.Columns = 1;
                    usergrid2.Rows = maxUsersToShow;

                    // Populate the additional user cards
                    for (int i = 15; i < 15 + maxUsersToShow; i++)
                    {
                        string fullName = userData.Tables[0].Rows[i]["fullname"].ToString();

                        // Create the TextBlock for the user's name
                        TextBlock nameTextBlock = new TextBlock
                        {
                            Text = fullName,
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5F5F5F")), // Gray text color
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            Padding = new Thickness(20, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };

                        // Create a Border for the user card
                        Border userCard = new Border
                        {
                            Width = 300,
                            Height = 70,
                            Background = new SolidColorBrush(Colors.White),
                            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB5B5B5")),
                            BorderThickness = new Thickness(0, 2, 2, 0),
                            Margin = new Thickness(0, 0, 0, 0),
                            Child = nameTextBlock
                        };

                        // Add the card to the additional user grid
                        usergrid2.Children.Add(userCard);
                    }

                    // Make the second grid visible (if hidden)
                    usergrid2.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("All users are already shown.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No user data found.", "Data Retrieval", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            if (selectedmonth.SelectedDate.HasValue)
            {
                // Get the current selected date
                DateTime currentSelectedDate = selectedmonth.SelectedDate.Value;

                // Subtract one month
                DateTime previousMonthDate = currentSelectedDate.AddMonths(-1);

                // Set the new date to the DatePicker
                selectedmonth.SelectedDate = previousMonthDate;

                // Populate the grid with the new month
                PopulateDaysInMonth(previousMonthDate.Year, previousMonthDate.Month);
            }
        }

        // Method for the Next Month button click
        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            if (selectedmonth.SelectedDate.HasValue)
            {
                // Get the current selected date
                DateTime currentSelectedDate = selectedmonth.SelectedDate.Value;

                // Add one month
                DateTime nextMonthDate = currentSelectedDate.AddMonths(1);

                // Set the new date to the DatePicker
                selectedmonth.SelectedDate = nextMonthDate;

                // Populate the grid with the new month
                PopulateDaysInMonth(nextMonthDate.Year, nextMonthDate.Month);
            }
        }


    }
}