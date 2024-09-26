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


        public static class LanguageManager
        {
            public static Dictionary<string, string> ArabicTexts = new Dictionary<string, string>
            {
        { "Greeting", "مرحبا" },
        { "Exit", "خروج" },
        { "World is big", "العالم كبير" }
        
            };
    
            public static Dictionary<string, string> EnglishTexts = new Dictionary<string, string>
            {
        { "Greeting", "Hello" },
        { "Exit", "Exit" },
        { "World is big", "The world is big" }
        
            };

            public static bool IsEnglish { get; set; } = false; // Default to Arabic (false)

            public static Dictionary<string, string> CurrentTexts
            {
                get
                {
                    return IsEnglish ? EnglishTexts : ArabicTexts;
                }
            }
        }















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
            reme = Properties.Settings.Default.reme;
            

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        
        

        

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {


        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Image_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            passwordreset.Visibility = Visibility.Visible;

            Storyboard popUpStoryboard = (Storyboard)FindResource("PopUpStoryboard1");
            popUpStoryboard.Begin();
        }

        private void mybutton_Click(object sender, RoutedEventArgs e)
        {

            dbname = pharmName.Text;
            dbpass = pharmName_Copy.Text;
            LinearGradientBrush gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFAB1A1A"), 0));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF44ABAE"), 1));

            LinearGradientBrush gradientBrush1 = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            gradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF1EA797"), 0));
            gradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF44ABAE"), 1));

            if (string.IsNullOrEmpty(dbname) || string.IsNullOrEmpty(dbpass))
            {
                

                pharmName.BorderBrush = gradientBrush;
                pharmName_Copy.BorderBrush = gradientBrush;



                return;
            }
            else
            {
                // Query to get all database names
                query = "SELECT name FROM sys.databases;";
                ds = conn.getData_(query);

                // Check if the entered dbname exists in the list of databases
                bool dbExists = false;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["name"].ToString().Equals(dbname, StringComparison.OrdinalIgnoreCase))
                    {
                        dbExists = true;
                        break;
                    }
                }

                // Provide feedback based on whether the db exists or not
                /*if (dbExists && pharmName_Copy.Text == Properties.Settings.Default.databasepassword && pharmName.Text == Properties.Settings.Default.databasename)*/
                if(dbExists)
                {
                    pharmName.BorderBrush = gradientBrush1;
                    pharmName_Copy.BorderBrush = gradientBrush1;
                    finaldbname = dbname;
                    Login_ l = new Login_();
                    l.Show();
                    this.Close();
                }
                else
                {
                    pharmName.BorderBrush = gradientBrush;
                    pharmName_Copy.BorderBrush = gradientBrush;
                    MessageBox.Show("Wrong info");
                }
            }

           
        }

        private void mybutton_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void mybutton_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            createdb.Visibility = Visibility.Visible;

            Storyboard popUpStoryboard = (Storyboard)FindResource("PopUpStoryboard");
            popUpStoryboard.Begin();
        }

        private void labelShadow1_Loaded(object sender, RoutedEventArgs e)
        {
           if (Properties.Settings.Default.reme == "1")
            {
                RememberMeCheckBox.IsChecked = true;    
            }
            else
            {
                RememberMeCheckBox.IsChecked= false;
            }            

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
                pharmName.Text = Properties.Settings.Default.databasename;
                pharmName_Copy.Text = Properties.Settings.Default.databasepassword;
            }
            if (Properties.Settings.Default.firstlogin == "1")
            {
                pharmName.IsEnabled = true;
                pharmName_Copy.IsEnabled = true;
                mybutton.IsEnabled = true;
            }
            else
            {
                pharmName.IsEnabled = false;
                pharmName_Copy.IsEnabled = false;
                mybutton.IsEnabled = false;
            }
            
        }

        private void pharmName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pharmName.Text == "Database Name...")
            {
                pharmName.Text = "";
                pharmName.Foreground = new SolidColorBrush(Colors.Black);

            }
            else if (pharmName.Text == "")
            {
                pharmName.Text = "";
                pharmName.Foreground = new SolidColorBrush(Colors.Black);

            }
            pharmName.Foreground = new SolidColorBrush(Colors.Black);
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
            if (pharmName.Text == "Database Name...")
            {
                pharmName.Text = "Database Name...";
                pharmName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA0A0A0"));

            }
            else if (pharmName.Text == "")
            {
                pharmName.Text = "Database Name...";
                pharmName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA0A0A0"));

            }
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
            pharmName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDCDCDC"));
        }

        private void pharmName_MouseLeave(object sender, MouseEventArgs e)
        {
            pharmName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        }

        private void pharmName_Copy_MouseLeave(object sender, MouseEventArgs e)
        {
            pharmName_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        }

        private void RememberMeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RememberMeLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1EA797"));
            RememberMeCheckBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1EA797"));

            if (pharmName.Text == Properties.Settings.Default.databasename && pharmName_Copy.Text == Properties.Settings.Default.databasepassword)
            {
                Properties.Settings.Default.reme = "1";
                Properties.Settings.Default.Save();
            }
            
        }

        private void RememberMeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RememberMeLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB3B3B3"));
            RememberMeCheckBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB3B3B3"));
            Properties.Settings.Default.reme = "0";
            Properties.Settings.Default.Save();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void RememberMeLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RememberMeCheckBox.IsChecked == false)
            {
                RememberMeCheckBox.IsChecked = true;
                if (pharmName.Text == Properties.Settings.Default.databasename && pharmName_Copy.Text == Properties.Settings.Default.databasepassword)
                {
                    Properties.Settings.Default.reme = "1";
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                RememberMeCheckBox.IsChecked = false;
                Properties.Settings.Default.reme = "0";
            Properties.Settings.Default.Save();
                
            }
        }
    }
}
