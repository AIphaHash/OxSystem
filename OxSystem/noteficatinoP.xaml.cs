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
using WpfAnimatedGif;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for noteficatinoP.xaml
    /// </summary>
    public partial class noteficatinoP : UserControl
    {
        private List<int> selectedIds = new List<int>();
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        string checklabel = "label1";
        public noteficatinoP()
        {
            InitializeComponent();
        }

        

        

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
             selectedIds.Clear(); // Clear previous selections

            foreach (var item in DataGrid.SelectedItems)
            {
                var selectedRow = item as DataRowView;
                if (selectedRow != null)
                {
                    int id = Convert.ToInt32(selectedRow["mid"]);
                    selectedIds.Add(id);
                }
            }
        }

        private void DataGrid_Loaded_1(object sender, RoutedEventArgs e)
        {

            label3.Content = Properties.Settings.Default.ex2 + "Or less";
            label2.Content = Properties.Settings.Default.ex3 + "Or less";
            label1.Content = Properties.Settings.Default.ex4 + "Or less";





            DateTime futureDate1 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex1));
            string shortDateString1 = futureDate1.ToString("yyyy-MM-dd");


            try
            {
                query = "select * from medicinfo where exdate <= '"+shortDateString1+"'";
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

        private void bprice_GotFocus(object sender, RoutedEventArgs e)
        {
            if (bprice.Text == "Search Medic Name...")
            {
                bprice.Text = "";
                bprice.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (bprice.Text == "")
            {
                bprice.Text = "";
                bprice.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void bprice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (bprice.Text == "")
            {
                bprice.Text = "Search Medic Name...";
                bprice.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
                UserControl_Loaded(sender, e);
            }
        }

        private void bprice_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void bprice_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private async void bprice_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(1000);



            if (bprice.Text != null || bprice.Text != "Search Medic Name...")
            {


                if (checklabel == "label4")
                {
                    DateTime futureDate2 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex1));
                    string shortDateString2 = futureDate2.ToString("yyyy-MM-dd");

                    query = "select * from medicinfo where exdate <=  '"+shortDateString2+"' AND mname like '" + bprice.Text + "%'";
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
                else if (checklabel == "label3")
                {
                    DateTime futureDate3 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex2));
                    string shortDateString3 = futureDate3.ToString("yyyy-MM-dd");

                    query = "select * from medicinfo where exdate <=  '" + shortDateString3 + "' AND mname like '" + bprice.Text + "%'"; 
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
                else if (checklabel == "label2")
                {
                    DateTime futureDate4 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex3));
                    string shortDateString4 = futureDate4.ToString("yyyy-MM-dd");

                    query = "select * from medicinfo where exdate <=  '" + shortDateString4 + "' AND mname like '" + bprice.Text + "%'";
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
                else if (checklabel == "label1")
                {
                    DateTime futureDate5 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex4));
                    string shortDateString5 = futureDate5.ToString("yyyy-MM-dd");


                    query = "select * from medicinfo where exdate <=  '" + shortDateString5 + "' AND mname like '" + bprice.Text + "%'";
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
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIds.Count == 0)
            {
                MessageBox.Show("No rows selected for deletion.", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete the selected rows?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Create a query to delete all selected rows
                var idsString = string.Join(",", selectedIds);
                string query = $"DELETE FROM medicinfo WHERE mid IN ({idsString})";

                conn.setData(query);

                // Reload the data to reflect changes
                UserControl_Loaded(sender, e);
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {

        }

        /*private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Label_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            
        }*/
        private void startgifanimation()
        {
            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-39-trash-hover-trash-empty.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage6, image7);
            ImageBehavior.SetRepeatBehavior(accountentimage6, System.Windows.Media.Animation.RepeatBehavior.Forever);



            var image8 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-44-folder-hover-folder.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage7, image8);
            ImageBehavior.SetRepeatBehavior(accountentimage7, System.Windows.Media.Animation.RepeatBehavior.Forever);


        }
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Reset all borders to grey
            Storyboard greyFill = (Storyboard)this.Resources["GreyFillAnimation"];
            greyFill.Begin(border1);
            greyFill.Begin(border2);
            greyFill.Begin(border3);
            greyFill.Begin(border4);

            // Apply blue fill to the corresponding border
            Storyboard blueFill = (Storyboard)this.Resources["BlueFillAnimation"];
            if (sender == label1)
            {

                checklabel = "label1";
                blueFill.Begin(border1);
                DateTime futureDate5 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex4));
                string shortDateString5 = futureDate5.ToString("yyyy-MM-dd");


                query = "select * from medicinfo where exdate <=  '" + shortDateString5 + "'";
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
            else if (sender == label2)
            {
                checklabel = "label2";
                blueFill.Begin(border2);
                DateTime futureDate4 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex3));
                string shortDateString4 = futureDate4.ToString("yyyy-MM-dd");

                query = "select * from medicinfo where exdate <=  '" + shortDateString4 + "'";
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
            else if (sender == label3)
            {
                checklabel = "label3";
                blueFill.Begin(border3);
                DateTime futureDate3 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex2));
                string shortDateString3 = futureDate3.ToString("yyyy-MM-dd");

                query = "select * from medicinfo where exdate <=  '" + shortDateString3 + "'";
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
            else if (sender == label4)
            {
                checklabel = "label4";
                blueFill.Begin(border4);
                DateTime futureDate2 = DateTime.Now.AddDays(int.Parse(Properties.Settings.Default.ex1));
                string shortDateString2 = futureDate2.ToString("yyyy-MM-dd");

                query = "select * from medicinfo where exdate <=  '" + shortDateString2 + "'";
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
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Properties.Settings.Default.ex1);
            Console.WriteLine(Properties.Settings.Default.ex2);
            Console.WriteLine(Properties.Settings.Default.ex3);
            Console.WriteLine(Properties.Settings.Default.ex4);
            startgifanimation();
         
               
                query = "select * from medicinfo";
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Clicks(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Clickr(object sender, RoutedEventArgs e)
        {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Border_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void accountentimage6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void accountentimage7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void pdf_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
