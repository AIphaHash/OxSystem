using Syncfusion.UI.Xaml.Charts;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace OxSystem
{
    public partial class Addacount : UserControl, INotifyPropertyChanged
    {
        public static string role_add;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        private string _currentTime;
        public string CurrentUserId = Login_.iduser;



        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }

        private DispatcherTimer _timer;
        public static string full = Login_.fullName;

        public Addacount()
        {
            InitializeComponent();
            query = "SELECT role FROM users_info WHERE dbid = '"+Properties.Settings.Default.dbid+"' and id = '" + CurrentUserId + "'";
            ds = conn.getData(query);

            // Check if the DataSet contains tables and if the first table has rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // If there are rows, set the control values
                // Role
            }
            else
            {

                add_Copy.Visibility = Visibility.Collapsed;
                add_Copy1.Visibility = Visibility.Collapsed;

            }
            DataContext = this;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            UpdateCurrentTime();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            fullnameLabel.Content = full;
        }
        private async void pharm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimateBorder(pharm);
            await Task.Delay(200);
            Storyboard storyboard = (Storyboard)addaccount.FindResource("MoveDownStoryboard");
            storyboard.Begin(AnimatedBorder);
            role_add = "Pharm";
            info.pdf.Content = "Create Pharmacist";
            info.Visibility = Visibility.Visible;
            info.pPerm.Visibility = Visibility.Visible;
            info.adminPerm.Visibility = Visibility.Collapsed;
            info.rolee.Text = role_add;
            add.Visibility = Visibility.Collapsed;

        }
        private async void admin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimateBorder(admin);
            await Task.Delay(200);
            Storyboard storyboard1 = (Storyboard)this.Resources["ClickEffectStoryboard"];
            storyboard1.Begin();
            Storyboard storyboard = (Storyboard)addaccount.FindResource("MoveDownStoryboard");
            storyboard.Begin(AnimatedBorder);
            role_add = "Admin";
            info.pdf.Content = "Create Admin";
            info.Visibility = Visibility.Visible;
            info.pPerm.Visibility = Visibility.Collapsed;
            info.adminPerm.Visibility = Visibility.Visible;
            info.rolee.Text = role_add;
            add.Visibility = Visibility.Collapsed;
        }
        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and role = 'Admin'";
            ds = conn.getData(query);
            adminnum.Text = ds.Tables[0].Rows.Count.ToString();
            query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and role = 'Pharm'";
            ds = conn.getData(query);
            pharmnum.Text = ds.Tables[0].Rows.Count.ToString();
            query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and role = 'Accountent'";
            ds = conn.getData(query);
            pharmnum1.Text = ds.Tables[0].Rows.Count.ToString();
        }
        private void pharm_MouseEnter(object sender, MouseEventArgs e)
        {
            pharm.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCACACA"));
            P.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF505050"));
        }
        private void pharm_MouseLeave(object sender, MouseEventArgs e)
        {
            pharm.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            P.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF63676B"));
        }
        private void admin_MouseEnter(object sender, MouseEventArgs e)
        {
            admin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCACACA"));
            a.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF505050"));
        }
        private void admin_MouseLeave(object sender, MouseEventArgs e)
        {
            admin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            a.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF63676B"));
        }
        private async void addaccount_Loaded(object sender, RoutedEventArgs e)
        {
            fullnameLabel.Content = full;
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
        }

        private void StartGifAnimation()
        {
            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1221-test-tubes-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(pharmimage, image1);
            ImageBehavior.SetRepeatBehavior(pharmimage, System.Windows.Media.Animation.RepeatBehavior.Forever);

            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-948-stock-share-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage, image2);
            ImageBehavior.SetRepeatBehavior(accountentimage, System.Windows.Media.Animation.RepeatBehavior.Forever);

            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-268-avatar-man-hover-jump.gif"));
            ImageBehavior.SetAnimatedSource(adminimage, image3);
            ImageBehavior.SetRepeatBehavior(adminimage, System.Windows.Media.Animation.RepeatBehavior.Forever);

            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (2).gif"));
            ImageBehavior.SetAnimatedSource(back, image7);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);

        }
        private void pharm_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            Storyboard storyboard = (Storyboard)addaccount.FindResource("MoveDownStoryboard");
            storyboard.Begin(AnimatedBorder);
            role_add = "Accountent";
            info.pdf.Content = "Create Accountent";
            info.Visibility = Visibility.Visible;
            info.pPerm.Visibility = Visibility.Collapsed;
            info.adminPerm.Visibility = Visibility.Collapsed;
            info.rolee.Text = role_add;
            add.Visibility = Visibility.Collapsed;
        }
        private void pharm_MouseEnter1(object sender, MouseEventArgs e)
        {
            pharm_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCACACA"));
            P1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF505050"));
        }
        private void pharm_MouseLeave1(object sender, MouseEventArgs e)
        {
            pharm_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            P1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF63676B"));
        }
        private async void Button_Clickr(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            report.choose.Opacity = 1;
            report.reprots1.Opacity = 1;
            report.reprots1.Visibility = Visibility.Visible;
            report.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            fadeOutStoryboard.Begin();
            await Task.Delay(1200);
        }
        private async void Button_Clicks(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            setting.mainsetting.Visibility = Visibility.Visible;
            setting.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
            fadeOutStoryboard.Begin();
        }
        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }
        private async void pharm_Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimateBorder(pharm_Copy);
            await Task.Delay(200);
            Storyboard storyboard = (Storyboard)addaccount.FindResource("MoveDownStoryboard");
            storyboard.Begin(AnimatedBorder);
            role_add = "Accountent";
            info.pdf.Content = "Create Accountent";
            info.Visibility = Visibility.Visible;
            info.pPerm.Visibility = Visibility.Collapsed;
            info.adminPerm.Visibility = Visibility.Collapsed;
            info.rolee.Text = role_add;
            add.Visibility = Visibility.Collapsed;
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

        public async void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            info.Grid_Loaded(sender, e);
            back.IsEnabled = false;
            if (add.Visibility != Visibility.Visible)
            {
                Grid_Loaded_1(sender, e);
                Storyboard storyboard = (Storyboard)addaccount.FindResource("MoveupStoryboard");
                storyboard.Begin(AnimatedBorder);
                await Task.Delay(200);
                info.Grid_Loaded(sender, e);
                add.Visibility = Visibility.Visible;
                info.Visibility = Visibility.Collapsed;
            }
            await Task.Delay(0);
            back.IsEnabled = true;
        }
    }
}
