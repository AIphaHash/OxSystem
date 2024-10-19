using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Printing;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfAnimatedGif;
using static OxSystem.main_background;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for main_background.xaml
    /// </summary>
    public partial class main_background : UserControl
    {
        private bool isDataLoadedForMedicData = false;

        private bool isDataLoaded = false;
        private bool isDataLoadedForLowestMedics = false;


        private string comboBoxText;

        public class TransactionData
        {
            public string TransactionType { get; set; }
            public decimal Amount { get; set; }
        }
        public class BuySellDataModel
        {
            public string BillType { get; set; }  // Buy or Sell
            public int BillCount { get; set; }    // Number of items
        }



        public class StorageMedicData
        {
            public string Sname { get; set; }
            public int MedicCount { get; set; }
        }
        public class MedicExpirationData
        {
            public string ExpirationPeriod { get; set; }
            public int Count { get; set; }

        }
        public class MedicInfo
        {
            public string Mname { get; set; } // Represents the medic name
            public int Nummedic { get; set; } // Represents the quantity
        }
        private string _currentTime;
        private DispatcherTimer _timer;
        private bool isFlipped = false;
        private bool isChartVisible = true;
        string query;
        DbConnection conn = new DbConnection();
        DataTable dt;
        DataSet ds;
        public static string full = Login_.fullName;
        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        public main_background()
        {
            InitializeComponent();
            LoadChartData1();
            AdminSeries1.ItemsSource = GetMedicData();
            LoadChartData();
            LoadMedicExpirationData();
            DataContext = this;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            UpdateCurrentTime();

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
        private void LoadChartData1()
        {
            try
            {
                DbConnection db = new DbConnection();
                string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

                // Query to get today's transactions, grouping by type (buy/sell)
                string query = $@"
                    SELECT type, SUM(Price) AS TotalAmount
                    FROM bills
                    WHERE dbid = '"+Properties.Settings.Default.dbid+"' and bdate = '"+todayDate+"' GROUP BY type";

                DataSet ds = db.getData(query);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    List<TransactionData> chartData = new List<TransactionData>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string type = row["type"].ToString();
                        decimal amount = Convert.ToDecimal(row["TotalAmount"]);

                        chartData.Add(new TransactionData
                        {
                            TransactionType = type == "buy" ? "Spend" : "Profit",
                            Amount = amount
                        });
                    }

                    // Bind data to the pieChart (which is now a bar chart)
                    pieChart.DataContext = chartData;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the chart data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadSellBuyDataForChart()
        {
            try
            {
                // Define your SQL query to count buy/sell items for the current user
                string currentUsername = Login_.fullName;  // Assuming this stores the current user
                string query = $@"
            SELECT 
                type AS BillType, 
                COUNT(medicinfo.mid) AS BillCount 
            FROM 
                bills 
            INNER JOIN 
                medicinfo ON bills.billId = medicinfo.billId 
            WHERE bills.dbid = '"+Properties.Settings.Default.dbid+"' and bills.by_ = '"+currentUsername+"'   AND bills.bdate = CAST(GETDATE() AS DATE) GROUP BY type";

                // Retrieve the data using the getData method from DbConnection
                DbConnection db = new DbConnection();
                DataSet ds = db.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // Create a list to store the data in a format suitable for the chart
                    var buySellData = new List<BuySellDataModel>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        buySellData.Add(new BuySellDataModel
                        {
                            BillType = row["BillType"].ToString(),
                            BillCount = Convert.ToInt32(row["BillCount"])
                        });
                    }

                    // Bind the data to the chart
                    BillsChart.DataContext = buySellData;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public List<MedicExpirationData> GetMedicExpirationData()
        {
            List<MedicExpirationData> dataList = new List<MedicExpirationData>();

            string query = @"
        SELECT 
    CASE 
        WHEN DATEDIFF(day, GETDATE(), exdate) <= 3 THEN 'Less than 3 days'
        WHEN DATEDIFF(day, GETDATE(), exdate) > 3 AND DATEDIFF(day, GETDATE(), exdate) <= 7 THEN 'More than 3 days and less than 1 week'
        WHEN DATEDIFF(day, GETDATE(), exdate) > 7 AND DATEDIFF(day, GETDATE(), exdate) <= 30 THEN 'More than 1 week and less than 1 month'
        ELSE 'More than 1 month'
    END AS ExpirationPeriod,
    COUNT(*) AS Count
FROM medicinfo
WHERE dbid = '" + Properties.Settings.Default.dbid + "' GROUP BY  CASE  WHEN DATEDIFF(day, GETDATE(), exdate) <= 3 THEN 'Less than 3 days' WHEN DATEDIFF(day, GETDATE(), exdate) > 3 AND DATEDIFF(day, GETDATE(), exdate) <= 7 THEN 'More than 3 days and less than 1 week' WHEN DATEDIFF(day, GETDATE(), exdate) > 7 AND DATEDIFF(day, GETDATE(), exdate) <= 30 THEN 'More than 1 week and less than 1 month' ELSE 'More than 1 month' END;";

            DataSet ds = conn.getData(query); // Assume getData returns a DataSet
            DataTable dt = ds.Tables[0]; // Getting the first DataTable from DataSet

            foreach (DataRow row in dt.Rows)
            {
                dataList.Add(new MedicExpirationData
                {
                    ExpirationPeriod = row["ExpirationPeriod"].ToString(),
                    Count = int.Parse(row["Count"].ToString())
                });
            }

            return dataList;
        }

        public void LoadMedicExpirationData()
        {
            AdminSeries11.ItemsSource = GetMedicExpirationData();
        }




        public List<StorageMedicData> GetStorageMedicData()
        {
            List<StorageMedicData> dataList = new List<StorageMedicData>();
            string query = @"
        SELECT si.sname, SUM(mi.nummedic) AS MedicCount
FROM storageinfo si
INNER JOIN medicinfo mi ON si.sname = mi.sname
WHERE mi.dbid = '" + Properties.Settings.Default.dbid + @"'
GROUP BY si.sname;"
;

            DataSet ds = conn.getData(query); // Assume getData returns a DataSet
            DataTable dt = ds.Tables[0]; // Getting the first DataTable from DataSet

            foreach (DataRow row in dt.Rows)
            {
                dataList.Add(new StorageMedicData
                {
                    Sname = row["sname"].ToString(),
                    MedicCount = int.Parse(row["MedicCount"].ToString())
                });
            }

            return dataList;
        }

        public void LoadChartData()
        {
            StorageSeries.ItemsSource = GetStorageMedicData();
        }


        public List<MedicInfo> GetMedicData()
        {
            List<MedicInfo> medicList = new List<MedicInfo>();
            query = "SELECT top 20 mname, nummedic FROM medicinfo where dbid = '"+Properties.Settings.Default.dbid+"'"; // Filter in SQL query
            ds = conn.getData(query); // Assume getData returns a DataSet
            dt = ds.Tables[0]; // Getting the first DataTable from DataSet

            foreach (DataRow row in dt.Rows)
            {
                medicList.Add(new MedicInfo
                {
                    Mname = row["mname"].ToString(),
                    Nummedic = int.Parse(row["nummedic"].ToString())
                });
            }

            return medicList;
        }



        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Clicka(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void thisyear_MouseEnter(object sender, MouseEventArgs e)
        {
            thisyear.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCBD7F1"));
        }

        private void thisyear_MouseLeave(object sender, MouseEventArgs e)
        {
            thisyear.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F7FE"));
        }

        private void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            if (isDataLoaded)
            {
                return;
            }
            AnimateBorder(thisyear);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            thisday.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCBD7F1"));
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            thisday.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F7FE"));
        }
        private void AnimateBorder(Border targetBorder)
        {
            if (targetBorder != null)
            {
                // Retrieve the storyboard from resources
                Storyboard storyboard = (Storyboard)this.Resources["ClickEffectStoryboard"];

                // Apply the storyboard to the target border
                Storyboard.SetTarget(storyboard, targetBorder);

                // Start the animation
                storyboard.Begin();
            }
        }
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDataLoadedForLowestMedics)
            {
                return;
            }

            AnimateBorder(thisday);
        }

        private void SplineSeries_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SplineSeries_Loaded1(object sender, RoutedEventArgs e)
        {

        }

        private void SplineSeries_Loaded2(object sender, RoutedEventArgs e)
        {

        }

        private void SplineSeries_Loaded3(object sender, RoutedEventArgs e)
        {

        }

        private async void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            fullnameLabel.Content = pharmacist.fulln;
           
            welcome_Copy.Content = pharmacist.fulln;

            if (Properties.Settings.Default.check_currency == "$")
            {
                // Query to calculate the total number of bills
                query = @"
       SELECT 
    ISNULL(SUM(mh.quantity), 0) AS TotalMedicsBought
FROM 
    bills b
INNER JOIN 
    medichistory mh ON b.billId = mh.billId
WHERE b.dbid = '"+Properties.Settings.Default.dbid+"' and b.type = 'buy' AND CONVERT(DATE, b.bdate) = CONVERT(DATE, GETDATE());";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalMedicsBought"];
                    if (result != DBNull.Value)
                    {
                        // Convert the result to an integer and display it
                        int totalMedicsPurchased = Convert.ToInt32(result);
                        totalbills.Content = totalMedicsPurchased.ToString(); // Assuming 'totalbills' is the name of the label
                    }
                    else
                    {
                        totalbills.Content = "0"; // Default if there are no purchases or result is null
                    }
                }
                else
                {
                    totalbills.Content = "0"; // Default if there are no records or result is null
                }


                // Query to calculate the loss (buy - sell)
                query = @"
SELECT 
            ISNULL(SUM(Price), 0) AS TotalSellPrice
        FROM 
            bills
        WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'buy' AND bdate = CAST(GETDATE() AS DATE);";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSellPrice"];
                    if (result != DBNull.Value)
                    {
                        // Convert the result to decimal and display as currency without braces
                        decimal loss = Convert.ToDecimal(result);
                        loss_.Content = loss.ToString("C", CultureInfo.CurrentCulture)
                                            .Replace("(", "-")   // Replace ( with -
                                            .Replace(")", "");   // Remove closing parenthesis
                    }
                    else
                    {
                        loss_.Content = "$0.00"; // Default if there is no loss
                    }
                }
                else
                {
                    loss_.Content = "$0.00"; // Default if there are no records or result is null
                }


                query = @"
        SELECT 
            ISNULL(SUM(Price), 0) AS TotalSellPrice
        FROM 
            bills
        WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'sell' AND bdate = CAST(GETDATE() AS DATE);";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSellPrice"];
                    if (result != DBNull.Value)
                    {
                        // Convert the result to decimal and display as currency
                        decimal totalSellPrice = Convert.ToDecimal(result);
                        profit_.Content = totalSellPrice.ToString("C", CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        profit_.Content = "$0.00"; // Default if there are no sell transactions or result is null
                    }
                }
                else
                {
                    profit_.Content = "$0.00"; // Default if there are no records or result is null
                }





                query = @"
