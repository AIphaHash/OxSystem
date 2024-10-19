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

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for showreport.xaml
    /// </summary>
    public partial class
        showreport : UserControl
    {

        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();

        public showreport()
        {
            InitializeComponent();
        }

        private async void reportfilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(500);

            // Retrieve the user's role from the database
            query = "select role from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and id = '" + Login_.iduser + "'";
            ds = conn.getData(query);
            string userrole = ds.Tables[0].Rows[0][0].ToString();



            // Query the report table to filter based on report_to_role
            query = "select rid, report_header, report_from, report_date from report " +
                    "where dbid = '"+Properties.Settings.Default.dbid+"' and report_to_role = 'All' or report_to_role = '" + userrole + "' AND report_header like '" + reportfilter.Text + "'";
            ds = conn.getData(query);

            // Bind the filtered data to the DataGrid
            DataGrid.ItemsSource = ds.Tables[0].DefaultView;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            // Ensure the user double-clicked on a valid row
            if (DataGrid.SelectedItem is DataRowView selectedRow)
            {
                // Retrieve the report ID from the selected row
                int reportId = Convert.ToInt32(selectedRow["rid"]);

                // Query to get the full header and body of the selected report
                query = $"select report_header, report_body ,report_from  from report where dbid = '"+Properties.Settings.Default.dbid+"' and rid = {reportId}";
                ds = conn.getData(query);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    // Extract the report details
                    string fullHeader = ds.Tables[0].Rows[0]["report_header"].ToString();
                    string fullBody = ds.Tables[0].Rows[0]["report_body"].ToString();
                    string from_ = ds.Tables[0].Rows[0][2].ToString();

                    // Set the TextBlocks to display the report details
                    headerreport.Text = fullHeader;
                    bodyreport.Text = fullBody;
                    fullname.Text = from_;

                    // Make the Border visible to display the report details
                    history.Visibility = Visibility.Collapsed;
                    show.Visibility = Visibility.Visible;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Retrieve the user's role from the database
            query = "select role from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and id = '" + Login_.iduser + "'";
            ds = conn.getData(query);
            string userrole = ds.Tables[0].Rows[0][0].ToString();

            // Define the roles to add to the ComboBox
            

            // Query the report table to filter based on report_to_role
            query = "select rid, report_header, report_from, report_date from report " +
                    "where dbid = '"+Properties.Settings.Default.dbid+"' and report_to_role = 'All' or report_to_role = '"+userrole+"'";
            ds = conn.getData(query);

            // Bind the filtered data to the DataGrid
            DataGrid.ItemsSource = ds.Tables[0].DefaultView;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            show.Visibility = Visibility.Collapsed;
            history.Visibility = Visibility.Visible;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
         

            // Fade out the choose grid
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "showg");
            fadeOutStoryboard.Begin(this);

            await Task.Delay(1200);

            showg.Visibility = Visibility.Collapsed;
            showg.Opacity = 1;
            UserControl_Loaded(sender, e);
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
