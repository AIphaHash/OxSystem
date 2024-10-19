using SciChart.Charting.Visuals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Threading;
using static OxSystem.Login;
using WpfAnimatedGif;
using Syncfusion.Windows.Shared;
namespace OxSystem

{
    public partial class AdminDash : Window
    {

        private DispatcherTimer chatReloadTimer;
        private bool isTextboxFocused = false;
        public static string fulln;
        public string CurrentUserId = Login_.iduser;
        private string selectedUserId = null;
        private string selectedUserName = null;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public AdminDash()
        {
            InitializeComponent();
            LV.SelectedIndex = 0;
            query = "SELECT role, fullname, address FROM users_info WHERE dbid = '"+Properties.Settings.Default.dbid+"' and id = '" + CurrentUserId + "'";
            ds = conn.getData(query);

            // Check if the DataSet contains tables and if the first table has rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // If there are rows, set the control values
                Fulln.Content = ds.Tables[0].Rows[0][1].ToString(); // Full Name
                Pname.Text = ds.Tables[0].Rows[0][0].ToString();  // Role
                place.Text = ds.Tables[0].Rows[0][1].ToString();  // Address
                role_.Text = ds.Tables[0].Rows[0][2].ToString();   // Role
            }
            else
            {
                
                // If there are no rows, set a default value for role_
                role_.Text = "Default Role"; // Replace with your desired default value
            }

            navbar.Visibility = Visibility.Collapsed;
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
        private async void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard2"];
            Storyboard.SetTarget(fadeInStoryboard, lgog);
            fadeInStoryboard.Begin();
            op.Visibility = Visibility.Collapsed;
            lgog.Visibility = Visibility.Visible;
            Addacount.Opacity = 1;
            info.Opacity = 1;
            adminAccount.Opacity = 1;
            showaccounts.Opacity = 1;
            AdminDashboard.Opacity = 1;
            lgog.Visibility = Visibility.Visible;
        }