SELECT 
    ISNULL(SUM(mh.quantity), 0) AS TotalMedicsSold
FROM 
    bills b
INNER JOIN 
    medichistory mh ON b.billId = mh.billId
WHERE b.dbid = '"+Properties.Settings.Default.dbid+"' and b.type = 'sell' AND CONVERT(DATE, b.bdate) = CONVERT(DATE, GETDATE());";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalMedicsSold"];
                    if (result != DBNull.Value)
                    {
                        int totalMedicsSold = Convert.ToInt32(result); // Convert to an integer
                        spends.Content = totalMedicsSold.ToString(); // Update label with the total
                    }
                    else
                    {
                        spends.Content = "0"; // Default if no medics sold
                    }
                }
                else
                {
                    spends.Content = "0"; // Default if no records or result is null
                }



                query = @"
        SELECT m.mname, SUM(b.Price) AS TotalSales
        FROM medicinfo m
        INNER JOIN bills b ON m.billId = b.billId
        WHERE b.dbid = '"+Properties.Settings.Default.dbid+"' and b.type = 'sell' AND b.bdate = CAST(GETDATE() AS DATE) GROUP BY m.mname ORDER BY TotalSales DESC";

                // Fetch data from the database
                ds = conn.getData(query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // Get the most sold medic
                    DataRow topMedicRow = ds.Tables[0].Rows[0];
                    string mostSoldMedicName = topMedicRow["mname"].ToString();

                    // Update the salesLabel with the result
                    sales.Content = $"Most Sold Medic Today: {mostSoldMedicName}";
                }
                else
                {
                    // Handle cases where no data is available
                    sales.Content = "No Sales Data Available";
                }




                query = @"
        SELECT 
            s.sid,
            s.supname,
            s.supnum,
           
            (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
        FROM 
            Suppliers s where dbid = '"+Properties.Settings.Default.dbid+"'";

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
    ISNULL(SUM(mh.quantity), 0) AS TotalMedicsBought
FROM 
    bills b
INNER JOIN 
    medichistory mh ON b.billId = mh.billId
WHERE b.dbid = '"+Properties.Settings.Default.dbid+"' and b.type = 'buy' AND CONVERT(DATE, b.bdate) = CONVERT(DATE, GETDATE());";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalMedicsBought"];
                    if (result != DBNull.Value)
                    {
                        // Convert the result to an integer and display it
                        int totalMedicsPurchased = Convert.ToInt32(result);
                        totalbills.Content = totalMedicsPurchased.ToString(); // Assuming 'totalbills' is the name of the label
                    }
                    else
                    {
                        totalbills.Content = "0"; // Default if there are no purchases or result is null
                    }
                }
                else
                {
                    totalbills.Content = "0"; // Default if there are no records or result is null
                }



                query = @"
