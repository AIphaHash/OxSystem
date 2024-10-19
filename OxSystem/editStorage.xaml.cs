using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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

    public partial class editStorage : UserControl
    {
        public static string storagen;
        public static string storagel;
        public static string ssize;
        
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public editStorage()
        {
            InitializeComponent();
        }

        private void editborder_Loaded(object sender, RoutedEventArgs e)
        {
            sname.Text = storageadd.sn;
            slocation.Text = storageadd.sl;
            size.Text = storageadd.ss;
           
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

        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            storagen = sname.Text;
            storagel = slocation.Text;
            ssize = size.Text;
            
            if (sname.Text != "" && slocation.Text != "" && size.Text != "" && sname.Text != "Storage Name..." && slocation.Text != "Storage Location..." && size.Text != "9999" && sname.Text != "insert the Storage Name!" && slocation.Text != "insert the Storage Location!")
            {
                query = " update storageinfo set  sname = '" + storagen + "' , slocation = '" + storagel + "' , size = '" + ssize + "'   where dbid = '"+Properties.Settings.Default.dbid+"' and sname like '" + sname.Text + "' ";
                conn.setData(query);

                Storyboard moveInStoryboard = (Storyboard)FindResource("MoveUpStoryboard");
                moveInStoryboard.Begin();
                await Task.Delay(300);
                Storyboard fadeInStoryboard = (Storyboard)FindResource("FadeOutStoryboard");
                fadeInStoryboard.Begin();
                await Task.Delay(300);
                this.Visibility = Visibility.Collapsed;
            }
            if (sname.Text == "Storage Name..." || sname.Text == "insert the Storage Name!")
            {
                sname.BorderBrush = new SolidColorBrush(Colors.Red);
                sname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                sname.Text = "insert the Storage Name!";
            }
            if (slocation.Text == "Storage Location..." || slocation.Text == "insert the Storage Location!")
            {
                slocation.BorderBrush = new SolidColorBrush(Colors.Red);
                slocation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                slocation.Text = "insert the Storage Location!";
            }
            if (size.Text == "9999")
            {
                size.BorderBrush = new SolidColorBrush(Colors.Red);
                size.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                size.Text = "9999";
            }
            

        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (slocation.IsFocused)
            {
                if (slocation.Text == "" || slocation.Text == "Storage Location..." || slocation.Text == "insert the Storage Location!")
                {
                    slocation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    slocation.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void Password_MouseLeave(object sender, MouseEventArgs e)
        {
            slocation.Background = new SolidColorBrush(Colors.White);
        }

        private void Password_MouseEnter(object sender, MouseEventArgs e)
        {
            slocation.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));

        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(slocation.Text))
            {
                slocation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                slocation.Text = "Storage Location...";
            }
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            if (slocation.Text == "Storage Location..." || slocation.Text == "insert the Storage Location!")
            {
                slocation.Text = "";
                slocation.Foreground = new SolidColorBrush(Colors.Black);

            }
            slocation.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        

        private void Username_MouseEnter(object sender, MouseEventArgs e)
        {
            sname.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));

        }

        private void Username_MouseLeave(object sender, MouseEventArgs e)
        {
            sname.Background = new SolidColorBrush(Colors.White);
        }

        private void full_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (size.IsFocused)
            {
                if (size.Text == "" || size.Text == "9999")
                {
                    size.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    size.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void full_MouseLeave(object sender, MouseEventArgs e)
        {
            size.Background = new SolidColorBrush(Colors.White);
        }

        private void full_MouseEnter(object sender, MouseEventArgs e)
        {
            size.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));

        }

        private void full_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(size.Text))
            {
                size.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                size.Text = "9999";
            }
        }

        private void full_GotFocus(object sender, RoutedEventArgs e)
        {
            if (size.Text == "9999" || size.Text == "9999")
            {
                size.Text = "";
                size.Foreground = new SolidColorBrush(Colors.Black);

            }
            size.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void sname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sname.Text == "Storage Name..." || sname.Text == "insert the Storage Name!")
            {
                sname.Text = "";
                sname.Foreground = new SolidColorBrush(Colors.Black);

            }
            sname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void sname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sname.Text))
            {
                sname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                sname.Text = "Storage Name...";
            }
        }

        private void sname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sname.IsFocused)
            {
                if (sname.Text == "" || sname.Text == "Storage Name..." || sname.Text == "insert the Storage Name!")
                {
                    sname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    sname.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }
    }
}
