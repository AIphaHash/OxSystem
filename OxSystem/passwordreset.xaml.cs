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
using System.Diagnostics;


namespace OxSystem
{

    public partial class passwordreset : UserControl
    {
        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public passwordreset()
        {
            InitializeComponent();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Label label = (Label)sender;
                string emailAddress = ((TextBlock)label.Content).Text;
                Console.WriteLine(emailAddress);
                string url = $"mailto:{emailAddress}?subject=Hello&body=I%20would%20like%20to%20send%20you%20a%20message.";
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the email client. Please make sure you have an email client configured.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            mybutton.IsEnabled = false;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            fadeOutStoryboard.Begin();
            await Task.Delay(1100);
            this.Visibility = Visibility.Collapsed;
            fadeElement.Opacity = 1;
            mybutton.IsEnabled = true;
        }


        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Label label = (Label)sender;
                TextBlock textBlock = (TextBlock)label.Content;
                string phoneNumber = "+964" + textBlock.Text;
                Console.WriteLine(phoneNumber);
                string url = $"https://wa.me/{phoneNumber}";
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open WhatsApp. Please check your internet connection or try again later.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Label_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Label label = (Label)sender;
                string emailAddress = ((TextBlock)label.Content).Text;
                Console.WriteLine(emailAddress);
                string url = $"mailto:{emailAddress}?subject=Hello&body=I%20would%20like%20to%20send%20you%20a%20message.";
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the email client. Please make sure you have an email client configured.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
