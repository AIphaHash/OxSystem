
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class filtersell : UserControl
    {
        private sellmedic _parent;
        public static string storagename;
        public static string mfilter;
        private string comboBoxText;
        public static string sn;
        private ICollectionView _ollectionview;
        private ICollectionView _collectionView;
        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public filtersell(sellmedic parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        public List<string> GetStorageNames()
        {
            List<string> storageNames = new List<string>();
             query = "select sname from storageinfo";
             ds = conn.getData(query);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    storageNames.Add(row["sname"].ToString());
                }
            }

            return storageNames;
        }
        public List<string> getmedicfilters()
        {
            List<string> medicfilters = new List<string>();
             query = "SELECT COLUMN_NAME \r\nFROM INFORMATION_SCHEMA.COLUMNS\r\nWHERE TABLE_NAME = 'medicinfo';\r\n";
             ds = conn.getData(query);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    medicfilters.Add(row["COLUMN_NAME"].ToString());
                }
            }

            return medicfilters;
        }

        private void sname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sname.SelectedItem != null)
            {
                storagename = sname.SelectedItem.ToString();
            }
            else
            {
                storagename = "No Storage Filter"; 
            }
        }

        private void sname_Loaded(object sender, RoutedEventArgs e)
        {
            _collectionView = CollectionViewSource.GetDefaultView(GetStorageNames());
            sname.ItemsSource = _collectionView;
        }

        private void sname_KeyUp(object sender, KeyEventArgs e)
        {
            if (_collectionView == null)
                return;

            _collectionView.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(sname.Text))
                    return true;

                return ((string)item).IndexOf(sname.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            _collectionView.Refresh();
            sname.IsDropDownOpen = true;
        }

        

        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            if (storagename != null)
            {
                _parent.storage.Content = storagename;
            }
            if (mfilter != null)
            {
                _parent.mfilter.Content = mfilter;
            }                               
            Storyboard fadeInStoryboard1 = (Storyboard)this.FindResource("FadeOutStoryboard1");
            fadeInStoryboard1.Begin();
            await Task.Delay(400);

            f.Visibility = Visibility.Collapsed;

        }

        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard fadeInStoryboard1 = (Storyboard)this.FindResource("FadeOutStoryboard1");
            fadeInStoryboard1.Begin();
            await Task.Delay(400);
            
            f.Visibility = Visibility.Collapsed;
        }
        private void StartGifAnimation()
        {
            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-29-cross-hover-cross-3 (2).gif"));
            ImageBehavior.SetAnimatedSource(avatar, image1);
            ImageBehavior.SetRepeatBehavior(avatar, System.Windows.Media.Animation.RepeatBehavior.Forever);

        }
        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
        }

        private void sname_KeyUp1(object sender, KeyEventArgs e)
        {
            if (_ollectionview == null)
                return;

            _ollectionview.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(sname_Copy.Text))
                    return true;

                return ((string)item).IndexOf(sname_Copy.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            _ollectionview.Refresh();
            sname_Copy.IsDropDownOpen = true;
        }

        private void sname_Loaded1(object sender, RoutedEventArgs e)
        {
            _ollectionview = CollectionViewSource.GetDefaultView(getmedicfilters());
            sname_Copy.ItemsSource = _ollectionview;
        }

        private void sname_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (sname_Copy.SelectedItem != null)
            {
                mfilter = sname_Copy.SelectedItem.ToString();
            }
            else
            {
                mfilter = "No Medic Filter"; 
            }
        }
    }
}