        private async void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard2"];
            Storyboard.SetTarget(fadeOutStoryboard, lgog);
            fadeOutStoryboard.Begin();
            op.Visibility = Visibility.Visible;
            Addacount.Opacity = 0.3;
            info.Opacity = 0.3;
            adminAccount.Opacity = 0.3;
            showaccounts.Opacity = 0.3;
            AdminDashboard.Opacity = 0.3;
            lgog.Visibility = Visibility.Collapsed;
            lgog.Visibility = Visibility.Collapsed;
        }

        private void LoginUser(string userId)
        {
            CurrentUserId = userId;
        }
        public void SetActiveUserControl(UserControl newControl)
        {
            var currentVisibleControl = BG.Children.OfType<UserControl>().FirstOrDefault(c => c.Visibility == Visibility.Visible);
            if (currentVisibleControl == newControl)
            {
                return;
            }
            if (currentVisibleControl != null)
            {
                var dragOutStoryboard = (Storyboard)this.Resources["DragOutStoryboard"];
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
                    currentVisibleControl.Visibility = Visibility.Collapsed;
                    newControl.Visibility = Visibility.Visible;
                    var dragInStoryboard = (Storyboard)this.Resources["DragInStoryboard"];
                    foreach (var animation in dragInStoryboard.Children.OfType<ThicknessAnimation>())
                    {
                        Storyboard.SetTarget(animation, newControl);
                    }
                    foreach (var animation in dragInStoryboard.Children.OfType<DoubleAnimation>())
                    {
                        Storyboard.SetTarget(animation, newControl);
                    }
                    dragInStoryboard.Begin();
                }
                dragOutStoryboard.Begin();
            }
            else
            {
                newControl.Visibility = Visibility.Visible;
                var dragInStoryboard = (Storyboard)this.Resources["DragInStoryboard"];
                foreach (var animation in dragInStoryboard.Children.OfType<ThicknessAnimation>())
                {
                    Storyboard.SetTarget(animation, newControl);
                }
                foreach (var animation in dragInStoryboard.Children.OfType<DoubleAnimation>())
                {
                    Storyboard.SetTarget(animation, newControl);
                }
                dragInStoryboard.Begin();
            }
        }
        private void ListViewItem_Selected_1(object sender, RoutedEventArgs e)
        {
            DateTime currentDateOnly = DateTime.Now;
            query = "UPDATE state\r\nSET state.state = 'unseen'\r\nFROM state\r\nINNER JOIN users_info ON state.userid = users_info.id\r\nWHERE users_info.dbid = '"+Properties.Settings.Default.dbid+"' and users_info.id = '" + Login_.iduser + "' ";
            conn.setData(query);
            query = "insert into loginhistory values ('" + Login_.iduser + "' , '" + currentDateOnly + "' , 'out' , '"+Properties.Settings.Default.dbid+"')";
            conn.setData(query);
            Login_ l1 = new Login_();
            l1.Show();
            this.Close();
        }
        private void ListViewItem_Selected_3(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(adminAccount);
            navbar.Visibility = Visibility.Visible;

            if (nav_pnl.Width != 61)
            {
                Tg_Btn_Unchecked(sender, e);
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }

        private void background_Loaded(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(AdminDashboard);
            navbar.Visibility = Visibility.Visible;
            Tg_Btn_Unchecked(sender, e);
            if (nav_pnl.Width != 61)
            {
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(AdminDashboard);
            navbar.Visibility = Visibility.Visible;
            Tg_Btn_Unchecked(sender, e);
            if (nav_pnl.Width != 61)
            {
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            showaccounts.showacc1.Visibility = Visibility.Visible;
            SetActiveUserControl(showaccounts);
            navbar.Visibility = Visibility.Visible;
            if (nav_pnl.Width != 61)
            {
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nav_pnl.Width != 61)
            {
                Tg_Btn_Unchecked(sender, e);
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }

        private void nav_pnl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn_Checked(sender, e);
            if (nav_pnl.Width != 240)
            {
                Storyboard showStoryboard = (Storyboard)FindResource("ShowStackPanel");
                showStoryboard.Begin(this);
            }
        }
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' ";
            ds = conn.getData(query);
        }
        private void reset_Click_1(object sender, RoutedEventArgs e)
        {
            Tg_Btn_Unchecked(sender, e);
            if (nav_pnl.Width != 61)
            {
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

            query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and id = '" + CurrentUserId + "' AND perms = 'allpermmisions'";
            ds = conn.getData(query);

            if (ds.Tables[0].Rows.Count == 0)
            {
                pharm.Visibility = Visibility.Collapsed;
                accountent.Visibility = Visibility.Collapsed;
            }
            else
            {
                pharm.Visibility = Visibility.Visible;
                accountent.Visibility = Visibility.Visible;
            }
            ChatStackPanel.Children.Clear();
            query = $"SELECT id, fullname, role FROM users_info WHERE dbid = '"+Properties.Settings.Default.dbid+"' and id <> '"+CurrentUserId+"'";
            ds = new DbConnection().getData(query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string userId = row["id"].ToString();
                string fullName = row["fullname"].ToString();
                string role = row["role"].ToString();
                string lastMessageQuery = $"SELECT TOP 1 message FROM UserMessages WHERE dbid = '"+Properties.Settings.Default.dbid+"' and sender_id = '"+userId+"' ORDER BY timestamp DESC";
                DataSet lastMessageDs = new DbConnection().getData(lastMessageQuery);
                string lastMessage = lastMessageDs.Tables[0].Rows.Count > 0 ? lastMessageDs.Tables[0].Rows[0]["message"].ToString() : "No messages yet";

                StackPanel cardContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Height = 100,
                    Margin = new Thickness(0)
                };

                Image outerImage = new Image
                {
                    Source = new BitmapImage(new Uri("images/1414.png", UriKind.Relative)),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(0, 0, 0, 0)
                };

                Image innerImage = new Image
                {
                    Width = 50,
                    Height = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

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

                Grid imageGrid = new Grid();
                imageGrid.Children.Add(outerImage);
                imageGrid.Children.Add(innerImage);

                TextBlock buttonContent = new TextBlock
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black),  // Explicitly set Foreground color to ensure visibility
                    TextWrapping = TextWrapping.Wrap,  // Allows text to wrap if necessary
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

                cardContent.Children.Add(imageGrid);
                cardContent.Children.Add(buttonContent);

                Button cardButton = new Button
                {
                    Content = cardContent,
                    Tag = userId,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    Background = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    BorderBrush = null,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Width = 400,
                    MinHeight = 100
                };

                cardButton.Click += CardButton_Click;
                ChatStackPanel.Children.Add(cardButton);
            }


            var glowAnimation = (Storyboard)Resources["GlowAnimation"];
            glowAnimation.Begin(glowingBorder1, true);
            glowAnimation.Begin(glowingBorder2, true);
            glowAnimation.Begin(glowingBorder3, true);
        }
        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            selectedUserId = clickedButton.Tag.ToString();
            TextBlock buttonContent = clickedButton.Content as TextBlock;
            if (buttonContent != null)
            {
                Run nameRun = buttonContent.Inlines.OfType<Run>().FirstOrDefault();
                if (nameRun != null)
                {
                    selectedUserName = nameRun.Text;
                }
            }
            LoadChatHistory(selectedUserId, selectedUserName);
            chatReloadTimer.Start();
            ChatPanel.Visibility = Visibility.Visible;
        }


        private void LoadChatHistory(string userId, string userName)
        {
            ChatDisplay.Children.Clear();
            string query = $"SELECT sender_id, message, timestamp FROM UserMessages WHERE dbid = '"+Properties.Settings.Default.dbid+"' and (sender_id = '"+userId+"' AND receiver_id = '"+CurrentUserId+"') OR (sender_id = '"+CurrentUserId+"' AND receiver_id = '"+userId+"') ORDER BY timestamp";
            DataSet ds = new DbConnection().getData(query);
            DateTime? lastDate = null;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string senderId = row["sender_id"].ToString();
                string message = row["message"].ToString();
                DateTime timestamp = Convert.ToDateTime(row["timestamp"]);
                if (lastDate == null || lastDate.Value.Date != timestamp.Date)
                {
                    TextBlock dateSeparator = new TextBlock
                    {
                        Text = timestamp.ToString("yyyy-MM-dd"),
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.Gray),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 10, 0, 10)
                    };
                    ChatDisplay.Children.Add(dateSeparator);
                    lastDate = timestamp.Date;
                }
                Border messageBorder = new Border
                {
                    Background = senderId == CurrentUserId
                        ? new SolidColorBrush(Color.FromRgb(200, 230, 255))
                        : new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(10),
                    Margin = new Thickness(5),
                    HorizontalAlignment = senderId == CurrentUserId ? HorizontalAlignment.Right : HorizontalAlignment.Left
                };
                StackPanel messagePanel = new StackPanel();
                TextBlock messageBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = message
                };
                TextBlock timestampBlock = new TextBlock
                {
                    Text = timestamp.ToString("HH:mm"),
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(0, 5, 0, 0)
                };
                messagePanel.Children.Add(messageBlock);
                messagePanel.Children.Add(timestampBlock);
                messageBorder.Child = messagePanel;
                ChatDisplay.Children.Add(messageBorder);
            }
            query = "select fullname from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and id = '" + selectedUserId + "'";
            ds = conn.getData(query);
            selectedUserName = ds.Tables[0].Rows[0][0].ToString();
            ChatTitle.Text = $"Chat with {selectedUserName}";
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            
            string message = MessageInput.Text;
            string receiverId = selectedUserId;

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(receiverId))
            {
                string query = $"INSERT INTO UserMessages (sender_id, receiver_id, message, timestamp , dbid) VALUES ('{CurrentUserId}', '{receiverId}', '{message}', GETDATE() ,'"+Properties.Settings.Default.dbid+"')";
                new DbConnection().setData(query);
                MessageInput.Clear();
                LoadChatHistory(receiverId, selectedUserName);
            }
        }

        private string GetSelectedUserId()
        {
            return "";
        }

        private string GetSelectedUserName()
        {
            return "";
        }
        private void ToggleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard storyboard;
            if (ExpandableBorder.Height == 350)
            {
                storyboard = (Storyboard)this.Resources["CollapseBorderStoryboard"];
                ToggleLabel.Content = "▲";
                ChatPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                storyboard = (Storyboard)this.Resources["ExpandBorderStoryboard"];
                ToggleLabel.Content = "▼";
            }
            storyboard.Begin();
        }
        private void ToggleLabel_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            chatReloadTimer.Stop();
            ChatPanel.Visibility = Visibility.Collapsed;
        }
        private void mybutton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text;
            string receiverId = selectedUserId;

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(receiverId))
            {
                string query = $"INSERT INTO UserMessages (sender_id, receiver_id, message, timestamp , dbid) VALUES ('{CurrentUserId}', '{receiverId}', '{message}', GETDATE() ,'"+Properties.Settings.Default.dbid+"')";
                new DbConnection().setData(query);
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

                string query = $"SELECT id, role, fullname FROM users_info WHERE dbid = '"+Properties.Settings.Default.dbid+"' and id <> '"+CurrentUserId+"' and fullname like '"+fulln+"'";
                DataSet ds = new DbConnection().getData(query);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string userId = row["id"].ToString();
                    string fullName = row["fullname"].ToString();
                    string role = row["role"].ToString();
                    string lastMessageQuery = $"SELECT TOP 1 message FROM UserMessages WHERE dbid = '"+Properties.Settings.Default.dbid+"' and sender_id = '"+userId+"' ORDER BY timestamp DESC";
                    DataSet lastMessageDs = new DbConnection().getData(lastMessageQuery);
                    string lastMessage = lastMessageDs.Tables[0].Rows.Count > 0 ? lastMessageDs.Tables[0].Rows[0]["message"].ToString() : "No messages yet";

                    StackPanel cardContent = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Height = 100,
                        Margin = new Thickness(0)
                    };

                    Image outerImage = new Image
                    {
                        Source = new BitmapImage(new Uri("images/1414.png", UriKind.Relative)),
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(0, 0, 0, 0)
                    };

                    Image innerImage = new Image
                    {
                        Width = 50,
                        Height = 50,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

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

                    Grid imageGrid = new Grid();
                    imageGrid.Children.Add(outerImage);
                    imageGrid.Children.Add(innerImage);

                    TextBlock buttonContent = new TextBlock
                    {
                        Margin = new Thickness(10, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.Black),  // Explicitly set Foreground color to ensure visibility
                        TextWrapping = TextWrapping.Wrap,  // Allows text to wrap if necessary
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

                    cardContent.Children.Add(imageGrid);
                    cardContent.Children.Add(buttonContent);

                    Button cardButton = new Button
                    {
                        Content = cardContent,
                        Tag = userId,
                        Margin = new Thickness(0),
                        Padding = new Thickness(0),
                        Background = new SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        BorderBrush = null,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        Width = 400,
                        MinHeight = 100
                    };

                    cardButton.Click += CardButton_Click;
                    ChatStackPanel.Children.Add(cardButton);
                }

                var glowAnimation = (Storyboard)Resources["GlowAnimation"];
                glowAnimation.Begin(glowingBorder1, true);
                glowAnimation.Begin(glowingBorder2, true);
                glowAnimation.Begin(glowingBorder3, true);
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
                string query = $"INSERT INTO UserMessages (sender_id, receiver_id, message, timestamp ,dbid) VALUES ('{CurrentUserId}', '{receiverId}', '{message}', GETDATE() ,'"+Properties.Settings.Default.dbid+"')";
                new DbConnection().setData(query);
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
        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
            pharmacist p = new pharmacist();
            p.Show();
            loading l = new loading();
            l.Show();
            this.Close();
            await Task.Delay(5000);
            l.Close();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Accountent a = new Accountent();
            a.Show();
            loading l = new loading();
            l.Show();
            this.Close();
            await Task.Delay(5000);
            l.Close();
        }
        private async void Button_Clickr(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.report_visible == "1")
            {
                report.choose.Opacity = 1;
                report.reprots1.Opacity = 1;
                report.reprots1.Visibility = Visibility.Visible;
                report.Visibility = Visibility.Visible;
                Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
                fadeOutStoryboard.Begin();
                await Task.Delay(1200);
                Properties.Settings.Default.report_visible = "0";
                Properties.Settings.Default.Save();
            }
        }

        private void Button_Clicks(object sender, RoutedEventArgs e)
        {

            if (Properties.Settings.Default.setting_visible == "1")
            {


                setting.mainsetting.Visibility = Visibility.Visible;

                setting.Visibility = Visibility.Visible;
                Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
                fadeOutStoryboard.Begin();
                Properties.Settings.Default.setting_visible = "0";
                Properties.Settings.Default.Save();
            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn_Unchecked(sender, e);
            if (nav_pnl.Width != 61)
            {
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }

        private void ListViewItem_Selected(object sender, MouseButtonEventArgs e)
        {
            showaccounts.header.Visibility = Visibility.Visible;
            showaccounts.glowingBorder.Visibility = Visibility.Visible;
            showaccounts.addaccount.Visibility = Visibility.Collapsed;
            showaccounts.showacc1.Visibility = Visibility.Visible;
            SetActiveUserControl(showaccounts);
            navbar.Visibility = Visibility.Visible;
            if (nav_pnl.Width != 61)
            {
                Tg_Btn_Unchecked(sender, e);
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }
        private void lgog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nav_pnl.Width != 240)
            {
                Tg_Btn_Checked(sender, e);
                Storyboard showStoryboard = (Storyboard)FindResource("ShowStackPanel");
                showStoryboard.Begin(this);
            }
        }
        private void ListViewItem_Selected_2(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(AdminDashboard);
            navbar.Visibility = Visibility.Visible;
            if (nav_pnl.Width != 61)
            {
                Tg_Btn_Unchecked(sender, e);
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }
        private void ListViewItem_Selected_0(object sender, MouseButtonEventArgs e)
        {
            SetActiveUserControl(Addacount);
            navbar.Visibility = Visibility.Visible;
            if (nav_pnl.Width != 61)
            {
                Tg_Btn_Unchecked(sender, e);
                Storyboard hideStoryboard = (Storyboard)FindResource("HideStackPanel");
                hideStoryboard.Begin(this);
            }
        }
        private void StartGifAnimation()
        {

            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-41-home-hover-home-1.gif"));
            ImageBehavior.SetAnimatedSource(spendgif, image3);
            ImageBehavior.SetRepeatBehavior(spendgif, new RepeatBehavior(1));



            var image4 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-955-demand-in-reveal1.gif"));
            ImageBehavior.SetAnimatedSource(spendgif1, image4);
            ImageBehavior.SetRepeatBehavior(spendgif1, new RepeatBehavior(1));




            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-16-assessment-in-assessment.gif"));
            ImageBehavior.SetAnimatedSource(spendgif2, image5);
            ImageBehavior.SetRepeatBehavior(spendgif2, new RepeatBehavior(1));



            var image6 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-8-account-in-account.gif"));
            ImageBehavior.SetAnimatedSource(spendgif3, image6);
            ImageBehavior.SetRepeatBehavior(spendgif3, new RepeatBehavior(1));
        }

        private void lgog_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }

      
    }
}
