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

    public partial class editmedic : UserControl
    {
        public static string medicn;
        public static string buyp;
        public static string sellp;
        public static string expired;
        public static string manud;
        public static string numberm;
        public static string storagen;
        public static string storagename;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public editmedic()
        {
            InitializeComponent();
            LoadStorageNames();
            int currentYear = DateTime.Now.Year;          
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

        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            if (Username.Text != "" && Username.Text != "insert the Medic Name!" && Username.Text != "Medic Name..." && Password.Text != "" && Password.Text != "Num of Medic..." && Password.Text != "insert the Num of Medic!" && Address.Text != ""  && full.Text != ""  && Address.Text != "9999"  && full.Text != "9999")
            {
                query = " update medicinfo set  nummedic = '"+Password.Text+"', bprice = '" + full.Text + "' , sprice = '" + Address.Text + "'  , sname = '"+storagename+"'  where mname = '" + Username.Text + "' ";
                conn.setData(query);

                Storyboard moveInStoryboard = (Storyboard)FindResource("MoveUpStoryboard");
                moveInStoryboard.Begin();
                await Task.Delay(300);
                Storyboard fadeInStoryboard = (Storyboard)FindResource("FadeOutStoryboard");
                fadeInStoryboard.Begin();
                await Task.Delay(300);
                this.Visibility = Visibility.Collapsed;
            }

            if (Username.Text == "Medic Name..." || Username.Text == "insert the Medic Name!")
            {
                Username.BorderBrush = new SolidColorBrush(Colors.Red);
                Username.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Username.Text = "insert the Medic Name!";
                ApplyStoryboard(Username);
            }
            if (Password.Text == "Num of Medic..." || Password.Text == "insert the Num of Medic!")
            {
                Password.BorderBrush = new SolidColorBrush(Colors.Red);
                Password.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Password.Text = "insert the Num of Medic!";
                ApplyStoryboard(Password);
            }
            
            if (full.Text == "9999")
            {
                full.BorderBrush = new SolidColorBrush(Colors.Red);
                full.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                full.Text = "9999";
                ApplyStoryboard(full);
            }
            if (Address.Text == "9999")
            {
                Address.BorderBrush = new SolidColorBrush(Colors.Red);
                Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C5C5"));
                Address.Text = "9999";
                ApplyStoryboard(Address);
            }
        }
        private void editborder_Loaded(object sender, RoutedEventArgs e)
        {
            Username.Text = medic_num.medicn;
            Password.Text = medic_num.numberm;
            Address.Text = medic_num.buyp;
            full.Text = medic_num.sellp;
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
            Password.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2B9396"));

            if (Password.Text == "Num of Medic..." || Password.Text == "insert the Num of Medic!")
            {
                Password.Text = "";
                Password.Foreground = new SolidColorBrush(Colors.Black);

            }
            Password.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            Password.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));

            if (string.IsNullOrWhiteSpace(Password.Text))
            {
                Password.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                Password.Text = "Num of Medic...";
            }
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Password.IsFocused)
            {
                if (Password.Text == "" || Password.Text == "Num of Medic..." || Password.Text == "insert the Num of Medic!")
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
            full.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2B9396"));

            if (full.Text == "9999" || full.Text =="")
            {
                full.Text = "";
                full.Foreground = new SolidColorBrush(Colors.Black);

            }
            full.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }

        private void full_LostFocus(object sender, RoutedEventArgs e)
        {
            full.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));

            if (string.IsNullOrWhiteSpace(full.Text))
            {
                full.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                full.Text = "9999";
            }
        }

        private void full_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (full.IsFocused)
            {
                if (full.Text == "" || full.Text == "9999" )
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
                if (Address.Text == "" || Address.Text == "9999" )
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
        {Address.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
            if (string.IsNullOrWhiteSpace(Address.Text))
            {
                Address.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                Address.Text = "9999";
            }
        }

        private void Address_GotFocus(object sender, RoutedEventArgs e)
        {Address.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2B9396"));
            if (Address.Text == "" || Address.Text == "9999")
            {
                Address.Text = "";
                Address.Foreground = new SolidColorBrush(Colors.Black);

            }
            Address.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF94A6B3"));
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             storagename = sname.SelectedItem.ToString();
        }
        public List<string> GetStorageNames()
        {
            List<string> storageNames = new List<string>();
            string query = "select sname from storageinfo";
            DataSet ds = conn.getData(query);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    storageNames.Add(row["sname"].ToString());
                }
            }

            return storageNames;
        }
        private void LoadStorageNames()
        {
            List<string> storageNames = GetStorageNames();
            sname.ItemsSource = storageNames;
        }

        private void sname_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStorageNames();
        }

    }
}
