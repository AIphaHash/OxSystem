using Org.BouncyCastle.Asn1.X509;
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

namespace OxSystem
{
    public partial class supplieradd : UserControl
    {
        string namesup;
        string locationsup;
        string numsup;
        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public supplieradd()
        {
            InitializeComponent();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.FindResource("FadeOutStoryboard");
            fadeOutStoryboard.Completed += (s, ev) => FadingBorder.Visibility = Visibility.Collapsed;
            fadeOutStoryboard.Begin(FadingBorder);
            await Task.Delay(1000);
            this.Visibility = Visibility.Collapsed;
        }
        private void mname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (supname.Text == "" || supname.Text == "Supplier Name..." || supname.Text == "insert the Medic Name!")
            {
                supname.Text = "";
                supname.Foreground = new SolidColorBrush(Colors.Black);

            }
            supname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF808080"));
        }
        private void mname_GotFocus1(object sender, RoutedEventArgs e)
        {
            if (supnum.Text == "" || supnum.Text == "Supplier Number..." || supnum.Text == "insert the Medic Name!")
            {
                supnum.Text = "";
                supnum.Foreground = new SolidColorBrush(Colors.Black);

            }
            supnum.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF808080"));
        }
        private void mname_GotFocus2(object sender, RoutedEventArgs e)
        {
            if (suplocation.Text == "" || suplocation.Text == "Supplier Location..." || suplocation.Text == "insert the Medic Name!")
            {
                suplocation.Text = "";
                suplocation.Foreground = new SolidColorBrush(Colors.Black);

            }
            suplocation.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF808080"));
        }

        private void mname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(supname.Text))
            {
                supname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                supname.Text = "Supplier Name...";
            }
        }

        private void mname_MouseEnter(object sender, MouseEventArgs e)
        {
            supname.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));

        }
        private void mname_MouseLeave(object sender, MouseEventArgs e)
        {
            supname.Background = null;
        }
        private void mname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (supname.IsFocused)
            {
                if (supname.Text == "" || supname.Text == "Supplier Name..." || supname.Text == "insert the Medic Name!")
                {
                    supname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    supname.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void mname_LostFocus1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(supnum.Text))
            {
                supnum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                supnum.Text = "Supplier Number...";
            }
        }
        private void mname_LostFocus2(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(suplocation.Text))
            {
                suplocation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                suplocation.Text = "Supplier Location...";
            }
        }

        private void mname_MouseEnter1(object sender, MouseEventArgs e)
        {
            supnum.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void mname_MouseLeave1(object sender, MouseEventArgs e)
        {
            supnum.Background = null;
        }

        private void mname_MouseEnter2(object sender, MouseEventArgs e)
        {
            suplocation.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void mname_MouseLeave2(object sender, MouseEventArgs e)
        {
            suplocation.Background = null;
        }

        private void mname_TextChanged1(object sender, TextChangedEventArgs e)
        {
            if (supnum.IsFocused)
            {
                if (supnum.Text == "" || supnum.Text == "Supplier Number..." || supnum.Text == "insert the Medic Name!")
                {
                    supnum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    supnum.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void mname_TextChanged2(object sender, TextChangedEventArgs e)
        {
            if (suplocation.IsFocused)
            {
                if (suplocation.Text == "" || suplocation.Text == "Supplier Location..." || suplocation.Text == "insert the Medic Name!")
                {
                    suplocation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    suplocation.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void mybutton_Click(object sender, RoutedEventArgs e)
        {
            namesup = supname.Text;
            numsup = supnum.Text;
            locationsup = suplocation.Text;
            query = "insert into Suppliers values ('" + namesup + "','" + numsup + "','" + locationsup + "')";
            conn.setData(query);
            reset();
        }
        public void reset()
        {
            supname.Text = "Supplier Name...";
            supnum.Text = "Supplier Number...";
            suplocation.Text = "Supplier Location...";
        }
    }
}
