using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAnimatedGif;
namespace OxSystem
{

    public partial class pharmacist : Window
    {
        private DispatcherTimer chatReloadTimer;
        private bool isTextboxFocused = false;
        public static string currencyies;
        private decimal defaultCurrency = 1400m;
        public static string check_currency = "IQD";
        public string pharmperm;
        public static string fulln;
        public string CurrentUserId = Login_.iduser;
        private string selectedUserId = null;
        private string selectedUserName = null;
        string Name_;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;

        public pharmacist()
        {
            InitializeComponent();
            LV.SelectedIndex = 0;
            //Name_ = Name;
            Pname.Text = Name_;
            chatReloadTimer = new DispatcherTimer();
            chatReloadTimer.Interval = TimeSpan.FromSeconds(3);
            chatReloadTimer.Tick += ChatReloadTimer_Tick;
        }

        // Timer Tick Event to reload chat history
        private void ChatReloadTimer_Tick(object sender, EventArgs e)
        {
            if (!isTextboxFocused && !string.IsNullOrEmpty(selectedUserId))
            {
                LoadChatHistory(selectedUserId, selectedUserName);
            }
        }
        public void LoginUser(string userId)
        {
            CurrentUserId = userId;

            // Load user-specific data or transition to the main window
        }
        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
           
        }

        

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard2"];

            // Set the target of the animation to the image
            Storyboard.SetTarget(fadeInStoryboard, lgog);

            // Begin the animation
            fadeInStoryboard.Begin();
            op.Visibility = Visibility.Collapsed;
            medic_warning1.Opacity = 1;
            medic_num1.Opacity = 1;
            main_background.Opacity = 1;
            medic_add.Opacity = 1;
            noteficatinoP.Opacity = 1;
            storageadd.Opacity = 1;
            sellmedic.Opacity=1;
            outmedic.Opacity = 1;

        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard2"];

            // Set the target of the animation to the image
            Storyboard.SetTarget(fadeOutStoryboard, lgog);

