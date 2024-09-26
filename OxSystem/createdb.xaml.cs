using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections;
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

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for createdb.xaml
    /// </summary>
    public partial class createdb : UserControl
    {
        string query;
        DataSet ds;
        public static string finaldbname;
        public static string dbpass;
        public static string dbname;
        DbConnection conn = new DbConnection();
        public createdb()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];

            fadeOutStoryboard.Begin();
            await Task.Delay(1100);
            
            this.Visibility = Visibility.Collapsed;
            fadeElement.Opacity = 1;

        }

       

        private void mybutton_Click(object sender, RoutedEventArgs e)
        {
            dbname = pharmName.Text;
            dbpass = pharmName_Copy.Text;

            if (string.IsNullOrEmpty(dbname) || string.IsNullOrEmpty(dbpass))
            {
                pharmName.BorderBrush = new SolidColorBrush(Colors.Red);
                pharmName_Copy.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                query = "SELECT name FROM sys.databases;";
                ds = conn.getData(query);

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
                if (dbExists)
                {
                    MessageBox.Show("Use another name.");
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show($"Database '{dbname}' does not exist. Would you like to create it?", "Database Not Found", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Properties.Settings.Default.firstlogin = "1";
                        Properties.Settings.Default.databasename = pharmName.Text;
                        Properties.Settings.Default.databasepassword = pharmName_Copy.Text; 
                        Properties.Settings.Default.Save();
                        // Create the database
                        conn.CreateDatabase(dbname);

                        pharmName.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1EA797"));
                        pharmName_Copy.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1EA797"));
                        
                    }
                }


                
            }

           
        }

      

        private async void Image_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];

            fadeOutStoryboard.Begin();
            await Task.Delay(1100);
            
            
            fadeElement.Opacity = 1;
            CREATEDB.Visibility = Visibility.Collapsed;
            CREATEDB.Opacity = 0;
            _lock.Opacity = 1;
            _lock.Visibility = Visibility.Visible;
            this.Visibility = Visibility.Collapsed;

        }

        private void fadeElement_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private async void mybutton_Click1(object sender, RoutedEventArgs e)
        {
            if (pharmName_Copy1.Text == "ameer")
            {
                // Show CREATEDB and hide _lock with fade in and out effect
                CREATEDB.Visibility = Visibility.Visible;
                _lock.Visibility = Visibility.Visible; // Ensure _lock is visible so the fade-out works

                // Start the storyboard
                Storyboard storyboard = (Storyboard)FindResource("FadeInOutStoryboard");
                storyboard.Begin();

                _lock.Visibility = Visibility.Collapsed;
                pharmName_Copy1.Clear();



                // Hide _lock after the fade-out animation is complete

            }
            else
            {
                pharmName_Copy1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD87979"));
                Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeAnimation"];
                shakeStoryboard.Begin(pharmName_Copy1);
            }

        }

        private void pharmName_Copy1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pharmName_Copy1.Text == "Server Password...")
            {
                pharmName_Copy1.Text = "";
                pharmName_Copy1.Foreground = new SolidColorBrush(Colors.Black);

            }
            else if (pharmName_Copy1.Text == "")
            {
                pharmName_Copy1.Text = "";
                pharmName_Copy1.Foreground = new SolidColorBrush(Colors.Black);

            }
            pharmName_Copy1.Foreground = new SolidColorBrush(Colors.Black);
        }

       

       

        private void pharmName_Copy1_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if (pharmName_Copy1.Text == "")
            {
                pharmName_Copy1.Text = "Server Password...";
                pharmName_Copy1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA4A4A4"));

            }
            pharmName_Copy1.Foreground = new SolidColorBrush(Colors.Black);

        }

        private void pharmName_Copy1_MouseEnter_1(object sender, MouseEventArgs e)
        {
            pharmName_Copy1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDCDCDC"));
        }

        private void pharmName_Copy1_MouseLeave_1(object sender, MouseEventArgs e)
        {
            pharmName_Copy1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        }

        private void pharmName_MouseEnter(object sender, MouseEventArgs e)
        {
            pharmName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDCDCDC"));
        }

        private void pharmName_MouseLeave(object sender, MouseEventArgs e)
        {
            pharmName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        }

        private void pharmName_Copy_MouseEnter(object sender, MouseEventArgs e)
        {
            pharmName_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDCDCDC"));
        }

        private void pharmName_Copy_MouseLeave(object sender, MouseEventArgs e)
        {
            pharmName_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        }
    }
}
