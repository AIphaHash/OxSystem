



using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
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

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {


















        private bool isPasswordVisible = false;

        public static string reme;
        public static string dbpass;
        public static string finaldbname;
        public static string dbname;
        public string hc_id;
        public string HC_name;
        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();

        public Login()
        {
            InitializeComponent();


        }














        private void Image_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            createdb.Visibility = Visibility.Visible;

            Storyboard popUpStoryboard = (Storyboard)FindResource("PopUpStoryboard");
            popUpStoryboard.Begin();
        }

        private void labelShadow1_Loaded(object sender, RoutedEventArgs e)
        {


            if (reme == "1")
            {
                Properties.Settings.Default.rememberme = "1";

            }
            else
            {
                Properties.Settings.Default.rememberme = "0";
            }


            if (Properties.Settings.Default.rememberme == "1")
            {
                pharmName_Copy.Text = Properties.Settings.Default.databasepassword;
            }
            if (Properties.Settings.Default.firstlogin == "1")
            {
                pharmName_Copy.IsEnabled = true;
                mybutton.IsEnabled = true;
            }
            else
            {
                pharmName_Copy.IsEnabled = false;
                mybutton.IsEnabled = false;
            }

        }

        private void pharmName_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void pharmName_Copy_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pharmName_Copy.Text == "Database Password...")
            {
                pharmName_Copy.Text = "";
                pharmName_Copy.Foreground = new SolidColorBrush(Colors.Black);

            }
            else if (pharmName_Copy.Text == "")
            {
                pharmName_Copy.Text = "";
                pharmName_Copy.Foreground = new SolidColorBrush(Colors.Black);

            }
            pharmName_Copy.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void pharmName_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void pharmName_Copy_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pharmName_Copy.Text == "Database Password...")
            {
                pharmName_Copy.Text = "Database Password...";
                pharmName_Copy.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA0A0A0"));

            }
            else if (pharmName_Copy.Text == "")
            {
                pharmName_Copy.Text = "Database Password...";
                pharmName_Copy.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA0A0A0"));

            }
        }

        private void pharmName_Copy_MouseEnter(object sender, MouseEventArgs e)
        {
            pharmName_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDCDCDC"));
        }

        private void pharmName_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void pharmName_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void pharmName_Copy_MouseLeave(object sender, MouseEventArgs e)
        {
            pharmName_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        }

        private void RememberMeCheckBox_Checked(object sender, RoutedEventArgs e)
        {


        }

        private void RememberMeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.reme = "0";
            Properties.Settings.Default.Save();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void RememberMeLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