            // Begin the animation
            fadeOutStoryboard.Begin();
            op.Visibility = Visibility.Visible;
            medic_warning1.Opacity = 0.3;
            medic_num1.Opacity = 0.3;
            main_background.Opacity = 0.3;
            medic_add.Opacity = 0.3;
            noteficatinoP.Opacity = 0.3;
            storageadd.Opacity = 0.3;
            sellmedic.Opacity = 0.3;
            outmedic.Opacity = 0.3;

        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
        }
        

        private void nav_pnl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn_Checked(sender, e);
            if (nav_pnl.Width != 240)
            {
                Storyboard showStoryboard = (Storyboard)FindResource("ShowStackPanel");

                // Begin the storyboard
                showStoryboard.Begin(this);
            }
        }



        public void SetActiveUserControl(UserControl newControl)
        {
            // Find currently visible control
            var currentVisibleControl = BG.Children.OfType<UserControl>().FirstOrDefault(c => c.Visibility == Visibility.Visible);

            // Check if the new control is the same as the currently visible control
            if (currentVisibleControl == newControl)
            {
                // If the control is already visible, do nothing and return
                return;
            }

            if (currentVisibleControl != null)
            {
                // Prepare storyboard for dragging out
                var dragOutStoryboard = (Storyboard)this.Resources["DragOutStoryboard"];

                // Update storyboard to target current visible control
                foreach (var animation in dragOutStoryboard.Children.OfType<ThicknessAnimation>())
                {
                    Storyboard.SetTarget(animation, currentVisibleControl);
                }
                foreach (var animation in dragOutStoryboard.Children.OfType<DoubleAnimation>())
                {
                    Storyboard.SetTarget(animation, currentVisibleControl);
                }

                // Detach any existing event handlers to avoid multiple triggers
                dragOutStoryboard.Completed -= DragOutStoryboard_Completed;

                // Attach a new event handler for completion
                dragOutStoryboard.Completed += DragOutStoryboard_Completed;

                // Local method to handle the completion of the drag-out animation
                void DragOutStoryboard_Completed(object sender, EventArgs e)
                {
                    dragOutStoryboard.Completed -= DragOutStoryboard_Completed;

                    // Hide the current control
                    currentVisibleControl.Visibility = Visibility.Collapsed;

                    // Show the new control
                    newControl.Visibility = Visibility.Visible;

                    // Prepare storyboard for dragging in
                    var dragInStoryboard = (Storyboard)this.Resources["DragInStoryboard"];

                    // Update storyboard to target new control
                    foreach (var animation in dragInStoryboard.Children.OfType<ThicknessAnimation>())
                    {
                        Storyboard.SetTarget(animation, newControl);
                    }
                    foreach (var animation in dragInStoryboard.Children.OfType<DoubleAnimation>())
                    {
                        Storyboard.SetTarget(animation, newControl);
                    }

                    // Start the drag-in animation
                    dragInStoryboard.Begin();
                }

                // Start the drag-out animation
                dragOutStoryboard.Begin();
            }
            else
            {
                // No control is visible, so directly show the new one
                newControl.Visibility = Visibility.Visible;

                // Prepare storyboard for dragging in
                var dragInStoryboard = (Storyboard)this.Resources["DragInStoryboard"];

                // Update storyboard to target new control
                foreach (var animation in dragInStoryboard.Children.OfType<ThicknessAnimation>())
                {
                    Storyboard.SetTarget(animation, newControl);
                }
                foreach (var animation in dragInStoryboard.Children.OfType<DoubleAnimation>())
                {
                    Storyboard.SetTarget(animation, newControl);
                }

                // Start the drag-in animation
                dragInStoryboard.Begin();
            }
        }






        private void ListViewItem_Selected_1(object sender, RoutedEventArgs e)
        {
            query = "UPDATE state\r\nSET state.state = 'unseen'\r\nFROM state\r\nINNER JOIN users_info ON state.userid = users_info.id\r\nWHERE users_info.id = '" + Login_.iduser + "' ";
            conn.setData(query);
            Login_ l1 = new Login_();
            l1.Show();
            this.Close();
        }

   
       

        private void ListViewItem_Selected_3(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(storageadd);

            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
            
        }

        public void background_Loaded(object sender, RoutedEventArgs e)
        {

            
            l1.Content =  Properties.Settings.Default.ex2 + "Days or less to Expire :";
            l2.Content = Properties.Settings.Default.ex3 + "Days or less to Expire :";
            l3.Content = Properties.Settings.Default.ex4 + "Days or less to Expire :";
            l4.Content = Properties.Settings.Default.st2 + "or less to run out :";
            l5.Content = Properties.Settings.Default.st3 + "or less to run out :";
            l6.Content = Properties.Settings.Default.st4 + "or less to run out :";




            query = "select perms from users_info where id = '" + CurrentUserId + "'";
            ds = conn.getData(query);
            pharmperm = ds.Tables[0].Rows[0][0].ToString();

            showreport.mybutton.IsEnabled = false;
            Console.WriteLine(pharmperm);
            string[] permissions = pharmperm.Split(',');

            foreach (string permission in permissions)
            {
                if (permission == "viewmedic")
                {
                    view.IsEnabled = true;
                }
                else if (permission == "addmedic")
                {
                    buy.IsEnabled = true;
                }
                else if (permission == "storagemanage")
                {
                    storage.IsEnabled = true;
                }
                else if (permission == "sellmedic")
                {
                    sell.IsEnabled = true;
                }
                else if (permission == "makereport")
                {
                    showreport.mybutton.IsEnabled = true;
                }
                else if (permission == "allpermmisions")
                {
                    sell.IsEnabled = true;
                    storage.IsEnabled = true;
                    buy.IsEnabled = true;
                    view.IsEnabled = true;

                }
               

            }


            query = "SELECT *, DATEDIFF(day, GETDATE(), exdate) AS DaysLeft FROM medicinfo WHERE exdate IS NOT NULL;";
            ds = conn.getData(query);

            // Variables to hold counts for different expiration periods
            int alreadyExpired = 0;
            int lessThan3Days = 0;
            int lessThan1Week = 0;
            int lessThan1Month = 0;

            // Iterate through the results and count the items based on the DaysLeft
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                // Check if exdate is null before calculating DaysLeft
                if (row["DaysLeft"] != DBNull.Value)
                {
                    int daysLeft = Convert.ToInt32(row["DaysLeft"]);

                    if (daysLeft < int.Parse( Properties.Settings.Default.ex1)) // Already expired
                    {
                        alreadyExpired++;
                    }
                     if (daysLeft <= int.Parse(Properties.Settings.Default.ex2)) // 3 days or less left
                    {
                        lessThan3Days++;
                    }
                     if (daysLeft <= int.Parse(Properties.Settings.Default.ex3)) // 1 week or less left
                    {
                        lessThan1Week++;
                    }
                     if (daysLeft <= int.Parse(Properties.Settings.Default.ex4)) // 1 month or less left
                    {
                        lessThan1Month++;
                    }
                }
            }

            // Check if there are any results for each period
            bool hasExpiredMeds = alreadyExpired > 0 || lessThan3Days > 0 || lessThan1Week > 0 || lessThan1Month > 0;

            if (hasExpiredMeds)
            {
                // Update UI elements accordingly
                if (alreadyExpired > 0)
                {
                    expireNum.Content = alreadyExpired.ToString();
                }
                if (lessThan3Days > 0)
                {
                    expire3.Content = lessThan3Days.ToString();
                }
                if (lessThan1Week > 0)
                {
                    expire1.Content = lessThan1Week.ToString();
                }
                if (lessThan1Month > 0)
                {
                    expire1m.Content = lessThan1Month.ToString();
                }

                // Set visibility for the UI elements only once
                none.Visibility = Visibility.Collapsed;
                numOFmexpire.Visibility = Visibility.Visible;
            }
            else
            {
                // No expired medications within the given periods
                numOFmexpire.Visibility = Visibility.Collapsed;
                if (numOFmout.Visibility == Visibility.Collapsed)
                {
                    none.Visibility = Visibility.Visible;
                }
            }




            query = "SELECT nummedic FROM medicinfo WHERE nummedic IS NOT NULL;";
            ds = conn.getData(query);

            // Variables to hold counts for different quantity levels
            int outOfStockQty = 0;
            int fiveOrLessQty = 0;
            int tenOrLessQty = 0;
            int fiftenorlessqty = 0;

            // Iterate through the results and count the items based on the quantity
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["nummedic"] != DBNull.Value)
                {
                    int qty = Convert.ToInt32(row["nummedic"]);

                    if (qty == int.Parse(Properties.Settings.Default.st1)) // Already run out
                    {
                        outOfStockQty++;
                    }
                     if (qty <= int.Parse(Properties.Settings.Default.st2)) // 5 or fewer units left
                    {
                        fiveOrLessQty++;
                    }
                     if (qty <= int.Parse(Properties.Settings.Default.st3)) // 10 or fewer units left
                    {
                        tenOrLessQty++;
                    }
                    if (qty <= int.Parse(Properties.Settings.Default.st4)) // 10 or fewer units left
                    {
                        fiftenorlessqty++;
                    }
                }
            }

            // Check if there are any results for each quantity level
            bool hasLowQtyMeds = outOfStockQty > 0 || fiveOrLessQty > 0 || tenOrLessQty > 0 || fiftenorlessqty > 0;

            if (hasLowQtyMeds)
            {
                // Update UI elements accordingly
                if (outOfStockQty > 0)
                {
                    outNum.Content = outOfStockQty.ToString();
                    Console.WriteLine(outOfStockQty);
                }
                if (fiveOrLessQty > 0)
                {
                    outNum5.Content = fiveOrLessQty.ToString();
                }
                if (tenOrLessQty > 0)
                {
                    outNum10.Content = tenOrLessQty.ToString();
                }
                if (fiftenorlessqty > 0)
                {
                    outNum15.Content = fiftenorlessqty.ToString();
                }

                // Set visibility for the UI elements only once
                none.Visibility = Visibility.Collapsed;
                numOFmout.Visibility = Visibility.Visible;
            }
            else
            {
                // No low quantity medications within the given thresholds
                numOFmout.Visibility = Visibility.Collapsed;
                if (numOFmexpire.Visibility == Visibility.Collapsed)
                {
                    none.Visibility = Visibility.Visible;
                }
            }




            SetActiveUserControl(main_background);
            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }




        }

       

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(main_background);

            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
            
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(medic_add);
            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }

          
            
        }

        private void numOFm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(noteficatinoP);
            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
            
        
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(noteficatinoP);

            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            query = "select * from medicinfo WHERE exdate <= GETDATE();";
            ds = conn.getData(query);
            if (ds.Tables[0].Rows.Count != 0)
            {
                none.Visibility = Visibility.Collapsed;
                numOFmexpire.Visibility = Visibility.Visible;
                numOFmout.Visibility = Visibility.Visible;
            }
            else
            {
                numOFmout.Visibility = Visibility.Collapsed;
                none.Visibility = Visibility.Visible;
                numOFmexpire.Visibility = Visibility.Collapsed;
            }
        }

        private void reset_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ListViewItem_Selecteds(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(sellmedic);
            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
           
                   }


        private void RemoveAllUserControls()
        {
            foreach (UIElement child in BG.Children.OfType<UserControl>().ToList())
            {
                BG.Children.Remove(child);
            }

            // Force garbage collection to free up resources
            ForceGarbageCollection();
        }

        private void ForceGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void numOFm_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(outmedic);
            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
            
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            query = "select fullname,address from users_info where id = '"+Login_.iduser+"'";
            ds = conn.getData(query);
            Pname.Text = ds.Tables[0].Rows[0][0].ToString();
            place.Text = ds.Tables[0].Rows[0][1].ToString();
            fulln = ds.Tables[0].Rows[0][0].ToString();
            ChatStackPanel.Children.Clear();

            // Query to get admin users, excluding the logged-in user and including their role
            query = $"SELECT id, fullname, role FROM users_info WHERE id <> '{CurrentUserId}'";
            ds = new DbConnection().getData(query);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string userId = row["id"].ToString();
                string fullName = row["fullname"].ToString();
                string role = row["role"].ToString();

                // Query to get the last message for each user
                string lastMessageQuery = $"SELECT TOP 1 message FROM UserMessages WHERE sender_id = '{userId}' ORDER BY timestamp DESC";
                DataSet lastMessageDs = new DbConnection().getData(lastMessageQuery);
                string lastMessage = lastMessageDs.Tables[0].Rows.Count > 0 ? lastMessageDs.Tables[0].Rows[0]["message"].ToString() : "No messages yet";

                // Create a StackPanel to hold the images and text
                StackPanel cardContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                // Create the outer Image element (circle image)
                Image outerImage = new Image
                {
                    Source = new BitmapImage(new Uri("images/1414.png", UriKind.Relative)),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(0, 0, 0, 0) // Margin between image and text
                };

                // Create the inner Image element (role-specific image)
                Image innerImage = new Image
                {
                    Width = 50, // Adjust as needed
                    Height = 50, // Adjust as needed
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Set the source of the inner image based on the role
                switch (role)
                {
                    case "Accountent":
                        innerImage.Source = new BitmapImage(new Uri("images/6348754.png", UriKind.Relative));
                        break;
                    case "Pharm":
                        innerImage.Source = new BitmapImage(new Uri("images/6938244.png", UriKind.Relative));
                        break;
                    case "Admin":
                        innerImage.Source = new BitmapImage(new Uri("images/pngtree-administrator-line-icon-png-image_9064932.png", UriKind.Relative));
                        break;
                }

                // Create a Grid to overlay the images
                Grid imageGrid = new Grid();
                imageGrid.Children.Add(outerImage);
                imageGrid.Children.Add(innerImage);

                // Create the TextBlock for the user's full name and last message
                TextBlock buttonContent = new TextBlock
                {
                    Margin = new Thickness(10, 20, 0, 0) // Adjust the top margin here
                };

                buttonContent.Inlines.Add(new Run(fullName)
                {
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                });

                buttonContent.Inlines.Add(new LineBreak());

                buttonContent.Inlines.Add(new Run($"Last message: {lastMessage}")
                {
                    FontSize = 12
                });

                // Add the image grid and text to the StackPanel
                cardContent.Children.Add(imageGrid);
                cardContent.Children.Add(buttonContent);

                // Create the Button for each user
                Button cardButton = new Button
                {
                    Content = cardContent,
                    Tag = userId,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    Background = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    BorderBrush = null,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Width = 400
                };

                cardButton.Click += CardButton_Click;

                ChatStackPanel.Children.Add(cardButton);
            }

            // Start the glow animation
            var glowAnimation = (Storyboard)Resources["GlowAnimation"];
            /*glowAnimation.Begin(glowingBorder1, true);
            glowAnimation.Begin(glowingBorder2, true);
            glowAnimation.Begin(glowingBorder3, true);*/
        }
        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            selectedUserId = clickedButton.Tag.ToString(); // Store the selected user ID

            // Extract the TextBlock from the Button's Content
            TextBlock buttonContent = clickedButton.Content as TextBlock;
            if (buttonContent != null)
            {
                Run nameRun = buttonContent.Inlines.OfType<Run>().FirstOrDefault();
                if (nameRun != null)
                {
                    selectedUserName = nameRun.Text; // Store the selected user name
                }
            }

            // Load chat history for the selected user
            LoadChatHistory(selectedUserId, selectedUserName);

            // Start the timer to reload chat history every 3 seconds
            chatReloadTimer.Start();

            // Show the chat panel
            ChatPanel.Visibility = Visibility.Visible;
        }


        private void LoadChatHistory(string userId, string userName)
        {
            // Clear existing chat display
            ChatDisplay.Children.Clear();

            // Query to get chat history
            string query = $"SELECT sender_id, message, timestamp FROM UserMessages WHERE (sender_id = '{userId}' AND receiver_id = '{CurrentUserId}') OR (sender_id = '{CurrentUserId}' AND receiver_id = '{userId}') ORDER BY timestamp";
            DataSet ds = new DbConnection().getData(query);

            DateTime? lastDate = null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string senderId = row["sender_id"].ToString();
                string message = row["message"].ToString();
                DateTime timestamp = Convert.ToDateTime(row["timestamp"]);

                // Check if the date has changed
                if (lastDate == null || lastDate.Value.Date != timestamp.Date)
                {
                    // Create a TextBlock for the date separator
                    TextBlock dateSeparator = new TextBlock
                    {
                        Text = timestamp.ToString("yyyy-MM-dd"),
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.Gray),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 10, 0, 10)
                    };

                    // Add the date separator to the ChatDisplay
                    ChatDisplay.Children.Add(dateSeparator);

                    // Update the lastDate
                    lastDate = timestamp.Date;
                }

                // Create a Border for the message
                Border messageBorder = new Border
                {
                    Background = senderId == CurrentUserId
                        ? new SolidColorBrush(Color.FromRgb(200, 230, 255))  // Background color for messages sent by the current user
                        : new SolidColorBrush(Color.FromRgb(220, 220, 220)), // Background color for messages received (grey)
                    CornerRadius = new CornerRadius(10), // Corner radius for the message background
                    Padding = new Thickness(10),
                    Margin = new Thickness(5),
                    HorizontalAlignment = senderId == CurrentUserId ? HorizontalAlignment.Right : HorizontalAlignment.Left // Align based on sender
                };

                // Create a StackPanel to hold the message and timestamp
                StackPanel messagePanel = new StackPanel();

                // Create a TextBlock for the message
                TextBlock messageBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = message
                };

                // Create a TextBlock for the timestamp
                TextBlock timestampBlock = new TextBlock
                {
                    Text = timestamp.ToString("HH:mm"),
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(0, 5, 0, 0) // Add a little margin on top
                };

                // Add the message and timestamp to the StackPanel
                messagePanel.Children.Add(messageBlock);
                messagePanel.Children.Add(timestampBlock);

                // Add the StackPanel to the Border
                messageBorder.Child = messagePanel;

                // Add the Border to the StackPanel (ChatDisplay)
                ChatDisplay.Children.Add(messageBorder);
            }

            // Update selectedUserName
            query = "select fullname from users_info where id = '" + selectedUserId + "'";
            ds = conn.getData(query);
            selectedUserName = ds.Tables[0].Rows[0][0].ToString();

            // Update chat title
            ChatTitle.Text = $"Chat with {selectedUserName}";
        }







        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text;
            string receiverId = selectedUserId;

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(receiverId))
            {
                // Insert new message into the database
                string query = $"INSERT INTO UserMessages (sender_id, receiver_id, message, timestamp) VALUES ('{CurrentUserId}', '{receiverId}', '{message}', GETDATE())";
                new DbConnection().setData(query);

                // Clear input and update chat display
                MessageInput.Clear();
                LoadChatHistory(receiverId, selectedUserName);
            }
        }

        private string GetSelectedUserId()
        {
            // Logic to get the ID of the selected user
            return ""; // Placeholder
        }

        private string GetSelectedUserName()
        {
            // Logic to get the name of the selected user
            return ""; // Placeholder
        }







        private void ToggleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Storyboard storyboard;
            if (ExpandableBorder.Height == 350)
            {
                storyboard = (Storyboard)this.Resources["CollapseBorderStoryboard"];
                ToggleLabel.Content = "▲"; // Change to up arrow when collapsing

                ChatPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                storyboard = (Storyboard)this.Resources["ExpandBorderStoryboard"];
                ToggleLabel.Content = "▼"; // Change to down arrow when expanding

            }
            storyboard.Begin();
        }

        private void ToggleLabel_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            // Stop the timer when the user quits the chat
            chatReloadTimer.Stop();

            // Hide the chat panel
            ChatPanel.Visibility = Visibility.Collapsed;
        }


        private void mybutton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text;
            string receiverId = selectedUserId;

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(receiverId))
            {
                // Insert new message into the database
                string query = $"INSERT INTO UserMessages (sender_id, receiver_id, message, timestamp) VALUES ('{CurrentUserId}', '{receiverId}', '{message}', GETDATE())";
                new DbConnection().setData(query);

                // Clear input and update chat display
                MessageInput.Clear();
                LoadChatHistory(receiverId, selectedUserName);
            }
        }
        private void searchBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            searchBox1.Foreground = new SolidColorBrush((Color)Colors.Black);
            if (searchBox1.Text == "🔍  Search" || searchBox1.Text == "")
            {
                searchBox1.Text = "";
            }

            searchBox1.BorderBrush = null;
        }

        private void searchBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "" || searchBox1.Text == "🔍  Search")
            {

                searchBox1.Text = "🔍  Search";

                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
                Window_Loaded(sender, e);
            }
        }

        private async void searchBox_TextChanged1(object sender, TextChangedEventArgs e)
        {

            await Task.Delay(300);

            fulln = searchBox1.Text;
            if (searchBox1.Text != "" && searchBox1.Text != "🔍  Search")
            {
                ChatStackPanel.Children.Clear();

                string query = $"SELECT id, fullname FROM users_info WHERE  id <> '{CurrentUserId}' and fullname like '{fulln}'";
                DataSet ds = new DbConnection().getData(query);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string userId = row["id"].ToString();
                    string fullName = row["fullname"].ToString();

                    // Query to get the last message for each user
                    string lastMessageQuery = $"SELECT TOP 1 message FROM UserMessages WHERE sender_id = '{userId}' ORDER BY timestamp DESC";
                    DataSet lastMessageDs = new DbConnection().getData(lastMessageQuery);
                    string lastMessage = lastMessageDs.Tables[0].Rows.Count > 0 ? lastMessageDs.Tables[0].Rows[0]["message"].ToString() : "No messages yet";

                    // Create a StackPanel to hold the image and text
                    StackPanel cardContent = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };

                    // Create the Image element
                    Image userImage = new Image
                    {
                        Source = new BitmapImage(new Uri("images/1414.png", UriKind.Relative)),
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(0, 0, 0, 0) // Margin between image and text
                    };

                    // Create the TextBlock for the user's full name and last message
                    TextBlock buttonContent = new TextBlock
                    {
                        Margin = new Thickness(10, 20, 0, 0) // Adjust the top margin here
                    };

                    buttonContent.Inlines.Add(new Run(fullName)
                    {
                        FontSize = 16,
                        FontWeight = FontWeights.Bold
                    });

                    buttonContent.Inlines.Add(new LineBreak());

                    buttonContent.Inlines.Add(new Run($"Last message: {lastMessage}")
                    {
                        FontSize = 12
                    });

                    // Add the image and text to the StackPanel
                    cardContent.Children.Add(userImage);
                    cardContent.Children.Add(buttonContent);

                    // Create the Button for each user
                    Button cardButton = new Button
                    {
                        Content = cardContent,
                        Tag = userId,
                        Margin = new Thickness(0),
                        Padding = new Thickness(0),
                        Background = new SolidColorBrush((Colors.White)),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        BorderBrush = null,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        Width = 400
                    };

                    cardButton.Click += CardButton_Click;

                    ChatStackPanel.Children.Add(cardButton);
                }

                // Start the glow animation
                var glowAnimation = (Storyboard)Resources["GlowAnimation"];
                /*glowAnimation.Begin(glowingBorder1, true);
                glowAnimation.Begin(glowingBorder2, true);
                glowAnimation.Begin(glowingBorder3, true);*/
            }
            else
            {
                ChatDisplay.Children.Clear();
                Window_Loaded(sender, e);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string message = MessageInput.Text;
            string receiverId = selectedUserId;

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(receiverId))
            {
                // Insert new message into the database
                string query = $"INSERT INTO UserMessages (sender_id, receiver_id, message, timestamp) VALUES ('{CurrentUserId}', '{receiverId}', '{message}', GETDATE())";
                new DbConnection().setData(query);

                // Clear input and update chat display
                MessageInput.Clear();
                LoadChatHistory(receiverId, selectedUserName);
            }
        }

        private async void searchBox1_GotFocus1(object sender, RoutedEventArgs e)
        {
            isTextboxFocused = true;
            chatReloadTimer.Stop();

            MessageInput.Foreground = new SolidColorBrush(Colors.Black);
            if (MessageInput.Text == "Type a message" || MessageInput.Text == "")
            {
                MessageInput.Text = "";
            }

            MessageInput.BorderBrush = null;

            // Wait for 25 seconds, then start the timer again if still focused
            await Task.Delay(25000);
            if (isTextboxFocused)
            {
                chatReloadTimer.Start();
            }
        }

        private void searchBox1_LostFocus1(object sender, RoutedEventArgs e)
        {
            isTextboxFocused = false;

            if (!chatReloadTimer.IsEnabled)
            {
                chatReloadTimer.Start();
            }

            if (MessageInput.Text == "" || MessageInput.Text == "Type a message")
            {
                MessageInput.Text = "Type a message";
                MessageInput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
                Window_Loaded(sender, e);
            }
        }

        private void searchBox_TextChanged11(object sender, TextChangedEventArgs e)
        {

        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
             SendButton_Click(sender, e);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            AdminDash ad = new AdminDash();
            ad.Show();
            ad.Show();
        }

       

       
        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {

        }


        

        

        

        

        private void ExchangeRateLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the TextBox that raised the event
            var textBox = sender as TextBox;

            // Store the caret position before modifying the text
            int caretPosition = textBox.SelectionStart;

            // Remove any existing commas
            string text = textBox.Text.Replace(",", "");

            // Check if the text is numeric
            if (decimal.TryParse(text, out decimal value))
            {
                // Format the number with commas
                textBox.Text = string.Format("{0:N0}", value);

                // Restore the caret position
                textBox.SelectionStart = caretPosition;
            }
        }

       

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            report.Visibility = Visibility.Visible;
            report.Opacity = 1;
            showreport.Visibility = Visibility.Visible;  
            showreport.Opacity = 0;
            showreport.reprots1.Opacity = 1;
            showreport.reprots1.Visibility = Visibility.Visible;

            

            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            fadeOutStoryboard.Begin();

            await Task.Delay(1200);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            numOFmexpire.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF628788"));
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            numOFmexpire.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8EBDBF"));
        }

        private void Border_MouseEnter_1(object sender, MouseEventArgs e)
        {
            numOFmout.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF628788"));
        }

        private void Border_MouseLeave_1(object sender, MouseEventArgs e)
        {
            numOFmout.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8EBDBF"));
        }

        private void Button_Clicks(object sender, RoutedEventArgs e)
        {
            setting.mainsetting.Opacity = 1; // Reset Opacity to 1
            setting.mainsetting.Visibility = Visibility.Visible;

            setting.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
            fadeOutStoryboard.Begin();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
          
            if (nav_pnl.Width != 61)
            {
                Tg_Btn_Unchecked(sender, e);
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);
            }

        }

        private void Button_Clicka(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Clickr(object sender, RoutedEventArgs e)
        {

        }

        private void lgog_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }
        private void StartGifAnimation()
        {





            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-41-home-hover-home-1.gif"));
            ImageBehavior.SetAnimatedSource(spendgif, image3);
            ImageBehavior.SetRepeatBehavior(spendgif, new RepeatBehavior(1));



            var image4 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-955-demand-in-reveal1.gif"));
            ImageBehavior.SetAnimatedSource(spendgif1, image4);
            ImageBehavior.SetRepeatBehavior(spendgif1, new RepeatBehavior(1));




            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1356-wooden-box-in-reveal.gif"));
            ImageBehavior.SetAnimatedSource(spendgif2, image5);
            ImageBehavior.SetRepeatBehavior(spendgif2, new RepeatBehavior(1));



            var image6 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-5-wallet-hover-wallet (1).gif"));
            ImageBehavior.SetAnimatedSource(spendgif3, image6);
            ImageBehavior.SetRepeatBehavior(spendgif3, new RepeatBehavior(1));
            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-6-shopping-in-shopping.gif"));
            ImageBehavior.SetAnimatedSource(spendgi, image7);
            ImageBehavior.SetRepeatBehavior(spendgi, new RepeatBehavior(1));
        }
        private void lgog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nav_pnl.Width != 240)
            {
                Tg_Btn_Checked(sender, e);
                Storyboard showStoryboard = (Storyboard)FindResource("ShowStackPanel");

                // Begin the storyboard
                showStoryboard.Begin(this);
            }
        }

        private void background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nav_pnl.Width != 61)
            {
                Tg_Btn_Unchecked(sender, e);
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);
            }
        }

        private void view1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void ListViewItem_Selected_2(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(main_background);

            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
        }

        private void ListViewItem_Selectd_2(object sender, MouseButtonEventArgs e)
        {

            medic_num1.medicadd.Visibility = Visibility.Collapsed;
            medic_num1.medicnum.Visibility = Visibility.Visible;
            if (nav_pnl.Width != 61)
            {

                Tg_Btn_Unchecked(sender, e);

                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");

                // Begin the storyboard
                hideStoryboard.Begin(this);

            }
            SetActiveUserControl(medic_num1);
        }
    }
}

