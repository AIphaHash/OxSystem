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
using System.Windows.Threading;
using WpfAnimatedGif;

namespace OxSystem
{
    public partial class adminaccount : UserControl, INotifyPropertyChanged
    {
        private string _currentTime;
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
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public static string fn;
        public static string r;
        public static string un;
        public static string pn;
        public static string em;
        public static string c;
        public static string p;
        public static string dob;
        public adminaccount()
        {
            InitializeComponent();
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
        public string user_name = Login_.username;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            UserControl_Loaded(sender, e);
            Storyboard moveInStoryboard = (Storyboard)paccountedit.FindResource("MoveUpStoryboard");
            moveInStoryboard.Begin();
            await Task.Delay(300);
            Storyboard fadeInStoryboard = (Storyboard)paccountedit.FindResource("FadeOutStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            paccountedit.Visibility = Visibility.Collapsed;
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
        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            fullnameLabel.Content = full;
            query = "select * from users_info where user_name = '" + user_name + "'";
            DataSet ds = conn.getData(query);

            if (ds.Tables[0].Rows.Count != 0)
            {
                fullname.Content = ds.Tables[0].Rows[0][8].ToString();
                fn = fullname.Content.ToString();
                name.Content = fn;
                r = ds.Tables[0].Rows[0][2].ToString();
                username.Content = ds.Tables[0].Rows[0][0].ToString();
                un = username.Content.ToString();
                phonenum.Content = ds.Tables[0].Rows[0][5].ToString();
                pn = phonenum.Content.ToString();
                email.Content = ds.Tables[0].Rows[0][4].ToString();
                em = email.Content.ToString();
                country.Content = ds.Tables[0].Rows[0][6].ToString();
                c = country.Content.ToString();
                p = ds.Tables[0].Rows[0][1].ToString();
                dobLABEL.Content = ds.Tables[0].Rows[0][7].ToString();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            paccountedit.editborder.Visibility = Visibility.Collapsed;
            paccountedit.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)paccountedit.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            paccountedit.editborder.Visibility = Visibility.Visible;
            Storyboard moveInStoryboard = (Storyboard)paccountedit.FindResource("MoveDownStoryboard");
            moveInStoryboard.Begin();
        }
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            edit.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF14756B"));
            edit.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF14756B"));
        }
        private void edit_MouseLeave(object sender, MouseEventArgs e)
        {
            edit.BorderBrush = new SolidColorBrush(Colors.White);
            edit.Foreground = new SolidColorBrush(Colors.White);
        }
        private async void reset_Click(object sender, RoutedEventArgs e)
        {
            paccountedit.editborder.Visibility = Visibility.Collapsed;
            paccountedit.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)paccountedit.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            paccountedit.editborder.Visibility = Visibility.Visible;
            Storyboard moveInStoryboard = (Storyboard)paccountedit.FindResource("MoveDownStoryboard");
            moveInStoryboard.Begin();
        }
        private async void Button_Clicks(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            setting.mainsetting.Visibility = Visibility.Visible;
            setting.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
            fadeOutStoryboard.Begin();
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

        private void StartGifAnimation()
        {
            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1414-circle-hover-pinch (1).gif"));
            ImageBehavior.SetAnimatedSource(avatar, image1);
            ImageBehavior.SetRepeatBehavior(avatar, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (2).gif"));
            ImageBehavior.SetAnimatedSource(back, image7);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);
        }
        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }
        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            Login_ l1 = new Login_();
            l1.Show();
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
        private void Button_MouseEnter1(object sender, MouseEventArgs e)
        {
            signout1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED7F7F"));
            signout1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED7F7F"));
        }

        private void edit_MouseLeave1(object sender, MouseEventArgs e)
        {
            signout1.BorderBrush = new SolidColorBrush(Colors.White);
            signout1.Foreground = new SolidColorBrush(Colors.White);
        }
    }
}

