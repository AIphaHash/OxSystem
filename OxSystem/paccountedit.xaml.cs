using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for paccountedit.xaml
    /// </summary>
    public partial class paccountedit : UserControl
    {
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public paccountedit()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
            Username.Text = adminaccount.un;
            PhoneNum.Text = adminaccount.pn;
            Email.Text = adminaccount.em;
            Address.Text = adminaccount.c;
            Password.Text = adminaccount.p;
            full.Text = adminaccount.fn;
            role.Text = adminaccount.r;



        }

        public void ApplyStoryboard(TextBox textBox)
        {
            var storyboard = (Storyboard)this.Resources["ShakeAndRedBorderStoryboard"];

            // Apply RenderTransform if not already applied
            if (textBox.RenderTransform == null || !(textBox.RenderTransform is TransformGroup))
            {
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform());
                textBox.RenderTransform = transformGroup;
            }

            // Begin the storyboard for the specific TextBox
            Storyboard.SetTarget(storyboard, textBox);
            storyboard.Begin();
        }
        private T FindParentControl<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return FindParentControl<T>(parentObject);
            }
        }
        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            string fullname = full.Text;
            string password = Password.Text;
            string phonenum = PhoneNum.Text;
            string email = Email.Text;
            string addres = Address.Text;
            if (Password.Text != "" && Address.Text != "" && Email.Text != "" && PhoneNum.Text != "" && full.Text != "" && Password.Text != "Password..." && Address.Text != "Address..." && Email.Text != "Email..." && PhoneNum.Text != "7810000000" && full.Text != "Fullname..." && Password.Text != "insert the Password!" && Address.Text != "insert the Address!" && Email.Text != "insert the Email!" && full.Text != "insert the Fullname!")
            {
                query = " update users_info set  phone_num = '" + phonenum + "' , email = '" + email + "' , address = '" + addres + "' , fullname = '" + fullname + "' , password = '" + Password.Text + "' where user_name = '" + Username.Text + "' ";
                conn.setData(query);
                edit.Visibility = Visibility.Collapsed;
                verfication1.Visibility = Visibility.Visible;
                Storyboard moveInStoryboard = (Storyboard)FindResource("MoveUpStoryboard");
                moveInStoryboard.Begin();
                await Task.Delay(300);
                Storyboard fadeInStoryboard = (Storyboard)FindResource("FadeOutStoryboard");
                fadeInStoryboard.Begin();
                await Task.Delay(300);
                verfication.Visibility = Visibility.Visible;
                this.Visibility = Visibility.Collapsed;
                var parent = FindParentControl<adminaccount>(this);

                if (parent != null)
                {
                    parent.UserControl_Loaded(sender, e);  // Call the method to reload data
                }
                else
                {
                    MessageBox.Show("Unable to find the parent Show Accounts control.");
                }
            }
            if (Username.Text == "Username..." || Username.Text == "" || Username.Text == "insert the Username!")
            {
                Username.BorderBrush = new SolidColorBrush(Colors.Red);
                Username.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Username.Text = "insert the Username!";
                ApplyStoryboard(Username);
            }
            if (Password.Text == "Password..." || Password.Text == "" || Password.Text == "insert the Password!")
            {
                Password.BorderBrush = new SolidColorBrush(Colors.Red);
                Password.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Password.Text = "insert the Password!";
                ApplyStoryboard(Password);
            }
            if (Email.Text == "Email..." || Email.Text == "" || Email.Text == "insert the Email!")
            {
                Email.BorderBrush = new SolidColorBrush(Colors.Red);
                Email.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Email.Text = "insert the Email!";
                ApplyStoryboard(Email);
            }
            if (full.Text == "Fullname..." || full.Text == "" || full.Text == "insert the Fullname!")
            {
                full.BorderBrush = new SolidColorBrush(Colors.Red);
                full.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                full.Text = "insert the Fullname!";
                ApplyStoryboard(full);
            }
            if (Address.Text == "Address..." || Address.Text == "" || Address.Text == "insert the Address!")
            {
                Address.BorderBrush = new SolidColorBrush(Colors.Red);
                Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));

                Address.Text = "insert the Address!";
                ApplyStoryboard(Address);
            }
            if (PhoneNum.Text == "7810000000" || PhoneNum.Text == "")
            {
                PhoneNum.BorderBrush = new SolidColorBrush(Colors.Red);
                PhoneNum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));

                PhoneNum.Text = "7810000000";
                ApplyStoryboard(PhoneNum);
            }




        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void Username_MouseLeave(object sender, MouseEventArgs e)
        {
            Username.Background = new SolidColorBrush(Colors.White);
        }

        private void Password_MouseLeave(object sender, MouseEventArgs e)
        {
            Password.Background = new SolidColorBrush(Colors.White);
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Password.Text == "Password..." || Password.Text == "insert the Password!")
            {
                Password.Text = "";
                Password.Foreground = new SolidColorBrush(Colors.Black);

            }
            Password.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password.Text))
            {
                Password.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                Password.Text = "Password...";
            }
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Password.IsFocused)
            {
                if (Password.Text == "" || Password.Text == "Password..." || Password.Text == "insert the Password!")
                {
                    Password.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    Password.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void full_GotFocus(object sender, RoutedEventArgs e)
        {
            if (full.Text == "Fullname..." || full.Text == "insert the Fullname!")
            {
                full.Text = "";
                full.Foreground = new SolidColorBrush(Colors.Black);

            }
            full.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void full_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(full.Text))
            {
                full.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                full.Text = "Fullname...";
            }
        }

        private void full_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (full.IsFocused)
            {
                if (full.Text == "" || full.Text == "Fullname..." || full.Text == "insert the Fullname!")
                {
                    full.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    full.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void Address_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Address.IsFocused)
            {
                if (Address.Text == "" || Address.Text == "Address..." || Address.Text == "insert the Address!")
                {
                    Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    Address.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void Address_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Address.Text))
            {
                Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                Address.Text = "Address...";
            }
        }

        private void Address_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Address.Text == "Address..." || Address.Text == "insert the Address!")
            {
                Address.Text = "";
                Address.Foreground = new SolidColorBrush(Colors.Black);

            }
            Address.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void PhoneNum_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PhoneNum.Text == "7810000000" || PhoneNum.Text == "")
            {
                PhoneNum.Text = "";
                PhoneNum.Foreground = new SolidColorBrush(Colors.Black);

            }
            PhoneNum.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void PhoneNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PhoneNum.Text))
            {
                PhoneNum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                PhoneNum.Text = "7810000000";
            }
        }

        private void PhoneNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneNum.Text, "^[0-9]+$"))
            {
                // If non-digit characters are found, clear the textbox
                PhoneNum.Text = "";

            }
            if (PhoneNum.IsFocused)
            {
                if (PhoneNum.Text == "" || PhoneNum.Text == "7810000000")
                {
                    PhoneNum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    PhoneNum.Foreground = new SolidColorBrush(Colors.Black);
                }
            }

        }



        private void Email_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Email.Text == "Email..." || Email.Text == "insert the Email!")
            {
                Email.Text = "";
                Email.Foreground = new SolidColorBrush(Colors.Black);

            }
            Email.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void Email_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email.Text))
            {
                Email.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                Email.Text = "Email...";
            }
        }

        private void Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Email.IsFocused)
            {
                if (Email.Text == "" || Email.Text == "Email..." || Email.Text == "insert the Email!")
                {
                    Email.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    Email.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void Username_MouseEnter(object sender, MouseEventArgs e)
        {
            Username.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));
        }



        private void Password_MouseEnter(object sender, MouseEventArgs e)
        {
            Password.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));
        }

        private void full_MouseEnter(object sender, MouseEventArgs e)
        {
            full.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));
        }


        private void full_MouseLeave(object sender, MouseEventArgs e)
        {
            full.Background = new SolidColorBrush(Colors.White);
        }

        private void Address_MouseEnter(object sender, MouseEventArgs e)
        {
            Address.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));
        }

        private void Address_MouseLeave(object sender, MouseEventArgs e)
        {
            Address.Background = new SolidColorBrush(Colors.White);
        }

        private void PhoneNum_MouseEnter(object sender, MouseEventArgs e)
        {
            PhoneNum.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));
        }

        private void PhoneNum_MouseLeave(object sender, MouseEventArgs e)
        {
            PhoneNum.Background = new SolidColorBrush(Colors.White);
        }

        private void role_MouseEnter(object sender, MouseEventArgs e)
        {
            role.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));
        }

        private void role_MouseLeave(object sender, MouseEventArgs e)
        {
            role.Background = new SolidColorBrush(Colors.White);
        }

        private void Email_MouseEnter(object sender, MouseEventArgs e)
        {
            Email.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));
        }

        private void Email_MouseLeave(object sender, MouseEventArgs e)
        {
            Email.Background = new SolidColorBrush(Colors.White);
        }

        private async void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard moveInStoryboard = (Storyboard)FindResource("MoveUpStoryboard");
            moveInStoryboard.Begin();
            await Task.Delay(300);
            Storyboard fadeInStoryboard = (Storyboard)FindResource("FadeOutStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            this.Visibility = Visibility.Collapsed;
        }
        private void StartGifAnimation()
        {
            // Run the GIF animation setup on a separate thread

            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-29-cross-hover-cross-3.gif"));
            ImageBehavior.SetAnimatedSource(avatar, image1);
            ImageBehavior.SetRepeatBehavior(avatar, System.Windows.Media.Animation.RepeatBehavior.Forever);



        }


        private async void avatar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            edit.Visibility = Visibility.Collapsed;
            verfication.Visibility = Visibility.Visible; 
            Storyboard moveInStoryboard = (Storyboard)FindResource("MoveUpStoryboard");
            moveInStoryboard.Begin();
            await Task.Delay(300);
            Storyboard fadeInStoryboard = (Storyboard)FindResource("FadeOutStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(1000);
            this.Visibility = Visibility.Collapsed;
        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }

        private void Password_GotFocus1(object sender, RoutedEventArgs e)
        {
            
        }

        private void Password_LostFocus1(object sender, RoutedEventArgs e)
        {

        }

        private void Password_MouseEnter1(object sender, MouseEventArgs e)
        {
            verfication1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF82BBB7"));
        }

        private void Password_MouseLeave1(object sender, MouseEventArgs e)
        {
            verfication1.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void Password_TextChanged1(object sender, TextChangedEventArgs e)
        {

            
        }

        private void mybutton_Clickver(object sender, RoutedEventArgs e)
        {
            query = "SELECT password FROM users_info WHERE id LIKE '" + Login_.iduser + "'";
            ds = conn.getData(query);

            // Check if the DataSet contains tables and if the first table has rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // If there are rows, retrieve the password
                string storedPassword = ds.Tables[0].Rows[0][0].ToString();

                // Verify the password entered by the user
                if (verfication1.Text == storedPassword)
                {
                    verfication1.Clear();
                    verfication.Visibility = Visibility.Collapsed;
                    edit.Visibility = Visibility.Visible;
                }
                else
                {
                    // Password does not match, apply the shaking animation
                    ApplyStoryboard(v); ApplyStoryboard(verfication1);
                }
            }
            else
            {
                // Handle the case where no user was found with the provided ID
                MessageBox.Show("Yout signed as a root Can't Edit info", "Verification Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private void ApplyStoryboard(UIElement targetElement)
        {
            Storyboard storyboard = (Storyboard)this.Resources["ShakeStoryboard"];
            Storyboard.SetTarget(storyboard, targetElement); // Set the target of the animation
            storyboard.Begin(); // Start the animation
        }
    }
}
