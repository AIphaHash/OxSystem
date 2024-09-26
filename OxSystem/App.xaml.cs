using Syncfusion.Windows.Tools.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace OxSystem
{
    public partial class App : Application
    {
        public App()
        {
            // Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXdedXRRRmZfUUBwVkM=");
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create and show the splash screen
          

            // Start GIF animation
            await Task.Run(() => StartGifAnimation());

            // Simulate some loading time (e.g., 3 seconds)
            
            Login_ mainWindow = new Login_();
            mainWindow.Show();
        }

        private void StartGifAnimation()
        {
           /* // Perform GIF animation setup
            Application.Current.Dispatcher.Invoke(() =>
            {
                var image1 = new BitmapImage(new Uri("pack://application:,,,/images/pharm.gif"));
                ImageBehavior.SetAnimatedSource(YourPharmImageElement, image1);
                ImageBehavior.SetRepeatBehavior(YourPharmImageElement, System.Windows.Media.Animation.RepeatBehavior.Forever);

                var image2 = new BitmapImage(new Uri("pack://application:,,,/images/pay.gif"));
                ImageBehavior.SetAnimatedSource(YourAccountentImageElement, image2);
                ImageBehavior.SetRepeatBehavior(YourAccountentImageElement, System.Windows.Media.Animation.RepeatBehavior.Forever);

                var image3 = new BitmapImage(new Uri("pack://application:,,,/images/admin.gif"));
                ImageBehavior.SetAnimatedSource(YourAdminImageElement, image3);
                ImageBehavior.SetRepeatBehavior(YourAdminImageElement, System.Windows.Media.Animation.RepeatBehavior.Forever);
            });*/
        }
    }
}
