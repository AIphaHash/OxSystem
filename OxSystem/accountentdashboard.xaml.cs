using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using Syncfusion.Windows.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
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
    /// <summary>
    /// Interaction logic for accountentdashboard.xaml
    /// </summary>
    /// 
    public class Bill
    {
        public string bdate { get; set; }
        public string Price { get; set; }
    }
    public class Bill11
    {
        public string bdate { get; set; }
        public string Price { get; set; }
    }
    public class Bill2
    {
        public string bdate { get; set; }
        public string Price { get; set; }
    }
    public class Bill3
    {
        public string bdate { get; set; }
        public string Price { get; set; }
    }
    public class Bill1
    {
        public string Type { get; set; }
        public int Count { get; set; }
    }

    public partial class accountentdashboard : UserControl
    {
        private DispatcherTimer chatReloadTimer;
        private bool isTextboxFocused = false;
        public static string fulln;
        public string CurrentUserId = Login_.iduser;
        private string selectedUserId = null;
        private string selectedUserName = null;
        string dateString = "%";
        string datehistroy = "%";
        private DateTime _selectedDate;
        string d_;
        private ICollectionView _collectionView;
        public static string f_ = "%";
        public static string t_ = "%";
        string type;
        string type1;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public ObservableCollection<Bill> BillsData { get; set; }
        public accountentdashboard()
        {
            InitializeComponent();

        }

        private void SwitchButton_Checked(object sender, RoutedEventArgs e)
        {
            BillLabel.Content = "Buy Bill";
            type = "buy";
            Window_Loaded(sender, e);
        }

        private void SwitchButton_Unchecked(object sender, RoutedEventArgs e)
        {
            BillLabel.Content = "Sell Bill";
            type = "sell";
            Window_Loaded(sender, e);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }


        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // Clear existing items in DataGrid_Copy
            DataGrid_Copy.ItemsSource = null;

            // Check if any cells are selected
            if (e.AddedCells.Count > 0)
            {
                // Assuming that the 'bid' column is the first column (0 index)
                // You may need to adjust the index based on your actual DataGrid setup
                if (e.AddedCells[0].Item is DataRowView selectedRow)
                {
                    // Retrieve the bid value from the selected row
                    int billId = Convert.ToInt32(selectedRow["billId"]); // Adjust the column name as needed

                    // Build your SQL query using the retrieved billId
                    string query = $@"
                SELECT DISTINCT m.mid, m.mname, m.bprice, m.sprice, h.quantity
                FROM bills b
                JOIN medicinfo m ON b.billId = m.billId
                JOIN medichistory h ON b.billId = h.billId AND m.mid = h.mid
                WHERE b.billId = {billId} AND b.type = '"+type+"';";

                    // Fetch data using your existing getData method
                    DataSet ds = conn.getData(query);

                    // Check if the DataSet has any tables and rows
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        // Convert the DataTable to a list of MedicInfo objects
                        List<MedicInfo> medicData = new List<MedicInfo>();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            medicData.Add(new MedicInfo
                            {
                                Mid = Convert.ToInt32(row["mid"]),
                                Mname = row["mname"].ToString(),
                                Bprice = Convert.ToDecimal(row["bprice"]),
                                Sprice = Convert.ToDecimal(row["sprice"]),
                                Quantity = Convert.ToInt32(row["quantity"])
                            });
                        }

                        // Set the ItemsSource for DataGrid_Copy
                        DataGrid_Copy.ItemsSource = medicData;
                    }
                    else
                    {
                        MessageBox.Show("No data found for the selected bill.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }




        public class MedicInfo
        {
            public int Mid { get; set; }
            public string Mname { get; set; }
            public decimal Bprice { get; set; }
            public decimal Sprice { get; set; }
            public int Quantity { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            starg();
            try
            {
                query = "select * from bills where type ='" + type + "'";
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


            // Query to get admin users, excluding the logged-in user
            query = $"SELECT id, fullname FROM users_info WHERE  id <> '{CurrentUserId}'";
            ds = new DbConnection().getData(query);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string userId = row["id"].ToString();
                string fullName = row["fullname"].ToString();

                // Query to get the last message for each user
                string lastMessageQuery = $"SELECT TOP 1 message FROM UserMessages WHERE sender_id = '{userId}' ORDER BY timestamp DESC";
                DataSet lastMessageDs = new DbConnection().getData(lastMessageQuery);
                string lastMessage = lastMessageDs.Tables[0].Rows.Count > 0 ? lastMessageDs.Tables[0].Rows[0]["message"].ToString() : "No messages yet";

                // Create a StackPanel to hold the image and text
                StackPanel cardContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                // Create the Image element
                Image userImage = new Image
                {
                    Source = new BitmapImage(new Uri("images/1414.png", UriKind.Relative)),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(0, 0, 0, 0) // Margin between image and text
                };

                // Create the TextBlock for the user's full name and last message
                TextBlock buttonContent = new TextBlock
                {
                    Margin = new Thickness(10, 20, 0, 0) // Adjust the top margin here
                };

                buttonContent.Inlines.Add(new Run(fullName)
                {
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                });

                buttonContent.Inlines.Add(new LineBreak());

                buttonContent.Inlines.Add(new Run($"Last message: {lastMessage}")
                {
                    FontSize = 12
                });

                // Add the image and text to the StackPanel
                cardContent.Children.Add(userImage);
                cardContent.Children.Add(buttonContent);

                // Create the Button for each user
                Button cardButton = new Button
                {
                    Content = cardContent,
                    Tag = userId,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    Background = new SolidColorBrush((Colors.White)),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    BorderBrush = null,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Width = 400
                };


            }

            // Start the glow animation
            var glowAnimation = (Storyboard)Resources["GlowAnimation"];
            /*glowAnimation.Begin(glowingBorder1, true);
            glowAnimation.Begin(glowingBorder2, true);
            glowAnimation.Begin(glowingBorder3, true);*/

        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                query = "select * from bills";
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

        private void sname_KeyUp(object sender, KeyEventArgs e)
        {
            if (_collectionView == null)
                return;

            _collectionView.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(sname.Text))
                    return true;

                return ((string)item).IndexOf(sname.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            _collectionView.Refresh();
            sname.IsDropDownOpen = true;
        }

        private void sname_Loaded(object sender, RoutedEventArgs e)
        {
            _collectionView = CollectionViewSource.GetDefaultView(GetFromNames());
            sname.ItemsSource = _collectionView;
        }
        public List<string> GetFromNames()
        {
            List<string> storageNames = new List<string>();
            string query = "SELECT DISTINCT from_\r\nFROM bills;\r\n";

            // Assuming you have a method in your connection class to get data
            DataSet ds = conn.getData(query);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    storageNames.Add(row["from_"].ToString());
                }
            }

            return storageNames;
        }
        public List<string> GetTooNames()
        {
            List<string> storageNames = new List<string>();
            string query = "SELECT DISTINCT too_\r\nFROM bills;\r\n";

            // Assuming you have a method in your connection class to get data
            DataSet ds = conn.getData(query);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    storageNames.Add(row["too_"].ToString());
                }
            }

            return storageNames;
        }

        private void sname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sname.SelectedItem != null)
            {
                f_ = sname.SelectedItem.ToString();

                try
                {
                    query = "select * from bills where type = '"+type+"' and from_ like '" + f_ + "' AND too_ like'" + t_ + "'";
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
            else
            {
                f_ = null; // Or handle null case appropriately
            }
        }



        private void sname_KeyUp1(object sender, KeyEventArgs e)
        {
            if (_collectionView == null)
                return;

            _collectionView.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(sname_Copy.Text))
                    return true;

                return ((string)item).IndexOf(sname_Copy.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            _collectionView.Refresh();
            sname_Copy.IsDropDownOpen = true;
        }

        private void sname_Loaded1(object sender, RoutedEventArgs e)
        {
            _collectionView = CollectionViewSource.GetDefaultView(GetTooNames());
            sname_Copy.ItemsSource = _collectionView;
        }

        private void sname_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (sname_Copy.SelectedItem != null)
            {
                t_ = sname_Copy.SelectedItem.ToString();
                try
                {
                    query = "select * from bills where type = '"+type+"' and from_ like '" + f_ + "' AND too_ like'" + t_ + "'";
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
            else
            {
                t_ = null; // Or handle null case appropriately
            }
        }

        
        private DateTime GetEndOfWeekDate(string datehistroy)
        {
            DateTime selectedDate;

            // Try parsing the datehistroy string to DateTime
            if (DateTime.TryParse(datehistroy, out selectedDate))
            {
                // Calculate the end of the week (Sunday) for the given date
                DayOfWeek endOfWeek = DayOfWeek.Sunday;
                int daysUntilEndOfWeek = ((int)endOfWeek - (int)selectedDate.DayOfWeek + 7) % 7;
                return selectedDate.AddDays(daysUntilEndOfWeek).Date;
            }

            // Return current date if parsing fails
            return DateTime.Now.Date;
        }
        private async void Border_Loaded(object sender, RoutedEventArgs e)
        {




          

            Console.WriteLine(_daysDifference);

            if (_daysDifference == 0)
            {
                resetlabel();

                query = "SELECT Price FROM bills WHERE bdate LIKE '" + datehistroy + "' AND type LIKE '" + type1 + "'";
                ds = conn.getData(query);
                decimal totalPrices = 0;

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        if (decimal.TryParse(row["Price"].ToString(), out decimal price))
                        {
                            totalPrices += price;
                        }
                    }
                }

                string totalPricesString = totalPrices.ToString("0.00");

                if (totalPrices == 0)
                {
                    totalPricesString = "N/A";
                }
             
            }
            else
            {
                resetlabel();
            }
            if (_daysDifference <= 7 && _daysDifference > 0)
            {
                query = "SELECT Price FROM bills WHERE bdate BETWEEN '" + datehistroy + "' AND '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' AND type LIKE '" + type1 + "'";
                ds = conn.getData(query);
                decimal totalPrices = 0;

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        if (decimal.TryParse(row["Price"].ToString(), out decimal price))
                        {
                            totalPrices += price;
                        }
                    }
                }
                string concatenatedPrices = totalPrices.ToString("0.00");

                if (totalPrices == 0)
                {
                    concatenatedPrices = "N/A";
                }
             
            }


            // Handle Monthly and Yearly calculations
            DateTime selectedDate = DateTime.Parse(datehistroy);
            DateTime startOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            if (_daysDifference > 0)
            {
            }
        }

        private void resetlabel()
        {
        }








        private void SwitchButton_Unchecked1(object sender, RoutedEventArgs e)
        {
           
        }

        private void SwitchButton_Checked1(object sender, RoutedEventArgs e)
        {
        }

        private int _daysDifference;


        public void DateTimeEdit_SelectionChanged_1(object sender, RoutedEventArgs e)
        {
            


            Border_Loaded(sender, e);
        }

        private void SplineSeries_Loaded(object sender, RoutedEventArgs e)
        {

            BillsData = new ObservableCollection<Bill>();

            string query = "SELECT bdate, Price FROM bills";
            DataSet ds = conn.getData(query);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    // Get the original price as decimal
                    decimal originalPrice = Convert.ToDecimal(row["Price"]);

                    // Format the price with commas for every three digits and convert to string
                    string formattedPrice = String.Format("{0:N0}", originalPrice);

                    BillsData.Add(new Bill
                    {
                        bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                        Price = formattedPrice // Use the formatted price as a string
                    });
                }

            }

            // Bind the data to the chart
            this.DataContext = this;

        }
        private void PieChart_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Bill1> billsData = new ObservableCollection<Bill1>();

            // Query database for counts of buy and sell bills
            string query = "SELECT Type, COUNT(*) AS Count FROM bills GROUP BY Type";
            DataSet ds = conn.getData(query);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    billsData.Add(new Bill1
                    {
                        Type = row["Type"].ToString(),
                        Count = Convert.ToInt32(row["Count"])
                    });
                }
            }

            // Bind the data to the pie chart
            pieChart.DataContext = billsData;
        }


        // Class to hold the count data


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            PieChart_Loaded(sender, e);
        }

        private void syncdate_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void mybutton_LostFocus(object sender, RoutedEventArgs e)
        {

        }



        private void admindash_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private async void Label_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(400);
            try
            {
                query = "select * from bills where type like 'sell'";
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

        private void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DateTimeEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDateNullable = date.SelectedDate;

            if (selectedDateNullable.HasValue)
            {
                // If the selected date is not null, use its value
                _selectedDate = selectedDateNullable.Value;

                // Convert the DateTime to a string if needed
                dateString = _selectedDate.ToString("yyyy-MM-dd");


            }
            else
            {

            }

            try
            {
                query = "select * from bills where type = '"+type+"' and from_ like '" + f_ + "' AND too_ like'" + t_ + "' AND bdate like '" + dateString + "'";
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

        private void searchBox1_GotFocus(object sender, RoutedEventArgs e)
        {

            if (searchBox1.Text == "🔍  Type to search ")
            {
                searchBox1.Text = "";
                searchBox1.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (searchBox1.Text == "")
            {
                searchBox1.Text = "";
                searchBox1.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void searchBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "")
            {
                searchBox1.Text = "🔍  Type to search ";
                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox1.Text == "" || searchBox1.Text == "🔍  Type to search ")
            {
                query = "select * from bills where type like '"+type+"'";
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
            else
            {
                query = "select * from bills where type like '"+type+"' and from_ like '"+searchBox1.Text+"'";
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

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid_Loaded(sender, e);
            Grid_Loaded(sender, e);
            Border_Loaded(sender, e);
            Window_Loaded(sender, e);
        }
        private void starg()
        {
            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (2).gif"));
            ImageBehavior.SetAnimatedSource(back, image5);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);

        }
    }





}




