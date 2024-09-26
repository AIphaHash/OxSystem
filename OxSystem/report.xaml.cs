using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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
using WpfAnimatedGif;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for report.xaml
    /// </summary>
    public partial class report : UserControl
    {
        public class CheckableItem
        {
            public string Name { get; set; }
            public bool IsChecked { get; set; } // Checkbox state

            // Property to display selected items
            public string DisplayText => IsChecked ? Name : null;
        }


        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public report()
        {
            InitializeComponent();
        }

        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            back.IsEnabled = false;
            await Task.Delay(200);
            // Show the make grid
            Storyboard moveUpStoryboard = (Storyboard)FindResource("MoveUpStoryboard");
            moveUpStoryboard.Begin();
         
            make.Visibility = Visibility.Visible;
            await Task.Delay(300);
            // Fade in the make grid
            Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
            Storyboard.SetTargetName(fadeInStoryboard.Children[0], "make");
            fadeInStoryboard.Begin(this);

            // Fade out the choose grid
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "choose");
            fadeOutStoryboard.Begin(this);
            back.IsEnabled = true;
        

            // After the animation completes, collapse the choose grid
            choose.Visibility = Visibility.Collapsed;
            choose.Opacity = 1; // Reset Opacity for next time
          
        }


        private void mybutton_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void mybutton_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            choose.Opacity = 1;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "reprots1");
            fadeOutStoryboard.Begin(this);

            await Task.Delay(1200);

            // Hide and reset the grids and UserControl
            make.Visibility = Visibility.Collapsed;
            history.Visibility = Visibility.Collapsed;
            choose.Visibility = Visibility.Visible;
            choose.Opacity = 1; // Reset opacity to 1 so it's visible next time
            this.Visibility = Visibility.Collapsed;
            reprots1.Opacity = 1;
            Properties.Settings.Default.report_visible = "1";
            Properties.Settings.Default.Save();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];


            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "make");

            fadeOutStoryboard.Begin(this);
            await Task.Delay(1200);

            make.Visibility = Visibility.Collapsed;
        }

        private async void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border card = sender as Border;
            if (card != null)
            {
                // Call the animate border method on the clicked card
                AnimateBorder(card);
            }
            await Task.Delay(100);

            Border clickedCard = sender as Border;
            if (clickedCard != null)
            {
                int reportId = Convert.ToInt32(clickedCard.Tag); // Retrieve the report ID stored in the Tag

                // Update the state of the report to 'read'
                string updateQuery = $"UPDATE report_state SET state = 'read' WHERE rid = {reportId}";
                conn.setData(updateQuery); // Assuming setData executes the query

                // Query to get the full header and body of the selected report
                string query = $"SELECT report_header, report_body, report_from FROM report WHERE rid = {reportId}";
                DataSet ds = conn.getData(query);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    // Extract the report details
                    string fullHeader = ds.Tables[0].Rows[0]["report_header"].ToString();
                    string fullBody = ds.Tables[0].Rows[0]["report_body"].ToString();
                    string from_ = ds.Tables[0].Rows[0]["report_from"].ToString();

                    // Set the TextBlocks to display the report details
                    headerreport.Text = fullHeader;
                    bodyreport.Text = fullBody;
                    fullname.Text = from_;

                    // Start the animation to slide in the details grid
                    Storyboard showGridInStoryboard = (Storyboard)FindResource("ShowGridInAnimation");
                    showGridInStoryboard.Begin();

                    // Optionally, hide or show grids based on your UI structure
                    // history.Visibility = Visibility.Collapsed;
                    // show.Visibility = Visibility.Visible;
                }
            }
        }



        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private async void mybutton_Click1(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            Storyboard moveUpStoryboard = (Storyboard)FindResource("MoveUpStoryboard");
            moveUpStoryboard.Begin();
            await Task.Delay(400);// Show the make grid
            history.Visibility = Visibility.Visible;

            // Fade in the make grid
            Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
            Storyboard.SetTargetName(fadeInStoryboard.Children[0], "history");
            fadeInStoryboard.Begin(this);

            // Fade out the choose grid
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "choose");
            fadeOutStoryboard.Begin(this);


            // After the animation completes, collapse the choose grid
            choose.Visibility = Visibility.Collapsed;
            choose.Opacity = 1; // Reset Opacity for next time
        }




        private void reprots1_Loaded(object sender, RoutedEventArgs e)
        {
            // Query to get distinct user names
            query = "SELECT DISTINCT user_name FROM users_info";
            ds = conn.getData(query); // Assuming getData returns a DataSet

            // Populate the popup with checkboxes from the query result
            PopulateDropdownWithCheckBoxes();

            // Retrieve the user's role from the database
            query = "SELECT role, user_name FROM users_info WHERE id = '" + Login_.iduser + "'";
            ds = conn.getData(query);
            string userrole = ds.Tables[0].Rows[0][0].ToString();
            string username = ds.Tables[0].Rows[0][1].ToString();

            // Query to get the reports
            query = "SELECT rid, report_header, report_body, report_from, report_date, report_to_role FROM report";
            ds = conn.getData(query);

            // Create a list to store the filtered reports
            List<DataRow> filteredReports = new List<DataRow>();

            // Loop through each row in the dataset
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string reportToRole = row["report_to_role"].ToString();
                string[] roles = reportToRole.Split(',');

                bool includeReport = false;

                foreach (string role in roles)
                {
                    string trimmedRole = role.Trim();
                    if (trimmedRole.Equals("All", StringComparison.OrdinalIgnoreCase) ||
                        trimmedRole.Equals(userrole, StringComparison.OrdinalIgnoreCase) ||
                        trimmedRole.Equals(username, StringComparison.OrdinalIgnoreCase))
                    {
                        includeReport = true;
                        break;
                    }
                }

                if (includeReport)
                {
                    filteredReports.Add(row);
                }
            }

            // Clear the existing cards
            CardPanel.Children.Clear();
            foreach (DataRow row in filteredReports)
            {
                string ridValue = row["report_body"] != DBNull.Value ? row["report_body"].ToString() : "N/A";
                string headerValue = row["report_header"] != DBNull.Value ? row["report_header"].ToString() : "N/A";
                string fromValue = row["report_from"] != DBNull.Value ? row["report_from"].ToString() : "N/A";
                string dateValue = row["report_date"] != DBNull.Value ? row["report_date"].ToString() : "N/A";

                LinearGradientBrush originalBrush = new LinearGradientBrush
                {
                    StartPoint = new Point(1, 0),
                    EndPoint = new Point(0.5, 1),
                    GradientStops = new GradientStopCollection
    {
        new GradientStop((Color)ColorConverter.ConvertFromString("#FF315A5F"), 0),
        new GradientStop((Color)ColorConverter.ConvertFromString("#FF4A907F"), 1)
    }
                };

                // Create the Border
                Border card = new Border
                {
                    Width = 604,
                    Height = 150,
                    BorderThickness = new Thickness(0),
                    BorderBrush = Brushes.Gray,
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(10),
                    Background = originalBrush, // Set the initial gradient background
                    Tag = row["rid"] // Store the report ID in the Tag property
                };

                // MouseEnter event to change the background to hover color
                card.MouseEnter += (s, args) =>
                {
                    card.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF466563")); // Hover color
                };

                // MouseLeave event to reset the background to the original gradient
                card.MouseLeave += (s, args) =>
                {
                    card.Background = originalBrush;  // Reset to the original gradient background
                };
                // Add the MouseLeftButtonDown event handler
                card.MouseLeftButtonDown += Card_MouseLeftButtonDown;

                // Create the card content (same as before)
                Grid cardGrid = new Grid();
                cardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                cardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                card.Child = cardGrid;

                StackPanel headerPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0)
                };
                Grid.SetRow(headerPanel, 0);
                cardGrid.Children.Add(headerPanel);

                Grid imageOverlayGrid = new Grid
                {
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(5, 5, 0, 0)
                };

                Image overlayImage = new Image
                {
                    Source = new BitmapImage(new Uri("/images/output-onlinepngtools.png", UriKind.Relative)),
                    Width = 50,
                    Height = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                imageOverlayGrid.Children.Add(overlayImage);

                headerPanel.Children.Add(imageOverlayGrid);

                StackPanel senderPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 10, 0, 0)
                };

                TextBlock senderName = new TextBlock
                {
                    Text = fromValue,
                    Foreground = Brushes.White,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold
                };
                senderPanel.Children.Add(senderName);

                TextBlock timeText = new TextBlock
                {
                    Text = "Time: " + dateValue,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE0E0E0")),
                    FontSize = 12,
                    Margin = new Thickness(0, 0, 0, 0)
                };
                senderPanel.Children.Add(timeText);

                headerPanel.Children.Add(senderPanel);

                Border contentBorder = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    Background = Brushes.White,
                    BorderThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0, 0, 10, 10),
                    Margin = new Thickness(0, 17, 0, 0),
                    Padding = new Thickness(10)
                };
                Grid.SetRow(contentBorder, 1);
                cardGrid.Children.Add(contentBorder);

                StackPanel contentPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                contentBorder.Child = contentPanel;

                Image gifImage = new Image
                {
                    Source = new BitmapImage(new Uri("/images/system-regular-47-chat-hover-chat.gif", UriKind.Relative)),
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5, 5, 10, 0)
                };

                ImageBehavior.SetAnimatedSource(gifImage, gifImage.Source);
                ImageBehavior.SetRepeatBehavior(gifImage, System.Windows.Media.Animation.RepeatBehavior.Forever);

                contentPanel.Children.Add(gifImage);

                StackPanel textPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };

                TextBlock headerText = new TextBlock
                {
                    Text = "• " + headerValue,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Margin = new Thickness(-30, 0, 0, 0)
                };
                textPanel.Children.Add(headerText);

                TextBlock reportDetail = new TextBlock
                {
                    Text = ridValue.Length > 15 ? ridValue.Substring(0, 60) + "....." : ridValue,
                    FontSize = 14
                };
                textPanel.Children.Add(reportDetail);

                contentPanel.Children.Add(textPanel);

                // Handle state (same as before)
                string rid = row["rid"].ToString();
                string stateQuery = $"SELECT state FROM report_state WHERE rid = '{rid}'";
                DataSet stateDataSet = conn.getData(stateQuery);
                string reportState = stateDataSet.Tables[0].Rows.Count > 0 ? stateDataSet.Tables[0].Rows[0]["state"].ToString() : "";

                if (!reportState.Equals("read", StringComparison.OrdinalIgnoreCase))
                {
                    Border glowingDot = new Border
                    {
                        Width = 25,
                        Height = 25,
                        CornerRadius = new CornerRadius(12.5),
                        Background = new RadialGradientBrush
                        {
                            GradientStops = new GradientStopCollection
            {
                new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF81D0B6"), Offset = 1 },
                new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF2C7D63"), Offset = 0 }
            }
                        },
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0, 12, 12, 0)
                    };

                    Storyboard glowAnimation = new Storyboard();
                    DoubleAnimation opacityAnimation = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.1,
                        Duration = new Duration(TimeSpan.FromSeconds(1)),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };

                    Storyboard.SetTarget(opacityAnimation, glowingDot);
                    Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Border.OpacityProperty));
                    glowAnimation.Children.Add(opacityAnimation);
                    glowAnimation.Begin();

                    cardGrid.Children.Add(glowingDot);
                }

                // Add the card to the stack panel
                CardPanel.Children.Add(card);
            }
        }
        private void AnimateBorder(Border targetBorder)
        {
            if (targetBorder != null)
            {
                // Set the RenderTransformOrigin to the center for scaling effect
                targetBorder.RenderTransformOrigin = new Point(0.5, 0.5);

                // Check if the RenderTransform is a TransformGroup; if not, create it
                if (!(targetBorder.RenderTransform is TransformGroup transformGroup))
                {
                    // Create a new TransformGroup with ScaleTransform as the first child
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(new ScaleTransform(1.0, 1.0)); // [0] is ScaleTransform
                    targetBorder.RenderTransform = transformGroup;
                }

                // Retrieve the storyboard from resources
                Storyboard storyboard = (Storyboard)this.Resources["ClickEffectStoryboard"];

                if (storyboard != null)
                {
                    // Apply the storyboard to the target border
                    Storyboard.SetTarget(storyboard, targetBorder);

                    // Start the animation
                    storyboard.Begin();
                }
                else
                {
                    // Handle the case where the storyboard is not found
                    MessageBox.Show("ClickEffectStoryboard resource not found.");
                }
            }
        }



        // Event handler for clicking the card
      




        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            /*show.Visibility = Visibility.Collapsed;
            history.Visibility = Visibility.Visible;*/
           
        }

        

        private async void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "make");
            fadeOutStoryboard.Begin(this);
            await Task.Delay(300);
            choose.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
            Storyboard.SetTargetName(fadeInStoryboard.Children[0], "choose");
            fadeInStoryboard.Begin(this);
           
            Storyboard moveBackStoryboard = (Storyboard)FindResource("MoveBackStoryboard");
            moveBackStoryboard.Begin();




            // Fade out the choose grid



            make.Visibility = Visibility.Collapsed;
        }
        private void StartGifAnimation()
        {
            // Run the GIF animation setup on a separate thread

         
            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-11-arrow-up-hover-arrow-up-2.gif"));
            ImageBehavior.SetAnimatedSource(back, image2);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever); 
            var image = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-177-envelope-send-hover-flying (1).gif"));
            ImageBehavior.SetAnimatedSource(image33, image);
            ImageBehavior.SetRepeatBehavior(image33, System.Windows.Media.Animation.RepeatBehavior.Forever); 
            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-12-arrow-down-hover-arrow-down-1 (1).gif"));
            ImageBehavior.SetAnimatedSource(back1, image1);
            ImageBehavior.SetRepeatBehavior(back1, System.Windows.Media.Animation.RepeatBehavior.Forever); 
            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-29-cross (2).gif"));
            ImageBehavior.SetAnimatedSource(image4, image3);
            ImageBehavior.SetRepeatBehavior(image4, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image44 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-720-spinner-half-circles-loop-rotation.gif"));
            ImageBehavior.SetAnimatedSource(profitgif, image44);
            ImageBehavior.SetRepeatBehavior(profitgif, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image54 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-11-arrow-up-hover-arrow-up-2 (1).gif"));
            ImageBehavior.SetAnimatedSource(back11, image54);
            ImageBehavior.SetRepeatBehavior(back11, System.Windows.Media.Animation.RepeatBehavior.Forever);


        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }

        private void body_GotFocus(object sender, RoutedEventArgs e)
        {if(body.Text == "") { 
            reporth.Visibility = Visibility.Collapsed;
            body.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3FAFA2"));
        }
        }
        private void body_LostFocus(object sender, RoutedEventArgs e)
        {
            if ( body.Text == "") { 
            reporth.Visibility = Visibility.Visible;
            body.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A5A5"));
        }
        }
        private async void back_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {


            choose.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
            Storyboard.SetTargetName(fadeInStoryboard.Children[0], "choose");
            fadeInStoryboard.Begin(this);

            // Fade out the choose grid
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "history");
            fadeOutStoryboard.Begin(this);
            await Task.Delay(300);
            Storyboard moveBackStoryboard = (Storyboard)FindResource("MoveBackStoryboard");
            moveBackStoryboard.Begin();
          

            history.Visibility = Visibility.Collapsed;
        }

        private void Label_Loaded_1(object sender, RoutedEventArgs e)
        {
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
        }
        public void MoveLabelUp(Label label)
        {
            // Create a storyboard
            Storyboard storyboard = new Storyboard();

            // Create a DoubleAnimation to move the label up by 34 units
            DoubleAnimation moveUpAnimation = new DoubleAnimation
            {
                From = 0, // Start from current position
                To = -30, // Move up by 34 units (negative value moves upward)
                Duration = new Duration(TimeSpan.FromSeconds(0.1)) // Duration of the animation
            };

            // Set the target property (Y translation) for the animation
            Storyboard.SetTarget(moveUpAnimation, label);
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            // Add the animation to the storyboard
            storyboard.Children.Add(moveUpAnimation);

            // Start the animation
            storyboard.Begin();
        }

        public void MoveLabelDown(Label label)
        {
            // Create the TranslateTransform if not already defined
            TranslateTransform trans = new TranslateTransform();
            label.RenderTransform = trans;

            // Create the storyboard for moving the label down
            Storyboard storyboard = new Storyboard();

            // Create a DoubleAnimation to move the label down by 34 units
            DoubleAnimation animation = new DoubleAnimation
            {
                From = -30,
                To = 0, // Moving down by 34 units
                Duration = new Duration(TimeSpan.FromSeconds(0.1)) // Duration of the animation
            };

            // Set the target property for the animation (Y-axis translation)
            Storyboard.SetTarget(animation, label);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard to start the animation
            storyboard.Begin();
        }

        private void header_GotFocus(object sender, RoutedEventArgs e)
        {if(header.Text == "") {
            MoveLabelUp(rh);
            header.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3FAFA2"));
            }
        }

        private void header_LostFocus(object sender, RoutedEventArgs e)
        {
            if(header.Text == "") { 
            MoveLabelDown(rh);
            header.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A5A5"));
            }
        }

        private void rh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (header.Focus() == false)
            {
                header_GotFocus(sender, e);
                header.Focus();
            }
        }

        private List<string> checkedItems = new List<string>();

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


        // Button click event to toggle the popup
        private void PermsButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the popup
            permsPopup.IsOpen = !permsPopup.IsOpen;
        }

        // Checkbox click event to track checked/unchecked items
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

        private void Image_MouseLeftButtonDown3(object sender, MouseButtonEventArgs e)
        {
            // Query to get the full name of the current user
            query = "SELECT fullname FROM users_info WHERE id = '" + Login_.iduser + "'";
            ds = conn.getData(query);

            if (ds.Tables[0].Rows.Count > 0 && !DBNull.Value.Equals(ds.Tables[0].Rows[0][0]))
            {
                string fulln = ds.Tables[0].Rows[0][0].ToString();

                // Get the current date in the desired format
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                // Store the selected value from the ComboBox into a variable (comma-separated roles)
                string selectedRole = String.Join(",", checkedItems);
                Console.WriteLine(selectedRole);

                // Insert the report data into the 'report' table
                query = "INSERT INTO report(report_header, report_body, report_from, report_to_role, report_date) " +
                        "VALUES ('" + header.Text + "', '" + body.Text + "', '" + fulln + "', '" + selectedRole + "', '" + currentDate + "')";
                conn.setData(query);

                // Retrieve the last inserted report ID
                query = "SELECT TOP 1 rid FROM report ORDER BY rid DESC"; // Get the latest report ID
                ds = conn.getData(query);
                string rid = ds.Tables[0].Rows[0][0].ToString();

                // Split the selected roles by commas
                string[] roles = selectedRole.Split(',');
                Console.WriteLine(roles);
                foreach (string role in roles)
                {
                    // If the role is one of the predefined roles like 'Pharm', 'Admin', 'Accountant'
                    if (role == "Pharm" || role == "Admin" || role == "Accountant")
                    {
                        // Query to get all users with the specified role
                        query = "SELECT user_name FROM users_info WHERE role = '" + role + "'";
                        ds = conn.getData(query);

                        foreach (DataRow userRow in ds.Tables[0].Rows)
                        {
                            if (!DBNull.Value.Equals(userRow["user_name"]))
                            {
                                string userName = userRow["user_name"].ToString();

                                // Insert each user with the role into the report_state table
                                query = "INSERT INTO report_state(rid, permname, state) " +
                                        "VALUES ('" + rid + "', '" + userName + "', 'unread')";
                                conn.setData(query);
                            }
                        }
                    }
                    else
                    {
                        // For specific individual usernames (not roles), insert them directly into report_state
                        query = "INSERT INTO report_state(rid, permname, state) " +
                                "VALUES ('" + rid + "', '" + role + "', 'unread')";
                        conn.setData(query);
                    }
                }
            }
            else
            {
                // Handle case where fullname retrieval fails
                Console.WriteLine("Error: Failed to retrieve full name.");
            }

            // Reload the reports to reflect the new entries
            reprots1_Loaded(sender, e);
        }

        private void back_MouseLeftButtonDown11(object sender, MouseButtonEventArgs e)
        {
            Storyboard showGridOutStoryboard = (Storyboard)FindResource("ShowGridOutAnimation");
            showGridOutStoryboard.Begin();
        }
    }
}
