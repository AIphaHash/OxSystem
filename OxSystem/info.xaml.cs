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
using OxSystem;
using Syncfusion.Windows.Tools.Controls;


namespace OxSystem
{
    public partial class info : UserControl
    {        
        public static string perms = string.Empty;
        string query;
        DbConnection conn_ = new DbConnection();
        DataSet ds;
        public static string role;
        public info()
        {
            InitializeComponent();
            InitializeDatePicker();
            chkViewMedic.Checked += CheckBox_Checked;
            chkViewMedic.Unchecked += CheckBox_Unchecked;
            makereport.Checked += CheckBox_Checked;
            makereport.Unchecked += CheckBox_Unchecked;
            chkAddMedic.Checked += CheckBox_Checked;
            chkAddMedic.Unchecked += CheckBox_Unchecked;
            chkSellMedic.Checked += CheckBox_Checked;
            chkSellMedic.Unchecked += CheckBox_Unchecked;
            chkSellMedicDuplicate.Checked += CheckBox_Checked;
            chkSellMedicDuplicate.Unchecked += CheckBox_Unchecked;
            all.Checked += CheckBox_Checked;
            all.Unchecked += CheckBox_Unchecked;
        }

        private void InitializeDatePicker()
        {
            int currentYear = DateTime.Now.Year;
            for (int year = 1900; year <= currentYear; year++)
            {
                YearComboBox.Items.Add(year);
            }
            for (int month = 1; month <= 12; month++)
            {
                MonthComboBox.Items.Add(month);
            }
            YearComboBox.SelectedItem = DateTime.Now.Year;
            MonthComboBox.SelectedItem = DateTime.Now.Month;
            PopulateDays();
            DayComboBox.SelectedItem = DateTime.Now.Day;
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string permission = checkBox.Content.ToString().ToLower().Replace(" ", string.Empty); // Convert to lowercase and remove spaces

            if (!perms.Contains(permission))
            {
                if (string.IsNullOrEmpty(perms))
                {
                    perms = permission;
                }
                else
                {
                    perms += $",{permission}";
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string permission = checkBox.Content.ToString().ToLower().Replace(" ", string.Empty);

            if (perms.Contains(permission))
            {
                var permissionsList = perms.Split(',').ToList();
                permissionsList.Remove(permission);
                perms = string.Join(",", permissionsList);
            }
            Console.WriteLine($"Unchecked: {perms}");
        }
        private void PopulateDays()
        {
            if (YearComboBox.SelectedItem == null || MonthComboBox.SelectedItem == null)
                return;
            int year = (int)YearComboBox.SelectedItem;
            int month = (int)MonthComboBox.SelectedItem;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DayComboBox.Items.Clear();
            for (int day = 1; day <= daysInMonth; day++)
            {
                DayComboBox.Items.Add(day);
            }
        }
        public string role_ = Addacount.role_add;
        public void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Addacount ad = new Addacount();
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
            Username.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
            Username.Text = "Username...";
            Passswrod.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
            Passswrod.Text = "Password...";
            Email.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
            Email.Text = "Email...";
            Fullname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
            Fullname.Text = "Fullname...";
            Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
            Address.Text = "Address...";
            PhoneNum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
            PhoneNum.Text = "0780000000";



        }
        private bool IsPlaceholder(string value, string placeholder)
        {
            return value == placeholder || value == $"insert the {placeholder}!";
        }


        public void ApplyStoryboard(TextBox textBox)
        {
            var storyboard = (Storyboard)this.Resources["ShakeAndRedBorderStoryboard"];
            if (textBox.RenderTransform == null || !(textBox.RenderTransform is TransformGroup))
            {
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform());
                textBox.RenderTransform = transformGroup;
            }
            Storyboard.SetTarget(storyboard, textBox);
            storyboard.Begin();
        }
       

        private void reset_()
        {
          
            Username.Clear();
            Passswrod.Clear();
            Fullname.Clear(); 
            Email.Clear();
            PhoneNum.Clear();
            Address.Clear();
            YearComboBox.Items.Refresh();
            MonthComboBox.Items.Refresh();
            DayComboBox.Items.Refresh();
         
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDays();
        }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDays();
        }
        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            Username.Foreground = new SolidColorBrush((Color)Colors.Black);
            Username.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1DB8AC"));
            
