using LiveCharts.Wpf;
using Org.BouncyCastle.Asn1.Sec;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
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

    public partial class AdminDashboard : UserControl, INotifyPropertyChanged
    {
        private string comboBoxText;
        private SfChart currentChart = null;
        public class PharmacistBills
        {
            public string PharmacistName { get; set; }
            public int BillCount { get; set; }
        }
        public class Bill1
        {
            public string Type { get; set; }
            public int Count { get; set; }
        }
        public class BillInfo
        {
            public string BillType { get; set; }
            public int BillCount { get; set; }
        }

        public ObservableCollection<BillInfo> BuySellData { get; set; }
        private List<string> _tips = new List<string>
        {
            "Tip 1: Always back up your data.",
            "Tip 2: Use strong passwords.",
            "Tip 3: Keep your software up to date."
        };

        public ObservableCollection<Bill> BillsData { get; set; }
        public ObservableCollection<Bill11> BillsData1 { get; set; }
        public ObservableCollection<Bill2> BillsData2 { get; set; }
        public ObservableCollection<Bill3> BillsData3 { get; set; }
        private string _randomTip;
        public string RandomTip
        {
            get { return _randomTip; }
            set
            {
                _randomTip = value;
                OnPropertyChanged(nameof(RandomTip));
            }
        }
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
        string chartquery;
        string chartquerylabel = "select bdate,Price from bills where dbid = '"+Properties.Settings.Default.dbid+"'";
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public string CurrentUserId = Login_.iduser;
        public static string full = Login_.fullName;
        public AdminDashboard()
        {
            InitializeComponent();

            query = "SELECT role FROM users_info WHERE dbid = '"+Properties.Settings.Default.dbid+"' and id = '" + CurrentUserId + "'";
            ds = conn.getData(query);

            // Check if the DataSet contains tables and if the first table has rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // If there are rows, set the control values
                  // Role
            }
            else
            {

                add_Copy.Visibility = Visibility.Collapsed;
                add_Copy1.Visibility = Visibility.Collapsed;
                
            }



            currentChart = all;
            Properties.Settings.Default.report_visible = "1";
            Properties.Settings.Default.Save();
            comboBoxText = ComboBoxAdv.Text;
            DataContext = this;
            LoadRandomTip();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            UpdateCurrentTime();
        }

        private void SplineSeries_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.chart_currency == "$")
            {
                BillsData = new ObservableCollection<Bill>();

                chartquery = "SELECT \r\n    bdate, \r\n    Price \r\nFROM \r\n    bills where dbid = '"+Properties.Settings.Default.dbid+"' \r\nORDER BY \r\n    bdate ASC  ;\r\n";
                DataSet ds = conn.getData(chartquery);

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
                this.DataContext = this;
            }
            else
            {
                BillsData = new ObservableCollection<Bill>();
                chartquery = "SELECT \r\n    bdate, \r\n    Price \r\nFROM \r\n    bills where dbid = '"+Properties.Settings.Default.dbid+"' \r\nORDER BY \r\n    bdate ASC ;\r\n";
                DataSet ds = conn.getData(chartquery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        // Assuming row["Price"] is a string and needs to be converted to decimal for calculations
                        decimal originalPrice = Convert.ToDecimal(row["Price"]);
                        decimal priceInIQD = originalPrice * Properties.Settings.Default.currency;

                        // Use Math.Floor only if needed
                        // You can use decimal.Round if you want to round to the nearest integer
                        decimal roundedPrice = Math.Floor(priceInIQD);

                        // Format the price as a string with commas
                        string formattedPrice = String.Format("{0:N0}", roundedPrice);

                        BillsData.Add(new Bill
                        {
                            bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                            Price = formattedPrice // Use the formatted price as a string
                        });
                    }
                }

                // Make sure the DataContext is refreshed after updating the BillsData
                this.DataContext = this;
            }

        }
        private void LoadRandomTip()
        {
            Random random = new Random();
            int index = random.Next(_tips.Count);
            RandomTip = _tips[index];
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
        public class RoleCount
        {
            public string Role { get; set; }
            public int Count { get; set; }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Properties.Settings.Default.currency);
            welcome_copy.Content = full;

            if (Properties.Settings.Default.chart_currency == "$")
            {
                query = @"
SELECT 
    COUNT(*) AS TotalBills
FROM 
    bills where dbid = '"+Properties.Settings.Default.dbid+"' ;";
                ds = conn.getData(query);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalBills"];
                    if (result != DBNull.Value)
                    {
                        int totalBills = Convert.ToInt32(result);
                        totalbills.Content = totalBills.ToString();
                    }
                    else
                    {
                        totalbills.Content = "0";
                    }
                }
                else
                {
                    totalbills.Content = "0";
                }
                query = @"
SELECT 
    ISNULL(SUM(CASE WHEN type = 'buy' THEN Price ELSE 0 END), 0) -
    ISNULL(SUM(CASE WHEN type = 'sell' THEN Price ELSE 0 END), 0) AS Loss
FROM 
    bills
WHERE  dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";
                ds = conn.getData(query);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["Loss"];
                    if (result != DBNull.Value)
                    {
                        decimal loss = Convert.ToDecimal(result);
                        loss_.Content = loss.ToString("C", CultureInfo.CurrentCulture)
                                            .Replace("(", "-")
                                            .Replace(")", "");
                    }
                    else
                    {
                        loss_.Content = "$0.00";
                    }
                }
                else
                {
                    loss_.Content = "$0.00";
                }
                query = @"
