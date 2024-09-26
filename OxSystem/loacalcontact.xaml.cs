using System;
using System.Collections.Generic;
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
    /// Interaction logic for loacalcontact.xaml
    /// </summary>
    public partial class loacalcontact : UserControl
    {
        public loacalcontact()
        {
            InitializeComponent();
        }

        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];

            fadeOutStoryboard.Begin();
            await Task.Delay(1100);
            this.Visibility = Visibility.Collapsed;
            fadeElement.Opacity = 1;
        }
    }
}