            if (Username.Text == "Username..." || Username.Text == "" || Username.Text == "insert the Username!")
            {
                Username.Text = "";
            }
            
        }
        private void Username_LostFocus(object sender, RoutedEventArgs e)
        {
            
            Username.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
            if (Username.Text == "")
            {
                Username.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Username.Text = "Username...";
            }
        }

        private void Passswrod_LostFocus(object sender, RoutedEventArgs e)
        {
            Passswrod.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
            if (Passswrod.Text == "")
            {
                Passswrod.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Passswrod.Text = "Password...";
            }
        }

        private void Fullname_LostFocus(object sender, RoutedEventArgs e)
        {
           
            Fullname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
            if (Fullname.Text == "")
            {Fullname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Fullname.Text = "Fullname...";
            }
        }

        private void PhoneNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneNum.Text, "^[0-9]+$"))
            {
                PhoneNum.Text = "";
            }
        }

        private void PhoneNum_LostFocus(object sender, RoutedEventArgs e)
        {
            PhoneNum.Background = new SolidColorBrush((Color)Colors.White);
            PhoneNum.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
            if (PhoneNum.Text == "")
            {
                PhoneNum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                PhoneNum.Text = "0780000000";
            }
        }

        private void Email_LostFocus(object sender, RoutedEventArgs e)
        {
            Email.Background = new SolidColorBrush((Color)Colors.White);
            Email.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
            if (Email.Text == "")
            {Email.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Email.Text = "Email...";
            }
        }

        private void Address_LostFocus(object sender, RoutedEventArgs e)
        {
            Address.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
            if (Address.Text == "")
            {
                Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Address.Text = "Address...";
            }
        }

        private void Passswrod_GotFocus(object sender, RoutedEventArgs e)

        {
            Passswrod.Foreground = new SolidColorBrush((Color)Colors.Black);

            Passswrod.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1DB8AC"));
            if (Passswrod.Text == "Password..." || Passswrod.Text == "" || Passswrod.Text == "insert the Password!")
            {
                Passswrod.Text = "";
            }
        }

        private void Fullname_GotFocus(object sender, RoutedEventArgs e)
        {
            Fullname.Foreground = new SolidColorBrush((Color)Colors.Black);
            if (Fullname.Text == "Fullname..." || Fullname.Text == "" || Fullname.Text == "insert the Fullname!")
            {
                Fullname.Text = "";
            }

            Fullname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1DB8AC"));

        }

        private void Email_GotFocus(object sender, RoutedEventArgs e)
        {

            Email.Foreground = new SolidColorBrush((Color)Colors.Black);
            if (Email.Text == "Email..." || Email.Text == "" || Email.Text == "insert the Email!") 
            {
                Email.Text = "";
            }

            Email.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1DB8AC"));

        }

        private void PhoneNum_GotFocus(object sender, RoutedEventArgs e)
        {

            if (PhoneNum.Text == "0780000000" || PhoneNum.Text == "")
            {
                PhoneNum.Text = "";
            }

            PhoneNum.Foreground = new SolidColorBrush((Color)Colors.Black);
            PhoneNum.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1DB8AC"));

        }

        private void Address_GotFocus(object sender, RoutedEventArgs e)
        {
            
            Address.Foreground = new SolidColorBrush((Color)Colors.Black);
            if (Address.Text == "Address..." || Address.Text == "" || Address.Text == "insert the Address!")
            {
                Address.Text = "";
            }

            Address.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1DB8AC"));

        }
        private void Fullname_MouseEnter(object sender, MouseEventArgs e)
        {
            
            Fullname.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B7986"));
        }

        private void Fullname_MouseLeave(object sender, MouseEventArgs e)
        {
            Fullname.Background = new SolidColorBrush((Color)Colors.White);
        }

        private void PhoneNum_MouseEnter(object sender, MouseEventArgs e)
        {
            PhoneNum.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B7986"));

        }

        private void PhoneNum_MouseLeave(object sender, MouseEventArgs e)
        {
            PhoneNum.Background = new SolidColorBrush((Color)Colors.White);
        }
        private void Email_MouseEnter(object sender, MouseEventArgs e)
        {
            Email.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B7986"));
        }

        private void Email_MouseLeave(object sender, MouseEventArgs e)
        {
            Email.Background = new SolidColorBrush((Color)Colors.White);

        }

        private void Address_MouseEnter(object sender, MouseEventArgs e)
        {
            Address.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B7986"));
        }

        private void Address_MouseLeave(object sender, MouseEventArgs e)
        {
            Address.Background = new SolidColorBrush((Color)Colors.White);
        }

        private void Username_MouseEnter(object sender, MouseEventArgs e)
        {
            Username.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B7986"));
        }

        private void Username_MouseLeave(object sender, MouseEventArgs e)
        {
            Username.Background = new SolidColorBrush((Color)Colors.White);
        }

        private void Passswrod_MouseEnter(object sender, MouseEventArgs e)
        {
            Passswrod.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B7986"));
        }

        private void Passswrod_MouseLeave(object sender, MouseEventArgs e)
        {
            Passswrod.Background = new SolidColorBrush((Color)Colors.White);
        }
        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and user_name = '"+Username.Text+"'";
            ds = conn_.getData(query);
            if (ds.Tables[0].Rows.Count == 0)
            {
                usererror.Visibility = Visibility.Collapsed;
                pdf.IsEnabled = true;
            }
            else
            {
                usererror.Visibility = Visibility.Visible;
                pdf.IsEnabled = false;
                ApplyStoryboard(Username);

            }
        }

        private async void allinfo_MouseEnter(object sender, MouseEventArgs e)
        {
            await Task.Delay(1500);
        }



        private void StartGifAnimation()
        {
            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-21-avatar-hover-looking-around.gif"));
            ImageBehavior.SetAnimatedSource(avatar, image1);
            ImageBehavior.SetRepeatBehavior(avatar, System.Windows.Media.Animation.RepeatBehavior.Forever);

            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-12-arrow-down-hover-arrow-down-1 (1).gif"));
            ImageBehavior.SetAnimatedSource(back, image2);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);
           
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }

        private async void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var parent = FindParent<Addacount>(this);

            if (parent != null)
            {
               parent.back_MouseLeftButtonDown(sender, e);
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            while (parentObject != null)
            {
                if (parentObject is T parent)
                {
                    return parent;
                }

                parentObject = VisualTreeHelper.GetParent(parentObject);
            }

            return null;
        
    }

        

        private async void pdf_Click(object sender, RoutedEventArgs e)
        {
            
            string username = Username.Text;
            string password = Passswrod.Text;
            string fullname = Fullname.Text;
            string email = Email.Text;
            string phonenum = PhoneNum.Text;
            string address = Address.Text;
            string userrole = rolee.Text;

            if (Username.Text == "Username..." || Username.Text == "insert the Username!" || Username.Text == "")
            {
                Username.BorderBrush = new SolidColorBrush(Colors.Red);
                Username.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Username.Text = "insert the Username!";
                ApplyStoryboard(Username);
            }
            if (Passswrod.Text == "Password..." || Passswrod.Text == "insert the Password!" || Passswrod.Text == "")
            {
                Passswrod.BorderBrush = new SolidColorBrush(Colors.Red);
                Passswrod.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Passswrod.Text = "insert the Password!";
                ApplyStoryboard(Passswrod);
            }
            if (Email.Text == "Email..." || Email.Text == "insert the Email!" || Email.Text == "")
            {
                Email.BorderBrush = new SolidColorBrush(Colors.Red);
                Email.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Email.Text = "insert the Email!";
                ApplyStoryboard(Email);
            }
            if (Fullname.Text == "Fullname..." || Fullname.Text == "insert the Fullname!" || Fullname.Text == "")
            {
                Fullname.BorderBrush = new SolidColorBrush(Colors.Red);
                Fullname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Fullname.Text = "insert the Fullname!";
                ApplyStoryboard(Fullname);
            }
            if (Address.Text == "Address..." || Address.Text == "insert the Address!" || Address.Text == "")
            {
                Address.BorderBrush = new SolidColorBrush(Colors.Red);
                Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Address.Text = "insert the Address!";
                ApplyStoryboard(Address);
            }
            if (PhoneNum.Text == "0780000000" || PhoneNum.Text == "")
            {
                PhoneNum.BorderBrush = new SolidColorBrush(Colors.Red);
                PhoneNum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                PhoneNum.Text = "0780000000";
                ApplyStoryboard(PhoneNum);
            }
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password) &&
                !string.IsNullOrWhiteSpace(fullname) && !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(phonenum) && !string.IsNullOrWhiteSpace(address) &&
                !string.IsNullOrWhiteSpace(userrole) && YearComboBox.SelectedItem != null &&
                MonthComboBox.SelectedItem != null && DayComboBox.SelectedItem != null &&
                !IsPlaceholder(username, "Username...") && !IsPlaceholder(password, "Password...") &&
                !IsPlaceholder(fullname, "Fullname...") && !IsPlaceholder(email, "Email...") &&
                !IsPlaceholder(phonenum, "0780000000") && !IsPlaceholder(address, "Address...") && !IsPlaceholder(username, "insert the Username!") && !IsPlaceholder(password, "insert the Password!") &&
                !IsPlaceholder(fullname, "insert the Fullname!") && !IsPlaceholder(email, "insert the Email!") &&
                  !IsPlaceholder(address, "insert the Address!") && YearComboBox.SelectedIndex >= 0 && MonthComboBox.SelectedIndex >= 0 && DayComboBox.SelectedIndex >= 0)
            {
                int year = (int)YearComboBox.SelectedItem;
                int month = (int)MonthComboBox.SelectedItem;
                int day = (int)DayComboBox.SelectedItem;
                string birthdate = new DateTime(year, month, day).ToString("yyyy-MM-dd");

                string query = "INSERT INTO users_info (user_name, password, role, email, phone_num, address, dob, fullname, perms, dbid) " +
                               $"VALUES ('{username}', '{password}', '{userrole}', '{email}', '{phonenum}', '{address}', '{birthdate}', '{fullname}','{perms}' ,'"+Properties.Settings.Default.dbid+"');";

                conn_.setData(query);
                MessageBox.Show("Created Succesfuly");
                reset_();
            }
           
        }

        private void pdf_MouseEnter(object sender, MouseEventArgs e)
        {
          
            pdf.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006B6D"));
        }

        private void pdf_MouseLeave(object sender, MouseEventArgs e)
        {
          
            pdf.Foreground = new SolidColorBrush(Colors.White);
        }

        private void AnimateBorder(Border targetBorder)
        {
            if (targetBorder != null)
            {
                Storyboard storyboard = (Storyboard)this.Resources["ClickEffectStoryboard"];
                Storyboard.SetTarget(storyboard, targetBorder);
                storyboard.Begin();
            }
        }
    }
}
