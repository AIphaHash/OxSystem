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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OxSystem
{
   
    public partial class salary : UserControl
    {

        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();
        public salary()
        {
            InitializeComponent();
        }

        private void header_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                query = "select * from users_info";
                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
                }
                else
                {
                    MessageBox.Show("No data found or an error occurred.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