SELECT 
            ISNULL(SUM(Price), 0) AS TotalSellPrice
        FROM 
            bills
        WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'buy' AND bdate = CAST(GETDATE() AS DATE);";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSellPrice"];
                    if (result != DBNull.Value)
                    {
                        // Convert the result to decimal
                        decimal lossUSD = Convert.ToDecimal(result);

                        // Convert the loss from USD to IQD
                        decimal lossIQD = lossUSD * 1300;

                        // Display the result in IQD with a negative sign for losses
                        loss_.Content = lossIQD.ToString("N0", CultureInfo.CurrentCulture)
                                               .Replace("(", "-")   // Replace ( with -
                                               .Replace(")", "") + " IQD";  // Remove closing parenthesis and add IQD
                    }
                    else
                    {
                        loss_.Content = "0 IQD"; // Default if there is no loss
                    }
                }
                else
                {
                    loss_.Content = "0 IQD"; // Default if there are no records or result is null
                }

                query = @"
 SELECT 
            ISNULL(SUM(Price), 0) AS TotalSellPrice
        FROM 
            bills
        WHERE dbid = '"+Properties.Settings.Default.dbid+"' and type = 'sell' AND bdate = CAST(GETDATE() AS DATE);";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalSellPrice"];
                    if (result != DBNull.Value)
                    {
                        // Convert the result to decimal
                        decimal profitUSD = Convert.ToDecimal(result);

                        // Convert the profit from USD to IQD
                        decimal profitIQD = profitUSD * 1300;

                        // Display the result in IQD with a negative sign for losses (if any)
                        profit_.Content = profitIQD.ToString("N0", CultureInfo.CurrentCulture)
                                                   .Replace("(", "-")   // Replace ( with -
                                                   .Replace(")", "") + " IQD";  // Remove closing parenthesis and add IQD
                    }
                    else
                    {
                        profit_.Content = "0 IQD"; // Default if there is no profit or loss
                    }
                }
                else
                {
                    profit_.Content = "0 IQD"; // Default if there are no records or result is null
                }

                query = @"
