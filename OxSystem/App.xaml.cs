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
            if (OxSystem.Properties.Settings.Default.actkey == "1")
            {
                Login_ mainWindow = new Login_();
                mainWindow.Show();
                
            }
            else
            {
                Login actwindow = new Login();
                actwindow.Show();
            }
        }
    }
}