SELECT 
    ISNULL(SUM(CASE WHEN type = 'sell' THEN Price ELSE 0 END), 0) -
    ISNULL(SUM(CASE WHEN type = 'buy' THEN Price ELSE 0 END), 0) AS Profit
FROM 
    bills
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";

                ds = conn.getData(query);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["Profit"];
                    if (result != DBNull.Value)
                    {
                        decimal profit = Convert.ToDecimal(result);
                        profit_.Content = profit.ToString("C", CultureInfo.CurrentCulture)
                                                  .Replace("(", "-")
                                                  .Replace(")", "");
                    }
                    else
                    {
                        profit_.Content = "$0.00";
                    }
                }
                else
                {
                    profit_.Content = "$0.00";
                }
                query = @"
SELECT 
    SUM(Price) AS TotalSpends
FROM 
    bills
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'buy'  AND bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSpends"];
                    if (result != DBNull.Value)
                    {
                        decimal totalSpends = Convert.ToDecimal(result);
                        spends.Content = totalSpends.ToString("C", CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        spends.Content = "$0.00";
                    }
                }
                else
                {
                    spends.Content = "$0.00";
                }
                query = @"
SELECT 
    SUM(Price) AS TotalSales
FROM 
    bills
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'sell' AND bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSales"];
                    if (result != DBNull.Value)
                    {
                        decimal totalSales = Convert.ToDecimal(result);
                        sales.Content = totalSales.ToString("C", CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        sales.Content = "$0.00";
                    }
                }
                else
                {
                    sales.Content = "$0.00";
                }
                query = @"
        SELECT 
            s.sid,
            s.supname,
            s.supnum,
           
            (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
        FROM 
            Suppliers s where s.dbid = '"+Properties.Settings.Default.dbid+"' ";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                }
            }
            else
            {
                query = @"
SELECT 
    ISNULL(SUM(CASE WHEN type = 'buy' THEN Price ELSE 0 END), 0) -
    ISNULL(SUM(CASE WHEN type = 'sell' THEN Price ELSE 0 END), 0) AS Loss
FROM 
    bills
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";
                ds = conn.getData(query);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["Loss"];
                    if (result != DBNull.Value)
                    {
                        decimal lossUSD = Convert.ToDecimal(result);
                        decimal lossIQD = lossUSD * Properties.Settings.Default.currency;
                        loss_.Content = lossIQD.ToString("N0", CultureInfo.CurrentCulture)
                                               .Replace("(", "-")
                                               .Replace(")", "") + " IQD";
                    }
                    else
                    {
                        loss_.Content = "0 IQD";
                    }
                }
                else
                {
                    loss_.Content = "0 IQD";
                }

                query = @"
