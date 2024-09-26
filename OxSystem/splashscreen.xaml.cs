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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for splashscreen.xaml
    /// </summary>
    public partial class splashscreen : Window
    {
        DbConnection conn = new DbConnection();
        DataSet ds;
        string query;
        public splashscreen()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            query = "select * from users_info";
            ds = conn.getData(query);
            await Task.Delay(3000);
            
           
            this.Close();
        }
    }
}
