using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for Accountent.xaml
    /// </summary>
    /// 
    

    public partial class Accountent : Window
    {
        private DispatcherTimer chatReloadTimer;
        private bool isTextboxFocused = false;
        public static string fulln;
        public string CurrentUserId = Login_.iduser;
        private string selectedUserId = null;
        private string selectedUserName = null;
        string dateString = "%";
        string datehistroy = "%";
        private DateTime _selectedDate;
        string d_;
        private ICollectionView _collectionView;
        public static string f_ = "%";
        public static string t_ = "%";
        string type;
        string type1;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public ObservableCollection<Bill> BillsData { get; set; }
        public Accountent()
        {
            InitializeComponent();

            chatReloadTimer = new DispatcherTimer();
            chatReloadTimer.Interval = TimeSpan.FromSeconds(3);
            chatReloadTimer.Tick += ChatReloadTimer_Tick;

        }

        private void ChatReloadTimer_Tick(object sender, EventArgs e)
        {
            if (!isTextboxFocused && !string.IsNullOrEmpty(selectedUserId))
            {
                LoadChatHistory(selectedUserId, selectedUserName);
            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            query = "select * from users_info where id = '" + CurrentUserId + "' AND perms = 'allpermmisions'";
            ds = conn.getData(query);
            if (ds.Tables[0].Rows.Count == 0)
            {
                admindash.Visibility = Visibility.Collapsed;
            }
            else
            {
                admindash.Visibility = Visibility.Visible;
            }

           
            ChatStackPanel.Children.Clear();

            // Query to get admin users, excluding the logged-in user
            query = $"SELECT id, fullname FROM users_info WHERE  id <> '{CurrentUserId}'";
            ds = new DbConnection().getData(query);

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

        private void admindash_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            AdminDash ad = new AdminDash();
            ad.Show();
        } 
        

        private void admindash_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void Button_Clicks(object sender, RoutedEventArgs e)
        {
            if (setting.Visibility != Visibility.Visible)
            {
                setting.mainsetting.Visibility = Visibility.Visible;

                setting.Visibility = Visibility.Visible;
                Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
                fadeOutStoryboard.Begin();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            report.Visibility = Visibility.Visible;
            report.Opacity = 1;
            showreport.Visibility = Visibility.Visible;
            showreport.Opacity = 0;
            showreport.showg.Opacity = 1;
            showreport.showg.Visibility = Visibility.Visible;



            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            fadeOutStoryboard.Begin();

            await Task.Delay(1200);
        }

        private void navbarhover_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard expandStoryboard = (Storyboard)this.FindResource("ExpandNavbar");
            expandStoryboard.Begin();
            
        }

        private void navbarhover_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard collapseStoryboard = (Storyboard)this.FindResource("CollapseNavbar");
            collapseStoryboard.Begin();
          
        }

        private void syncdate_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Clickg(object sender, RoutedEventArgs e)
        {
            query = "UPDATE state\r\nSET state.state = 'unseen'\r\nFROM state\r\nINNER JOIN users_info ON state.userid = users_info.id\r\nWHERE users_info.id = '" + Login_.iduser + "' ";
            conn.setData(query);
            Login_ l = new Login_();
            l.Show();
            this.Close();
        }
        private void ShowUserControl(UIElement controlToShow, UIElement controlToHide)
        {
            // If the controlToShow is already visible, we do nothing
            if (controlToShow.Visibility == Visibility.Visible)
            {
                return; // Exit if the control is already open
            }

            // If the control to hide is visible, trigger the fade-out animation
            if (controlToHide.Visibility == Visibility.Visible)
            {
                Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard2"];
                fadeOutStoryboard.Completed += (s, e) =>
                {
                    controlToHide.Visibility = Visibility.Collapsed;
                    controlToHide.Opacity = 0;  // Reset the opacity

                    // Fade-in the new control after the fade-out is completed
                    controlToShow.Visibility = Visibility.Visible;
                    Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard1"];
                    fadeInStoryboard.Begin((FrameworkElement)controlToShow); // Cast to FrameworkElement
                };

                fadeOutStoryboard.Begin((FrameworkElement)controlToHide); // Cast to FrameworkElement
            }
            else
            {
                // If no control is currently visible, directly fade in the new control
                controlToShow.Visibility = Visibility.Visible;
                Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard1"];
                fadeInStoryboard.Begin((FrameworkElement)controlToShow); // Cast to FrameworkElement
            }
        }



        private void Dashboard1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowUserControl(accountentdashboard, disc);
        }

        private void Dashboard2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowUserControl(disc, accountentdashboard);
        }
    }
}