SELECT 
    ISNULL(SUM(CASE WHEN type = 'sell' THEN Price ELSE 0 END), 0) -
    ISNULL(SUM(CASE WHEN type = 'buy' THEN Price ELSE 0 END), 0) AS Profit
FROM 
    bills
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["Profit"];
                    if (result != DBNull.Value)
                    {
                        decimal profitUSD = Convert.ToDecimal(result);
                        decimal profitIQD = profitUSD * Properties.Settings.Default.currency;
                        profit_.Content = profitIQD.ToString("N0", CultureInfo.CurrentCulture)
                                                   .Replace("(", "-")
                                                   .Replace(")", "") + " IQD";
                    }
                    else
                    {
                        profit_.Content = "0 IQD";
                    }
                }
                else
                {
                    profit_.Content = "0 IQD";
                }
                query = @"
SELECT 
    SUM(Price) AS TotalSpends
FROM 
    bills
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'buy'  AND bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSpends"];
                    if (result != DBNull.Value)
                    {
                        decimal totalSpends = Convert.ToDecimal(result);
                        decimal totalSpendsIQD = totalSpends * Properties.Settings.Default.currency;
                        spends.Content = totalSpendsIQD.ToString("N0", CultureInfo.CurrentCulture) + " IQD";
                    }
                    else
                    {
                        spends.Content = "0 IQD";
                    }
                }
                else
                {
                    spends.Content = "0 IQD";
                }

                query = @"
        SELECT 
            s.sid,
            s.supname,
            s.supnum,
           
            (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
        FROM 
            Suppliers s where s.dbid = '"+Properties.Settings.Default.dbid+"' ";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                }
                query = @"
SELECT 
    SUM(Price) AS TotalSales
FROM 
    bills
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'sell'  AND bdate >= DATEADD(month, -1, GETDATE()) AND bdate < GETDATE();";

                ds = conn.getData(query);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSales"];
                    if (result != DBNull.Value)
                    {
                        decimal totalSales = Convert.ToDecimal(result);
                        decimal totalSalesIQD = totalSales * Properties.Settings.Default.currency;
                        sales.Content = totalSalesIQD.ToString("N0", CultureInfo.CurrentCulture) + " IQD";
                    }
                    else
                    {
                        sales.Content = "0 IQD";
                    }
                }
                else
                {
                    sales.Content = "0 IQD";
                }
            }
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
            query = "select * from bills where dbid = '"+Properties.Settings.Default.dbid+"' ";
            ds = conn.getData(query);
            totalbills.Content = ds.Tables[0].Rows.Count;
            query = @"
        SELECT type, COUNT(*) as BillCount 
        FROM bills 
        WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type IN ('buy', 'sell')  GROUP BY type";
            ds = conn.getData(query);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                BuySellData = new ObservableCollection<BillInfo>();
                foreach (DataRow row in dt.Rows)
                {
                    BuySellData.Add(new BillInfo
                    {
                        BillType = row["type"].ToString(),
                        BillCount = Convert.ToInt32(row["BillCount"])
                    });
                }
                BillsChart.DataContext = this;
            }
            SplineSeries_Loaded(sender, e);
            SplineSeries_Loaded1(sender, e);
            SplineSeries_Loaded2(sender, e);
            SplineSeries_Loaded3(sender, e);
            welcome.Content = $"Welcome Back {full}";
            fullnameLabel.Content = AdminDash.fulln;
            try
            {
                var roleCounts = new List<RoleCount>();
                query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and role = 'Admin'";
                ds = conn.getData(query);
                int adminCount = ds.Tables[0].Rows.Count;
                roleCounts.Add(new RoleCount { Role = "Admin", Count = adminCount });
                query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and role = 'Pharm'";
                ds = conn.getData(query);
                int pharmCount = ds.Tables[0].Rows.Count;
                roleCounts.Add(new RoleCount { Role = "Pharm", Count = pharmCount });
                query = "select * from users_info where dbid = '"+Properties.Settings.Default.dbid+"' and role = 'Accountent'";
                ds = conn.getData(query);
                int accountentCount = ds.Tables[0].Rows.Count;
                roleCounts.Add(new RoleCount { Role = "Accountent", Count = accountentCount });
                pieChart.Series[0].ItemsSource = roleCounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            try
            {
                query = @"
        SELECT ui.fullname AS PharmacistName, COUNT(b.bid) AS BillCount
        FROM users_info ui
        LEFT JOIN bills b ON ui.fullname = b.by_
        WHERE ui.dbid = '"+Properties.Settings.Default.dbid+"' and ui.role = 'Pharm' GROUP BY ui.fullname";
                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    var pharmacistBillsList = new List<PharmacistBills>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        pharmacistBillsList.Add(new PharmacistBills
                        {
                            PharmacistName = row["PharmacistName"].ToString(),
                            BillCount = Convert.ToInt32(row["BillCount"])
                        });
                    }

                    AdminSeries.ItemsSource = pharmacistBillsList;
                    PharmSeries.ItemsSource = pharmacistBillsList;
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
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            thisday.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3E0FF"));
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            thisday.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F7FE"));
        }

        private void thismonth_MouseEnter(object sender, MouseEventArgs e)
        {
            thismonth.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3E0FF"));
        }

        private void thismonth_MouseLeave(object sender, MouseEventArgs e)
        {
            thismonth.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F7FE"));
        }

        private void thisyear_MouseEnter(object sender, MouseEventArgs e)
        {
            thisyear.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3E0FF"));
        }

        private void thisyear_MouseLeave(object sender, MouseEventArgs e)
        {
            thisyear.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F7FE"));
        }

        private async void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowChart(day);
            await Task.Delay(1000);
            year.Visibility = Visibility.Collapsed;
            month.Visibility = Visibility.Collapsed;
            all.Visibility = Visibility.Collapsed;
        }

        private async void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ShowChart(month);
            await Task.Delay(1000);
            year.Visibility = Visibility.Collapsed;
            all.Visibility = Visibility.Collapsed;
            day.Visibility = Visibility.Collapsed;
        }

        private async void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            ShowChart(year);
            await Task.Delay(1000);
            all.Visibility = Visibility.Collapsed;
            month.Visibility = Visibility.Collapsed;
            day.Visibility = Visibility.Collapsed;
        }
        private void Button_Clicka(object sender, RoutedEventArgs e)
        {
            chartquerylabel = "SELECT bdate, Price\r\nFROM bills\r\nWHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0)\r\n  AND bdate < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0);\r\n";
            UserControl_Loaded(sender, e);
        }
        private void SplineSeries_Loaded3(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.chart_currency == "$")
            {
                BillsData3 = new ObservableCollection<Bill3>();

                chartquery = "SELECT \r\n    bdate, \r\n    Price \r\nFROM \r\n    bills \r\nWHERE dbid = '"+Properties.Settings.Default.dbid+"' and \r\n    bdate >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)\r\n    AND bdate < DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1))\r\nORDER BY \r\n    bdate ASC;\r\n";
                DataSet ds = conn.getData(chartquery);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        // Get the original price as decimal
                        decimal originalPrice = Convert.ToDecimal(row["Price"]);

                        // Format the price with commas for every three digits and convert to string
                        string formattedPrice = String.Format("{0:N0}", originalPrice);

                        BillsData3.Add(new Bill3
                        {
                            bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                            Price = formattedPrice // Use the formatted price as a string
                        });
                    }

                }
                this.DataContext = this;
            }
            else
            {
                BillsData3 = new ObservableCollection<Bill3>();
                chartquery = @"
   SELECT 
    bdate, 
    Price 
FROM 
    bills 
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1) AND bdate < DATEADD(YEAR, 1, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) ORDER BY bdate ASC;";
                DataSet ds = conn.getData(chartquery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        // Assuming row["Price"] is a string and needs to be converted to decimal for calculations
                        decimal originalPrice = Convert.ToDecimal(row["Price"]);
                        decimal priceInIQD = originalPrice * Properties.Settings.Default.currency;
                        decimal priceWithoutDecimal = Math.Floor(priceInIQD);

                        // Format the price as a string with commas
                        string formattedPrice = String.Format("{0:N0}", priceWithoutDecimal);

                        BillsData3.Add(new Bill3
                        {
                            bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                            Price = formattedPrice // Assign the formatted price string here
                        });
                    }




                }
                this.DataContext = this;
            }
        }

        private void SplineSeries_Loaded2(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.chart_currency == "$")
            {
                BillsData2 = new ObservableCollection<Bill2>();

                chartquery = "SELECT \r\n    bdate, \r\n    Price \r\nFROM \r\n    bills \r\nWHERE dbid = '"+Properties.Settings.Default.dbid+"' and\r\n    bdate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0)\r\n    AND bdate < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)\r\nORDER BY \r\n    bdate ASC;\r\n";
                DataSet ds = conn.getData(chartquery);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        // Get the original price as decimal
                        decimal originalPrice = Convert.ToDecimal(row["Price"]);

                        // Format the price with commas for every three digits and convert to string
                        string formattedPrice = String.Format("{0:N0}", originalPrice);

                        BillsData2.Add(new Bill2
                        {
                            bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                            Price = formattedPrice // Use the formatted price as a string
                        });
                    }

                }
                this.DataContext = this;
            }
            else
            {
                BillsData2 = new ObservableCollection<Bill2>();
                chartquery = @"
   SELECT 
    bdate, 
    Price 
FROM 
    bills 
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0) AND bdate < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) ORDER BY  bdate ASC;";
                DataSet ds = conn.getData(chartquery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        // Assuming row["Price"] is a string and needs to be converted to decimal for calculations
                        decimal originalPrice = Convert.ToDecimal(row["Price"]);
                        decimal priceInIQD = originalPrice * Properties.Settings.Default.currency;
                        decimal priceWithoutDecimal = Math.Floor(priceInIQD);

                        // Format the price as a string with commas
                        string formattedPrice = String.Format("{0:N0}", priceWithoutDecimal);

                        BillsData2.Add(new Bill2
                        {
                            bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                            Price = formattedPrice // Assign the formatted price string here
                        });
                    }

                }
                this.DataContext = this;
            }
        }

        private void SplineSeries_Loaded1(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.chart_currency == "$")
            {
                BillsData1 = new ObservableCollection<Bill11>();

                chartquery = "SELECT \r\n    bdate, \r\n    Price \r\nFROM \r\n    bills \r\nWHERE dbid = '"+Properties.Settings.Default.dbid+"' and \r\n    bdate >= CAST(GETDATE() AS DATE)\r\n    AND bdate < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))\r\nORDER BY \r\n    bdate ASC;\r\n";
                DataSet ds = conn.getData(chartquery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        // Get the original price as decimal
                        decimal originalPrice = Convert.ToDecimal(row["Price"]);

                        // Format the price with commas for every three digits and convert to string
                        string formattedPrice = String.Format("{0:N0}", originalPrice);

                        BillsData1.Add(new Bill11
                        {
                            bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                            Price = formattedPrice // Use the formatted price as a string
                        });
                    }

                }
                this.DataContext = this;
            }
            else
            {
                BillsData1 = new ObservableCollection<Bill11>();
                chartquery = @"SELECT 
    bdate, 
    Price 
FROM 
    bills 
WHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate >= CAST(GETDATE() AS DATE) AND bdate < DATEADD(DAY, 1, CAST(GETDATE() AS DATE)) ORDER BY  bdate ASC;";
                DataSet ds = conn.getData(chartquery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        // Assuming row["Price"] is a string and needs to be converted to decimal for calculations
                        decimal originalPrice = Convert.ToDecimal(row["Price"]);
                        decimal priceInIQD = originalPrice * Properties.Settings.Default.currency;
                        decimal priceWithoutDecimal = Math.Floor(priceInIQD);

                        // Format the price as a string with commas
                        string formattedPrice = String.Format("{0:N0}", priceWithoutDecimal);

                        BillsData1.Add(new Bill11
                        {
                            bdate = Convert.ToDateTime(row["bdate"]).ToString("yyyy-MM-dd"),
                            Price = formattedPrice // Assign the formatted price string here
                        });
                    }

                }
                this.DataContext = this;
            }
        }
        private async void Label_MouseLeftButtonDown4(object sender, MouseButtonEventArgs e)
        {
            ShowChart(all);
            await Task.Delay(1000);
            year.Visibility = Visibility.Collapsed;
            month.Visibility = Visibility.Collapsed;
            day.Visibility = Visibility.Collapsed;
        }
        private async void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            query = @"
        SELECT 
            s.sid,
            s.supname,
            s.supnum,
           
            (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
        FROM 
            Suppliers s where s.dbid = '"+Properties.Settings.Default.dbid+"' ";
            ds = conn.getData(query);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataGrid.ItemsSource = ds.Tables[0].DefaultView;
            }
        }
        private void Label_MouseLeftButtonDown5(object sender, MouseButtonEventArgs e)
        {
            DataGrid_Loaded(sender, e);
        }
        private void searchBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "🔍  Type the Supplier Name to search ")
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
                searchBox1.Text = "🔍  Type the Supplier Name to search ";
                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
            }
        }

        private async void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (searchBox1.Text == "🔍  Type the Supplier Name to search " || searchBox1.Text == "")
            {
                DataGrid_Loaded(sender, e);

            }
            if (comboBoxText == "Name")
            {
                if (searchBox1.Text == "🔍  Type to search ")
                {
                    query = @"
        SELECT 
            s.sid,
            s.supname,
            s.supnum,
            (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
        FROM 
            Suppliers s where s.dbid = '"+Properties.Settings.Default.dbid+"' ";

                    ds = conn.getData(query);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
                else
                {
                    await Task.Delay(300);
                    string searchText = searchBox1.Text.Trim(); // Get the text from the search box and trim any extra spaces

                    query = $@"
    SELECT 
        s.sid,
        s.supname,
        s.supnum,
        (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
    FROM 
        Suppliers s
    WHERE s.dbid = '"+Properties.Settings.Default.dbid+"' and s.supname LIKE '"+searchText+"%'";

                    ds = conn.getData(query);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
            }
            if (comboBoxText == "PhoneNum")
            {
                if (searchBox1.Text == "🔍  Type to search ")
                {
                    query = @"
        SELECT 
    s.sid,
s.supname,
    s.supnum,
    (SELECT COUNT(*) 
     FROM bills b 
       b.from_ = s.supnum   AND b.type = 'buy') AS SupplierBills FROM  Suppliers s WHERE s.dbid = '"+Properties.Settings.Default.dbid+"';";

                    ds = conn.getData(query);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
                else
                {
                    await Task.Delay(300);

                    string searchText = searchBox1.Text.Trim();

                    query = $@"
    SELECT 
        s.sid,
       s.supname,
        s.supnum,
        (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname  AND b.type = 'buy') AS SupplierBills
    FROM 
        Suppliers s
    WHERE s.dbid = '"+Properties.Settings.Default.dbid+"' and  s.supnum LIKE '"+searchText+"%'";

                    ds = conn.getData(query);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
            }
            if (comboBoxText == "Location")
            {
                if (searchBox1.Text == "🔍  Type to search ")
                {
                    query = @"
        SELECT 
            s.sid,
            s.supname,
            s.supnum,
           
            (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
        FROM 
            Suppliers s where s.dbid = '"+Properties.Settings.Default.dbid+"'" ;

                    ds = conn.getData(query);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
                else
                {
                    await Task.Delay(300);

                    string searchText = searchBox1.Text.Trim();

                    query = $@"
    SELECT 
        s.sid,
       s.supname,
        s.supnum,
        (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
    FROM 
        Suppliers s
    WHERE 
      s.dbid = '"+Properties.Settings.Default.dbid+"' and   s.suplocation LIKE '"+searchText+"%'";

                    ds = conn.getData(query);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
            }
        }
        private async void Button_Clickr(object sender, RoutedEventArgs e)
        {
            AnimateButton(add_Copy1);
            await Task.Delay(200);
            if (Properties.Settings.Default.report_visible == "1")
            {
                report.choose.Opacity = 1;  // Ensure Opacity is reset
                report.reprots1.Opacity = 1;
                report.reprots1.Visibility = Visibility.Visible;
                report.Visibility = Visibility.Visible;
                Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
                fadeOutStoryboard.Begin();
                await Task.Delay(1200);
                Properties.Settings.Default.report_visible = "0";
                Properties.Settings.Default.Save();
            }
        }
        private async void Button_Clicks(object sender, RoutedEventArgs e)
        {
            AnimateButton(add_Copy);
            await Task.Delay(200);
            if (Properties.Settings.Default.setting_visible == "1")
            {
                setting.mainsetting.Visibility = Visibility.Visible;
                setting.Visibility = Visibility.Visible;
                Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
                fadeOutStoryboard.Begin();
                Properties.Settings.Default.setting_visible = "0";
                Properties.Settings.Default.Save();
            }
        }
        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            seeall.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF777777"));
        }

        private void seeall_MouseEnter(object sender, MouseEventArgs e)
        {
            seeall.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F9260"));
        }

        private void ComboBoxAdv_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                comboBoxText = (sender as ComboBox).Text;
                Console.WriteLine(comboBoxText);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void Label_MouseEnter_1(object sender, MouseEventArgs e)
        {
            seeall1.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void seeall1_MouseLeave(object sender, MouseEventArgs e)
        {
            seeall1.Foreground = new SolidColorBrush(Colors.White);

        }

        private void StartGifAnimation()
        {

            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1103-confetti-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(welcomegif, image1);
            ImageBehavior.SetRepeatBehavior(welcomegif, System.Windows.Media.Animation.RepeatBehavior.Forever);




            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1339-sale-loop-roll.gif"));
            ImageBehavior.SetAnimatedSource(salesgif, image2);
            ImageBehavior.SetRepeatBehavior(salesgif, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-290-coin-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(spendgif, image3);
            ImageBehavior.SetRepeatBehavior(spendgif, System.Windows.Media.Animation.RepeatBehavior.Forever);


            var image4 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-160-trending-up-hover-trend-up.gif"));
            ImageBehavior.SetAnimatedSource(profitgif, image4);
            ImageBehavior.SetRepeatBehavior(profitgif, System.Windows.Media.Animation.RepeatBehavior.Forever);


            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-159-trending-down-hover-trend-down.gif"));
            ImageBehavior.SetAnimatedSource(lossgif, image5);
            ImageBehavior.SetRepeatBehavior(lossgif, System.Windows.Media.Animation.RepeatBehavior.Forever);


            var image6 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-56-document (1).gif"));
            ImageBehavior.SetAnimatedSource(billgif, image6);
            ImageBehavior.SetRepeatBehavior(billgif, System.Windows.Media.Animation.RepeatBehavior.Forever);

            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (2).gif"));
            ImageBehavior.SetAnimatedSource(back, image7);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);


        }

        private void Border_Loaded_1(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }

        private void ShowChart(SfChart newChart)
        {
            if (currentChart == newChart && newChart.Visibility == Visibility.Visible)
            {
                return;
            }
            if (currentChart != null && currentChart != newChart)
            {
                Storyboard fadeOutStoryboard = (Storyboard)Resources["FadeOutStoryboard2"];
                fadeOutStoryboard.Completed -= FadeOutCompleted;
                fadeOutStoryboard.Completed += FadeOutCompleted;
                void FadeOutCompleted(object s, EventArgs e)
                {
                    currentChart.Visibility = Visibility.Collapsed;
                    fadeOutStoryboard.Completed -= FadeOutCompleted;
                    newChart.Opacity = 0;
                    newChart.Visibility = Visibility.Visible;
                    Panel.SetZIndex(newChart, 1);
                    Panel.SetZIndex(currentChart, 0);
                    Storyboard fadeInStoryboard = (Storyboard)Resources["FadeInStoryboard2"];
                    fadeInStoryboard.Begin(newChart);
                }

                fadeOutStoryboard.Begin(currentChart);
            }
            else
            {
                newChart.Opacity = 0;
                newChart.Visibility = Visibility.Visible;
                Panel.SetZIndex(newChart, 1);
                if (currentChart != null)
                {
                    Panel.SetZIndex(currentChart, 0);
                }
                Storyboard fadeInStoryboard = (Storyboard)Resources["FadeInStoryboard2"];
                fadeInStoryboard.Begin(newChart);
            }
            currentChart = newChart;
        }

        private void searchBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC9F4F7"));
        }

        private void searchBox1_MouseLeave(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush(Colors.White);
        }

        private void AnimateButton(Button targetButton)
        {
            if (targetButton != null)
            {
                Storyboard storyboard = (Storyboard)this.Resources["ClickEffectStoryboard"];
                Storyboard.SetTarget(storyboard, targetButton);
                storyboard.Begin();
            }
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InitializeComponent();
            DataGrid_Loaded(sender, e);
            SplineSeries_Loaded(sender, e);
            SplineSeries_Loaded1(sender, e);
            SplineSeries_Loaded2(sender, e);
            SplineSeries_Loaded3(sender, e);
            UserControl_Loaded(sender, e);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure that a row is selected
            if (DataGrid.SelectedItem != null)
            {
                // Assuming the DataGrid is bound to a DataTable and each row is a DataRowView
                DataRowView selectedRow = DataGrid.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    // Get the supplier number (supnum) from the selected row
                    string supnum = selectedRow["supnum"].ToString();

                    // Remove any leading zeros from the number (if applicable)
                    supnum = supnum.TrimStart('0');

                    // Format the number to include the Iraq country code +964
                    string iraqNumber = $"+964{supnum}";

                    // Launch WhatsApp chat with the formatted number
                    OpenWhatsAppChat(iraqNumber);
                }
            }
        }

        // Method to open WhatsApp chat
        private void OpenWhatsAppChat(string phoneNumber)
        {
            try
            {
                // Create the WhatsApp app schema (for desktop app)
                string whatsappDesktopUrl = $"whatsapp://send?phone={phoneNumber}";

                // Try to start WhatsApp desktop app
                var processInfo = new ProcessStartInfo
                {
                    FileName = whatsappDesktopUrl,
                    UseShellExecute = true
                };

                // Start WhatsApp desktop app, if installed
                System.Diagnostics.Process.Start(processInfo);
            }
            catch (Exception)
            {
                // If WhatsApp desktop is not installed, open the web version
                OpenWhatsAppWeb(phoneNumber);
            }
        }

        // Method to open WhatsApp Web
        private void OpenWhatsAppWeb(string phoneNumber)
        {
            try
            {
                // Create the WhatsApp Web URL schema
                string whatsappWebUrl = $"https://wa.me/{phoneNumber}";

                // Open the URL in the default browser
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = whatsappWebUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open WhatsApp chat: {ex.Message}");
            }
        }


    }
}