SELECT 
    ISNULL(SUM(mh.quantity), 0) AS TotalMedicsSold
FROM 
    bills b
INNER JOIN 
    medichistory mh ON b.billId = mh.billId
WHERE b.dbid = '"+Properties.Settings.Default.dbid+"' and b.type = 'sell' AND CONVERT(DATE, b.bdate) = CONVERT(DATE, GETDATE());";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["TotalMedicsSold"];
                    if (result != DBNull.Value)
                    {
                        int totalMedicsSold = Convert.ToInt32(result); // Convert to an integer
                        spends.Content = totalMedicsSold.ToString(); // Update label with the total
                    }
                    else
                    {
                        spends.Content = "0"; // Default if no medics sold
                    }
                }
                else
                {
                    spends.Content = "0"; // Default if no records or result is null
                }



                query = @"
        SELECT 
            s.sid,
            s.supname,
            s.supnum,
           
            (SELECT COUNT(*) FROM bills b WHERE b.from_ = s.supname AND b.type = 'buy') AS SupplierBills
        FROM 
            Suppliers s where dbid = '"+Properties.Settings.Default.dbid+"'";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                }





                query = @"
        SELECT m.mname, SUM(b.Price) AS TotalSales
        FROM medicinfo m
        INNER JOIN bills b ON m.billId = b.billId
        WHERE b.dbid = '"+Properties.Settings.Default.dbid+"' and b.type = 'sell' AND b.bdate = CAST(GETDATE() AS DATE) GROUP BY m.mname ORDER BY TotalSales DESC ";

                // Fetch data from the database
                ds = conn.getData(query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // Get the most sold medic
                    DataRow topMedicRow = ds.Tables[0].Rows[0];
                    string mostSoldMedicName = topMedicRow["mname"].ToString();

                    // Update the salesLabel with the result
                    sales.Content = $"Most Sold Medic Today: {mostSoldMedicName}";
                }
                else
                {
                    // Handle cases where no data is available
                    sales.Content = "No Sales Data Available for Today";
                }
            }

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

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private async void Label_MouseLeftButtonDown4(object sender, MouseButtonEventArgs e)
        {
            if (isDataLoadedForMedicData)
            {
                // If data is already loaded, exit the method
                return;
            }
            thisday.IsEnabled = false;
            thisyear.IsEnabled = false;
            isDataLoadedForMedicData = true;
            isDataLoaded = false;
            isDataLoadedForLowestMedics = false;
            // Start the fade out/in animation
            Storyboard storyboard = (Storyboard)this.Resources["FadeOutInStoryboard"];
            storyboard.Begin();

            // Wait for the animation to complete
            await Task.Delay(500);
            thisday.IsEnabled = true;
            thisyear.IsEnabled = true;
            // Update the chart data
            ChangeChart_Click1(sender, e);
            AdminSeries1.ItemsSource = GetMedicData();

            // Set the flag to indicate that data has been loaded

        }


        private void Label_MouseLeftButtonDown5(object sender, MouseButtonEventArgs e)
        {
            DataGrid_Loaded(sender, e);

        }

        private void searchBox1_LostFocus(object sender, RoutedEventArgs e)
        {

            if (searchBox1.Text == "")
            {
                searchBox1.Text = "🔍  Type the Supplier Name to search ";
                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
            }
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
            Suppliers s where dbid = '"+Properties.Settings.Default.dbid+"'";

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
    WHERE dbid = '"+Properties.Settings.Default.dbid+"' and s.supname LIKE '%"+searchText+"%'";

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
     WHERE b.from_ = s.supnum dbid = '"+Properties.Settings.Default.dbid+"' AND b.type = 'buy') AS SupplierBills FROM  Suppliers s;";

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
    WHERE dbid = '"+Properties.Settings.Default.dbid+"' and s.supnum LIKE '%{searchText}%'";

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
            Suppliers s where dbid = '"+Properties.Settings.Default.dbid+"'";

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
        s.suplocation LIKE '%{searchText}%' dbid = '"+Properties.Settings.Default.dbid+"'";

                    ds = conn.getData(query);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                }
            }



        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var medicData = GetMedicData(); // Fetch data from the database
            AdminSeries1.ItemsSource = medicData;

        }

        private void Label_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {

            // Slide out chart and slide in label
            Storyboard slideOut = (Storyboard)this.Resources["SlideOutChart"];
            slideOut.Begin();

            // Slide in chart and slide out label


        }

        private async void thisyear_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDataLoaded)
            {
                // If data is already loaded, exit the method
                return;
            }
            see.IsEnabled = false;
            thisday.IsEnabled = false;
            isDataLoaded = true;
            isDataLoadedForLowestMedics = false;
            isDataLoadedForMedicData = false;
            // Start the fade out/in animation
            Storyboard storyboard = (Storyboard)this.Resources["FadeOutInStoryboard"];
            storyboard.Begin();

            // Wait for the animation to complete
            await Task.Delay(500);
            see.IsEnabled = true;
            thisday.IsEnabled = true;
            // Update the chart data
            ChangeChart_Click1(sender, e);
            AdminSeries1.ItemsSource = GetTop10HighestMedics();

            // Set the flag to indicate that data has been loaded

        }


        private async void thisday_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDataLoadedForLowestMedics)
            {
                // If data is already loaded, exit the method
                return;
            }
            see.IsEnabled = false;
            thisyear.IsEnabled = false;
            // Start the fade out/in animation
            Storyboard storyboard = (Storyboard)this.Resources["FadeOutInStoryboard"];
            storyboard.Begin();
            isDataLoadedForLowestMedics = true;
            isDataLoaded = false;
            isDataLoadedForMedicData = false;
            // Wait for the animation to complete
            await Task.Delay(500);
            see.IsEnabled = true;
            thisyear.IsEnabled = true;
            // Update the chart data
            ChangeChart_Click(sender, e);
            AdminSeries1.ItemsSource = GetTop10LowestMedics();

            // Set the flag to indicate that data has been loaded

        }

        public List<MedicInfo> GetTop10HighestMedics()
        {
            List<MedicInfo> medicList = new List<MedicInfo>();
            string query = "SELECT TOP 10 mname, nummedic FROM medicinfo where dbid = '"+Properties.Settings.Default.dbid+"' ORDER BY nummedic DESC"; // Top 10 highest
            DataSet ds = conn.getData(query);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                medicList.Add(new MedicInfo
                {
                    Mname = row["mname"].ToString(),
                    Nummedic = int.Parse(row["nummedic"].ToString())
                });
            }

            return medicList;
        }
        private void ChangeChart_Click(object sender, RoutedEventArgs e)
        {
            // Update the data source of the chart


            // Change the brush of the column series dynamically
            var newBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };
            newBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0x65, 0x24, 0x24), 0.0)); // #FF652424
            newBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0x86, 0x56, 0x56), 1.0)); // #FF865656

            // Set the new brush to the Interior of the series
            AdminSeries1.Interior = newBrush;
        }
        private void ChangeChart_Click1(object sender, RoutedEventArgs e)
        {
            // Change back to the original brush
            var originalBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };
            originalBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0x3A, 0x91, 0x91), 0.0)); // #FF3A9191
            originalBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0x44, 0xAF, 0xAF), 1.0)); // #FF44AFAF

            // Set the original brush back to the Interior of the series
            AdminSeries1.Interior = originalBrush;
        }

        public List<MedicInfo> GetTop10LowestMedics()
        {
            List<MedicInfo> medicList = new List<MedicInfo>();
            string query = "SELECT top 10 mname, nummedic FROM medicinfo where dbid = '"+Properties.Settings.Default.dbid+"' and nummedic <=15 ORDER BY nummedic ASC";
            DataSet ds = conn.getData(query);
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                if (int.TryParse(row["nummedic"].ToString(), out int nummedic))
                {
                    medicList.Add(new MedicInfo
                    {
                        Mname = row["mname"].ToString(),
                        Nummedic = nummedic
                    });
                }
                else
                {
                    // Handle cases where nummedic is not a valid integer
                    Console.WriteLine($"Invalid nummedic value: {row["nummedic"]}");
                    // You could choose to skip this entry or set a default value
                }
            }

            return medicList;
        }

        private void Border_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadSellBuyDataForChart();
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
        }

        private void StartGifAnimation()
        {

            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1103-confetti-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(welcomegif, image1);
            ImageBehavior.SetRepeatBehavior(welcomegif, System.Windows.Media.Animation.RepeatBehavior.Forever);




            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1780-medal-first-place-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(salesgif, image2);
            ImageBehavior.SetRepeatBehavior(salesgif, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-453-savings-pig-hover-pinch (1).gif"));
            ImageBehavior.SetAnimatedSource(spendgif, image3);
            ImageBehavior.SetRepeatBehavior(spendgif, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image4 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-5-wallet-hover-wallet.gif"));
            ImageBehavior.SetAnimatedSource(profitgif, image4);
            ImageBehavior.SetRepeatBehavior(profitgif, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-160-trending-up-hover-trend-up.gif"));
            ImageBehavior.SetAnimatedSource(lossgif, image5);
            ImageBehavior.SetRepeatBehavior(lossgif, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image6 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-159-trending-down-hover-trend-down.gif"));
            ImageBehavior.SetAnimatedSource(billgif, image6);
            ImageBehavior.SetRepeatBehavior(billgif, System.Windows.Media.Animation.RepeatBehavior.Forever);

        }

        private void Border_Loaded_2(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }

        private void Label_MouseLeftButtonDown_31(object sender, MouseButtonEventArgs e)
        {
            Storyboard slideIn = (Storyboard)this.Resources["SlideInChart"];
            slideIn.Begin();

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
            seeall1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F9260"));

        }

        private void seeall1_MouseLeave(object sender, MouseEventArgs e)
        {
            seeall1.Foreground = new SolidColorBrush(Colors.White);

        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            see.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF545856"));
        }

        private void see_MouseLeave(object sender, MouseEventArgs e)
        {
            see.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F9260"));
        }

        private void thisday_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void searchBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD4EDED"));
        }

        private void searchBox1_MouseLeave(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush(Colors.White);
        }

        private async void Button_Clicks(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);

            setting.mainsetting.Visibility = Visibility.Visible;

            setting.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
            fadeOutStoryboard.Begin();
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private async void Button_Clickr(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            report.choose.Opacity = 1;  // Ensure Opacity is reset
            report.reprots1.Opacity = 1;
            report.reprots1.Visibility = Visibility.Visible;

            report.Visibility = Visibility.Visible;

            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            fadeOutStoryboard.Begin();

            await Task.Delay(1200);
        }
    }
}
