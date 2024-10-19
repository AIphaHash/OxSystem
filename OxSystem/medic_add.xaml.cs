using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using SciChart.Core;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using WpfTable = System.Windows.Documents.Table;
using PdfTable = iText.Layout.Element.Table;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media.Animation;
using static OxSystem.AdminDashboard;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Globalization;
using static OxSystem.medic_add;
using iText.Forms.Form.Element;
using WpfAnimatedGif;
using Org.BouncyCastle.Math;
using static OxSystem.sellmedic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using PdfSharp.Pdf.Content.Objects;









namespace OxSystem
{
    /// <summary>
    /// Interaction logic for medic_add.xaml
    /// </summary>
    public partial class medic_add : UserControl
    {
        private string barcodeInput = string.Empty; // To accumulate the barcode input
        private Stopwatch inputTimer = new Stopwatch(); // To measure the time between key inputs
        private const int barcodeThresholdMs = 50; // Threshold time in milliseconds


        private string _currentTime;
        private DispatcherTimer _timer;
        private List<string> scannedItems;
        private ICollectionView _collectionView;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public static string storagename;
        public static string sn;
        private decimal totalBuyPrice = 0;
        public static string brcode;
        public int maxid;
        public int bid;
        public static string supn;
        public static string recn;
        public static string full = Login_.fullName;

        public medic_add()
        {
            InitializeComponent();
            DataContext = this;
            UpdateCurrentTime();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            LoadStorageNames();
            scannedItems = new List<string>();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear && year <= 2100; year++)
            {
                YearComboBox.Items.Add(year);

            }
            for (int year1 = 2000; year1 >= 2000 && year1 <= currentYear; year1++)
            {
                YearComboBox1.Items.Add(year1);


            }

            // Populate MonthComboBox
            for (int month = 1; month <= 12; month++)
            {
                MonthComboBox.Items.Add(month);
                MonthComboBox1.Items.Add(month);
            }

            // Set default selections to today's date


            PopulateDays();


        }

        public void PopulateDays()
        {
            if (YearComboBox.SelectedItem == null || MonthComboBox.SelectedItem == null)
                return;

            int year = (int)YearComboBox.SelectedItem;
            int month = (int)MonthComboBox.SelectedItem;

            int daysInMonth = DateTime.DaysInMonth(year, month);

            DayComboBox.Items.Clear();

            for (int day = 1; day <= daysInMonth; day++)
            {
                DayComboBox.Items.Add(day);
            }
        }
        private void PopulateDays1()
        {
            if (YearComboBox1.SelectedItem == null || MonthComboBox1.SelectedItem == null)
                return;

            int year = (int)YearComboBox1.SelectedItem;
            int month = (int)MonthComboBox1.SelectedItem;

            int daysInMonth = DateTime.DaysInMonth(year, month);

            DayComboBox1.Items.Clear();
            for (int day = 1; day <= daysInMonth; day++)
            {
                DayComboBox1.Items.Add(day);
            }
        }

        public List<string> GetStorageNames()
        {
            List<string> storageNames = new List<string>();
            string query = "select sname from storageinfo where dbid = '"+Properties.Settings.Default.dbid+"'";

            // Assuming you have a method in your connection class to get data
            DataSet ds = conn.getData(query);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    storageNames.Add(row["sname"].ToString());
                }
            }

            return storageNames;
        }

        private void LoadStorageNames()
        {
            List<string> storageNames = GetStorageNames();
            sname.ItemsSource = storageNames;
        }

        public class MedicInfo
        {
            public string MedicName { get; set; }
            public string BuyPrice { get; set; }
            public string SellPrice { get; set; }
            public string NumberMedic { get; set; }
            public string ExpiryDate { get; set; }
            public string ManufactureDate { get; set; }
            public string Sname { get; set; } // Add this property
            public string SillBill { get; set; }

            public string BuyBill { get; set; }

            public string Codeid { get; set; }


        }


        public async void mybutton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);

            // Check if QR code (barcode) is missing
            if (string.IsNullOrWhiteSpace(brcode))
            {
                MessageBoxResult result = MessageBox.Show("Do you want to proceed without a QR code?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    AddDataToDataGrid();  // Proceed without QR code

                }
                else
                {
                    MessageBox.Show("Please add QR code.");
                    return;  // Stop execution if the user doesn't want to proceed
                }
            }
            else
            {
                // Check if the barcode already exists in the database
                string queryCheckBarcode = $"SELECT nummedic FROM medicinfo WHERE dbid = '"+Properties.Settings.Default.dbid+"' and Codeid = '{brcode}'";

                // Use DataSet to fetch data
                DataSet ds = conn.getData(queryCheckBarcode); // Assuming conn.getDataSet returns a DataSet

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // Barcode already exists
                    string existingQuantity = ds.Tables[0].Rows[0]["nummedic"].ToString();
                    MessageBoxResult updateResult = MessageBox.Show("There is already an item with this medic. Do you want to update the quantity of the item?", "Item Exists", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (updateResult == MessageBoxResult.Yes)
                    {
                        // Parse new quantity
                        if (int.TryParse(num.Text, out int newQuantity) && int.TryParse(existingQuantity, out int existingNum))
                        {
                            int updatedQuantity = existingNum + newQuantity;

                            // Update quantity in the database
                            string updateQuery = $"UPDATE medicinfo SET nummedic = {updatedQuantity} WHERE dbid = '"+Properties.Settings.Default.dbid+"' and Codeid = '{brcode}'";
                            conn.setData(updateQuery);
                            query = "SELECT TOP 1 mid\r\nFROM medicinfo where dbid = '"+Properties.Settings.Default.dbid+"'\r\nORDER BY mid DESC;\r\n";
                            ds = conn.getData(query);
                            int mid = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                           
                            query = "insert into medichistory values( '" + newQuantity + "' , '" + bid + "' , '" + currentDate + "' , '" + mid + "' ,'"+Properties.Settings.Default.dbid+"')";
                            conn.setData(query);
                            // Update the DataGrid
                            AddDataToDataGrid();  // Add the item to DataGrid after updating the quantity
                        }
                    }
                    else
                    {
                        // User declined the update, show message or simply exit
                        MessageBox.Show("Item not added.");
                        return;
                    }
                }
                else
                {
                    // No existing item with the same barcode, proceed normally
                    AddDataToDataGrid();
                }
            }
        }


        private void AddDataToDataGrid()
        {
            // Validate inputs
            if (mname.Text != "" && mname_Copy.Text != "" && sprice.Text != "" && bprice.Text != "" && num.Text != "" && scififcename.Text != "" && type.Text != "" &&
                supname.SelectedIndex >= 0 && sname.SelectedIndex >= 0 && YearComboBox1.SelectedIndex >= 0 &&
                MonthComboBox.SelectedIndex >= 0 && MonthComboBox1.SelectedIndex >= 0 && DayComboBox.SelectedIndex >= 0 && DayComboBox1.SelectedIndex >= 0 &&
                mname.Text != "insert the Medic Name!" && mname_Copy.Text != "insert the Too Name!" && num.Text != "9999" && scififcename.Text != "insert the Scinfic Name!" && type.Text != "insert the Type Name!" &&
                sname.Text != "999,999" && supname.Text != "999,99")
            {
                supname.IsEnabled = false;
                string medicname = mname.Text;

                // Remove commas from bprice and sprice
                string buy = bprice.Text;
                string sell = sprice.Text;

                // Convert to decimal and format with two decimal places
                if (decimal.TryParse(buy, out decimal buyValue))
                {
                    buy = buyValue.ToString("N2", CultureInfo.InvariantCulture);
                }
                if (decimal.TryParse(sell, out decimal sellValue))
                {
                    sell = sellValue.ToString("N2", CultureInfo.InvariantCulture);
                }

                string mnum = num.Text;
                string sb = supn;
                string bb = mname_Copy.Text;

                // Calculate expiry and manufacture dates
                int yearx = (int)YearComboBox.SelectedItem;
                int monthx = (int)MonthComboBox.SelectedItem;
                int dayx = (int)DayComboBox.SelectedItem;
                string datex = new DateTime(yearx, monthx, dayx).ToString("yyyy-MM-dd");

                int yearm = (int)YearComboBox1.SelectedItem;
                int monthm = (int)MonthComboBox1.SelectedItem;
                int daym = (int)DayComboBox1.SelectedItem;
                string datem = new DateTime(yearm, monthm, daym).ToString("yyyy-MM-dd");

                // Try parsing quantity as an integer
                if (int.TryParse(mnum, out int quantity))
                {
                    decimal totalRowPrice = buyValue * quantity; // Multiply buy price by quantity

                    // Create a new MedicInfo object
                    var newMedic = new MedicInfo
                    {
                        MedicName = medicname,
                        BuyPrice = buy, // Store the original buy price (not multiplied)
                        SellPrice = sell,
                        NumberMedic = mnum,
                        ExpiryDate = datex,
                        ManufactureDate = datem,
                        Sname = sn,
                        SillBill = supn,
                        BuyBill = bb,
                        Codeid = brcode
                    };

                    // Ensure DataGrid1 is bound to an ObservableCollection
                    var medicList = DataGrid1.ItemsSource as ObservableCollection<MedicInfo>;
                    if (medicList == null)
                    {
                        medicList = new ObservableCollection<MedicInfo>();
                        DataGrid1.ItemsSource = medicList;
                    }

                    // Add the new item to the collection
                    medicList.Add(newMedic);

                    // Update the total buy price and pricet label
                    totalBuyPrice += totalRowPrice; // Update total price by multiplying buy price by quantity

                    // Update pricet label
                    pricet.Content = totalBuyPrice.ToString("N2", CultureInfo.InvariantCulture) + " $";
                }

                // Insert the new item into the database
                if (!string.IsNullOrWhiteSpace(medicname) && !string.IsNullOrWhiteSpace(mnum) && !string.IsNullOrWhiteSpace(sn) &&
                    !string.IsNullOrWhiteSpace(supn) && !string.IsNullOrWhiteSpace(mname_Copy.Text))
                {
                    /*string query = "INSERT INTO medicinfo (mname, bprice, sprice, exdate, madate, nummedic, sname, from_, too_) " +
                                   $"VALUES ('{medicname}', '{buy}', '{sell}', '{datex}', '{datem}', '{mnum}', '{sn}', '{bb}', '{sb}')";
                    conn.setData(query);*/
                    reset_();
                }
            }

            else
            {
                // Handle validation errors here
                if (string.IsNullOrWhiteSpace(mname.Text) || mname.Text == "insert the Medic Name!")
                {


                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(mname, true);
                    Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                    shakeStoryboard.Begin(label1);
                }
                if (string.IsNullOrWhiteSpace(type.Text) || type.Text == "insert the Type Name!")
                {


                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(type, true);
                    Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                    shakeStoryboard.Begin(label13);
                }
                if (string.IsNullOrWhiteSpace(scififcename.Text) || scififcename.Text == "insert the Scinfic Name!")
                {


                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(scififcename, true);
                    Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];

                }
                if (string.IsNullOrWhiteSpace(bprice.Text) || bprice.Text == "999,999")
                {
                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(bprice, true);
                    Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                    shakeStoryboard.Begin(label4);

                }
                if (string.IsNullOrWhiteSpace(sprice.Text) || sprice.Text == "999,999")
                {
                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(sprice, true);
                    Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                    shakeStoryboard.Begin(label5);

                }
                if (string.IsNullOrWhiteSpace(num.Text) || num.Text == "9999")
                {
                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(num, true);
                    Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                    shakeStoryboard.Begin(label3);

                }
                if (string.IsNullOrWhiteSpace(mname_Copy.Text) || mname_Copy.Text == "insert the Too Name!")
                {
                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(mname_Copy, true);
                    Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                    shakeStoryboard.Begin(label2);

                }
                if (sname.SelectedIndex <= 0)
                {
                    sname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
                if (supname.SelectedIndex <= 0)
                {
                    supname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
                if (YearComboBox.SelectedIndex <= 0)
                {
                    ex.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
                if (YearComboBox1.SelectedIndex <= 0)
                {
                    ma.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
                if (MonthComboBox.SelectedIndex <= 0)
                {
                    ex.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
                if (MonthComboBox1.SelectedIndex <= 0)
                {
                    ma.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
                if (DayComboBox.SelectedIndex <= 0)
                {
                    ex.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
                if (DayComboBox1.SelectedIndex <= 0)
                {
                    ma.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC74747"));
                }
            }
        }

        public void reset_()

        {
            brcode = "";
            check.Content = "✗";
            check.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC72929"));
            InputTextBox.Clear();
            mname.Text = "";
            bprice.Clear();
            sprice.Clear();
            num.Text = "";
            //sname.Clear();

        }
        public List<string> GetFromNames()
        {
            List<string> storageNames = new List<string>();
            string query = "select DISTINCT  supname from Suppliers where dbid = '"+Properties.Settings.Default.dbid+"'";

            // Assuming you have a method in your connection class to get data
            DataSet ds = conn.getData(query);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    storageNames.Add(row["supname"].ToString());
                }
            }

            return storageNames;
        }
        public void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            query = "select sname from storageinfo where dbid = '"+Properties.Settings.Default.dbid+"'";
            ds = conn.getData(query);
            List<string> storagenames = new List<string>();
            try
            {


                if (ds != null && ds.Tables.Count > 0)
                {
                    sname.ItemsSource = ds.Tables[0].DefaultView;
                    sname.DisplayMemberPath = "sname";
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

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDays();
        }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDays();
        }

        private void YearComboBox_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            PopulateDays1();
        }

        private void MonthComboBox_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            PopulateDays1();
        }

        private void mname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mname.Text == "")
            {

                MoveLabelUp(label1);
                mname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2EA5A3"));
            }

        }

        private void mname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (mname.Text == "" || mname.Text == "insert the Medic Name!")
            {
                MoveLabelDown(label1);
                mname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C"));

            }
        }

        private void mname_MouseEnter(object sender, MouseEventArgs e)
        {
            mname.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void mname_MouseLeave(object sender, MouseEventArgs e)
        {
            mname.Background = null;
        }

        private void num_MouseEnter(object sender, MouseEventArgs e)
        {
            num.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void num_MouseLeave(object sender, MouseEventArgs e)
        {
            num.Background = null;
        }

        private void num_LostFocus(object sender, RoutedEventArgs e)
        {
            if (num.Text == "")
            {
                MoveLabelDown(label3);
                num.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C"));

            }
        }

        private void num_GotFocus(object sender, RoutedEventArgs e)
        {
            if (num.Text == "")
            {
                MoveLabelUp(label3);
                num.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2EA5A3"));

            }

        }

        private void sprice_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sprice.Text == "")
            {
                MoveLabelUp(label5);
                sprice.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2EA5A3"));

            }
        }

        private void sprice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sprice.Text == "")
            {
                MoveLabelDown(label5);
                sprice.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C"));

            }
        }

        private void sprice_MouseEnter(object sender, MouseEventArgs e)
        {
            sprice.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void sprice_MouseLeave(object sender, MouseEventArgs e)
        {
            sprice.Background = null;
        }

        private void bprice_MouseEnter(object sender, MouseEventArgs e)
        {
            bprice.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void bprice_MouseLeave(object sender, MouseEventArgs e)
        {
            bprice.Background = null;
        }

        private void bprice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (bprice.Text == "")
            {
                MoveLabelDown(label4);
                bprice.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C"));

            }
        }

        private void bprice_GotFocus(object sender, RoutedEventArgs e)
        {
            if (bprice.Text == "")
            {
                MoveLabelUp(label4);
                bprice.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2EA5A3"));

            }

        }

        private void mname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mname.IsFocused)
            {
                if (mname.Text == "" || mname.Text == "Medic Name..." || mname.Text == "insert the Medic Name!")
                {
                    mname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    mname.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void num_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if the TextBox is focused and modify the foreground color based on the text
            if (num.IsFocused)
            {
                if (string.IsNullOrEmpty(num.Text) || num.Text == "9999")
                {
                    num.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    num.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }



        private void sprice_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Temporarily remove the event handler to prevent recursion
            sprice.TextChanged -= sprice_TextChanged;

            // Store the current caret position
            int caretIndex = sprice.CaretIndex;

            // Remove all non-digit characters except commas
            string rawText = sprice.Text.Replace(",", "");

            if (long.TryParse(rawText, out long number))
            {
                string formattedText;


                // Format the number with commas for IQD (e.g., 1,000,000)
                formattedText = number.ToString("N0", CultureInfo.InvariantCulture);




                sprice.Text = formattedText;

                // Adjust the caret position after formatting
                int newCaretIndex = caretIndex + (formattedText.Length - rawText.Length);
                sprice.CaretIndex = Math.Min(newCaretIndex, sprice.Text.Length);
            }
            else
            {
                // If input is invalid, clear the text
                sprice.Text = string.Empty;
            }

            // Reattach the TextChanged event handler
            sprice.TextChanged += sprice_TextChanged;
        }


        private void sprice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input
            e.Handled = !Regex.IsMatch(e.Text, @"\d");
        }



        private void bprice_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Temporarily remove the event handler to prevent recursion
            bprice.TextChanged -= bprice_TextChanged;

            // Store the current caret position
            int caretIndex = bprice.CaretIndex;

            // Remove all non-digit characters except commas
            string rawText = bprice.Text.Replace(",", "");

            if (long.TryParse(rawText, out long number))
            {
                string formattedText = number.ToString("N0", CultureInfo.InvariantCulture);

                bprice.Text = formattedText;

                // Adjust the caret position
                int newCaretIndex = caretIndex + (formattedText.Length - rawText.Length);
                bprice.CaretIndex = Math.Min(newCaretIndex, bprice.Text.Length);
            }
            else
            {
                // If the input is invalid, clear the text
                bprice.Text = string.Empty;
            }

            // Reattach the TextChanged event handler
            bprice.TextChanged += bprice_TextChanged;
        }



        private void sname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sname.SelectedItem != null)
            {
                sn = sname.SelectedItem.ToString();
                Console.WriteLine(sn);
            }
            else
            {
                sn = null; // Or handle null case appropriately
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void mybutton_Click1(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            reset_();
        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            var selectedMedic = DataGrid1.SelectedItem as MedicInfo;
            if (selectedMedic != null)
            {
                // Ensure DataGrid1 is bound to an ObservableCollection
                var medicList = DataGrid1.ItemsSource as ObservableCollection<MedicInfo>;
                if (medicList != null)
                {
                    // Remove the selected item from the collection
                    medicList.Remove(selectedMedic);

                    // Recalculate the total price after removing the item
                    decimal totalPrice = 0m;
                    foreach (var medic in medicList)
                    {
                        // Multiply buy price by quantity (NumberMedic)
                        if (decimal.TryParse(medic.BuyPrice, out decimal buyPrice) && int.TryParse(medic.NumberMedic, out int quantity))
                        {
                            totalPrice += buyPrice * quantity;
                        }
                        UpdateTotalSellPrice();
                    }

                    // Update the pricet label with the new total
                    pricet.Content = $"{totalPrice:C}"; // Formats as currency
                    price.Content = $"{totalPrice:C}";
                    // Check if the DataGrid is now empty
                    if (medicList.Count == 0)
                    {
                        /*pharmacist parentWindow = (pharmacist)Window.GetWindow(this);
                        parentWindow?.enablecheck();*/
                    }

                }
            }
            else
            {
                MessageBox.Show("no Items to delete");
            }
        }



        private void Button_Click1(object sender, RoutedEventArgs e)
        {

        }

        private void bidcreate()
        {
        query = "SELECT MAX(billId) AS MaxBillId FROM bills where dbid = '"+Properties.Settings.Default.dbid+"' and type = 'buy'";
                ds = conn.getData(query);

                int maxid = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && int.TryParse(ds.Tables[0].Rows[0]["MaxBillId"].ToString(), out int result))
                {
                    maxid = result;
                }
            bid = maxid + 1;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            try
            {
                // Get the maximum bill ID from medicinfo
                

                var medicList = DataGrid1.ItemsSource as ObservableCollection<MedicInfo>;

                if (medicList == null || medicList.Count == 0)
                {
                    MessageBox.Show("No items to insert.");
                    return;
                }

                

                foreach (var medic in medicList.ToList())
                {
                    string medicName = medic.MedicName ?? string.Empty;
                    string buy = medic.BuyPrice ?? "0";
                    string sell = medic.SellPrice ?? "0";
                    string expiryDate = medic.ExpiryDate ?? string.Empty;
                    string manufactureDate = medic.ManufactureDate ?? string.Empty;
                    string numberMedic = medic.NumberMedic ?? "0";
                    string sname = medic.Sname ?? string.Empty;
                    string cid = medic.Codeid ?? string.Empty; // Assuming Codeid is the barcode

                    // Parse buy and sell prices
                    decimal buyPrice = 0;
                    decimal sellPrice = 0;

                    if (!decimal.TryParse(medic.BuyPrice, out buyPrice))
                    {
                        buyPrice = 0; // Default if parsing fails
                    }

                    if (!decimal.TryParse(medic.SellPrice, out sellPrice))
                    {
                        sellPrice = 0; // Default if parsing fails
                    }

                    // Parse dates
                    DateTime expiryDateValue, manufactureDateValue;
                    if (!DateTime.TryParse(medic.ExpiryDate, out expiryDateValue))
                    {
                        expiryDate = string.Empty; // Handle invalid date parsing
                    }
                    else
                    {
                        expiryDate = expiryDateValue.ToString("yyyy-MM-dd"); // SQL compatible date format
                    }

                    if (!DateTime.TryParse(medic.ManufactureDate, out manufactureDateValue))
                    {
                        manufactureDate = string.Empty; // Handle invalid date parsing
                    }
                    else
                    {
                        manufactureDate = manufactureDateValue.ToString("yyyy-MM-dd"); // SQL compatible date format
                    }

                    // Parse quantity
                    int quantity = 0;
                    if (!int.TryParse(medic.NumberMedic, out quantity))
                    {
                        numberMedic = "0"; // Default if parsing fails
                    }

                    // Sanitize inputs
                    medicName = SqlEscape(medicName);
                    expiryDate = SqlEscape(expiryDate);
                    manufactureDate = SqlEscape(manufactureDate);
                    numberMedic = SqlEscape(numberMedic);
                    sname = SqlEscape(sname);
                    cid = SqlEscape(cid); 
                    string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                    // Check if the record already exists based on the barcode (codeid)
                    query = $"SELECT * FROM medicinfo WHERE dbid = '"+Properties.Settings.Default.dbid+"' and codeid = '{cid}'";
                    ds = conn.getData(query);
                    if (cid == "")
                    {
                        cid = "0";
                    }
                    // If `codeid = 0`, allow multiple items with `codeid = 0`
                    if (cid == "0")
                    {
                        // Insert new record for items without a barcode
                        query = "INSERT INTO medicinfo (mname, bprice, sprice, exdate, madate, nummedic, sname, from_, too_, billId, codeid , ScintficName , medictype ,dbid) " +
                                $"VALUES ('{medicName}', '{buyPrice:F2}', '{sellPrice:F2}', '{expiryDate}', '{manufactureDate}', '{numberMedic}', '{sname}', '{supn}', '{mname_Copy.Text}', '{bid}', '{cid}' , '{scififcename.Text}' , '{type.Text}' ,'"+Properties.Settings.Default.dbid+"')";
                        conn.setData(query);
                        query = $"UPDATE storageinfo SET size = size - {quantity} WHERE dbid = '"+Properties.Settings.Default.dbid+"' and sname = '{sn}'";
                        conn.setData(query);
                        query = "SELECT TOP 1 mid\r\nFROM medicinfo where dbid = '"+Properties.Settings.Default.dbid+"' \r\nORDER BY mid DESC;\r\n";
                        ds = conn.getData(query);
                        int mid = int.Parse( ds.Tables[0].Rows[0][0].ToString());
                        query = "insert into medichistory values( '" + numberMedic+"' , '"+bid+"' , '"+currentDate+"' , '"+mid+"' ,'"+Properties.Settings.Default.dbid+"')";
                        conn.setData(query);

                        // Update storage size based on the quantity of the medic added
                    }
                    else if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 0)
                    {
                        // Ensure that codeid is unique for real barcodes (codeid != 0)
                        query = "INSERT INTO medicinfo (mname, bprice, sprice, exdate, madate, nummedic, sname, from_, too_, billId, codeid , ScintficName , medictype ,dbid) " +
                                $"VALUES ('{medicName}', '{buyPrice:F2}', '{sellPrice:F2}', '{expiryDate}', '{manufactureDate}', '{numberMedic}', '{sname}', '{supn}', '{mname_Copy.Text}', '{bid}', '{cid}' , '{scififcename.Text}' , '{type.Text}' ,'"+Properties.Settings.Default.dbid+"')";
                        conn.setData(query);

                        query = "SELECT TOP 1 mid\r\nFROM medicinfo where dbid = '"+Properties.Settings.Default.dbid+"'\r\nORDER BY mid DESC;\r\n";
                        ds = conn.getData(query);
                        int mid = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                        query = "insert into medichistory values( '" + numberMedic + "' , '" + bid + "' , '" + currentDate + "' , '" + mid + "' ,'"+Properties.Settings.Default.dbid+"')";
                        conn.setData(query);
                        // Update storage size based on the quantity of the medic added
                    }

                }

                // Clear the collection after insertion
                medicList.Clear();
                pricet.Content = "0,00";
                reset_();
                MessageBox.Show("Data processed successfully.");
                accountentimage4.IsEnabled = false;
                add_Copy.IsEnabled = false;
                add_Copy11.IsEnabled = false;
                add_Copy3.IsEnabled = true;
                // Insert into the bills table
                string from_ = supn;
                string too_ = mname_Copy.Text;
                decimal price = totalBuyPrice;
                string currentDate1 = DateTime.Now.ToString("yyyy-MM-dd");
                string by_ = Login_.username;

                query = "INSERT INTO bills (from_, too_, Price, bdate, billId, type, by_, currency , dbid) " +
                        $"VALUES ('{from_}', '{too_}', {price:F2}, '{currentDate1}', '{bid}', 'buy', '{by_}', '{setting.currencyies}' ,'"+Properties.Settings.Default.dbid+"')";
                conn.setData(query);

                totalBuyPrice = 0;
                supname.IsEnabled = true;
            }
            catch (FormatException ex)
            {
                // Handle format exceptions here
            }
            catch (Exception ex)
            {
                // Handle other exceptions here
            }
        }





        private string SqlEscape(string input)
        {
            if (input == null) return null;
            return input.Replace("'", "''");
        }
        




        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void sname_Loaded(object sender, RoutedEventArgs e)
        {
            fullnameLabel.Content = full;
            _collectionView = CollectionViewSource.GetDefaultView(GetStorageNames());
            sname.ItemsSource = _collectionView;
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

        private void bprice_GotFocus1(object sender, RoutedEventArgs e)
        {
            /*if (bbill.Text == "" || bbill.Text == "From..." )
            {
                bbill.Text = "";
                bbill.Foreground = new SolidColorBrush(Colors.Black);

            }
            bbill.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF808080"));*/
        }

        private void bprice_LostFocus1(object sender, RoutedEventArgs e)
        {
            /*if (string.IsNullOrWhiteSpace(bbill.Text))
            {
                bbill.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                bbill.Text = "From...";
            }*/
        }

        private void bprice_MouseEnter1(object sender, MouseEventArgs e)
        {
/*            bbill.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
*/
        }

        private void bprice_MouseLeave1(object sender, MouseEventArgs e)
        {
/*            bbill.Background = null;
*/
        }

        private void bprice_GotFocus2(object sender, RoutedEventArgs e)
        {
            /*if (sbill.Text == "" || sbill.Text == "Too...")
            {
                sbill.Text = "";
                sbill.Foreground = new SolidColorBrush(Colors.Black);

            }
            sbill.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF808080"));*/
        }

        private void bprice_LostFocus2(object sender, RoutedEventArgs e)
        {
            /*if (string.IsNullOrWhiteSpace(sbill.Text))
            {
                sbill.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                sbill.Text = "Too...";
            }*/
        }

        private void bprice_MouseEnter2(object sender, MouseEventArgs e)
        {
           // sbill.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void bprice_MouseLeave2(object sender, MouseEventArgs e)
        {
            //sbill.Background = null;
        }

        private async void mybutton_Click2(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                // Create a FlowDocument
                FlowDocument fd = new FlowDocument();
                fd.PagePadding = new Thickness(50);
                fd.ColumnWidth = printDialog.PrintableAreaWidth;

                // Add a header with the current date and time
                fd.Blocks.Add(new System.Windows.Documents.Paragraph(new Run("Your Company Name"))
                {
                    FontSize = 24,
                    FontWeight = System.Windows.FontWeights.Bold,
                    TextAlignment = System.Windows.TextAlignment.Center
                });

                fd.Blocks.Add(new System.Windows.Documents.Paragraph(new Run($"Date and Time: {DateTime.Now.ToString("f")}\n"))
                {
                    FontSize = 14,
                    FontWeight = System.Windows.FontWeights.Bold,
                    TextAlignment = System.Windows.TextAlignment.Center
                });

                // Add the total price
                fd.Blocks.Add(new System.Windows.Documents.Paragraph(new Run($"Total Price: {pricet.Content}\n"))
                {
                    FontSize = 16,
                    FontWeight = System.Windows.FontWeights.Bold,
                    TextAlignment = System.Windows.TextAlignment.Right
                });

                // Create a WPF Table (System.Windows.Documents.Table)
                System.Windows.Documents.Table table = new System.Windows.Documents.Table();
                table.CellSpacing = 0;
                table.BorderThickness = new Thickness(0.5);
                table.BorderBrush = Brushes.Black;

                // Add columns to the Table
                for (int i = 0; i < DataGrid1.Columns.Count; i++)
                {
                    table.Columns.Add(new TableColumn() { Width = new GridLength(DataGrid1.Columns[i].ActualWidth) });
                }

                // Add headers to the Table
                TableRowGroup headerGroup = new TableRowGroup();
                TableRow headerRow = new TableRow();
                foreach (var column in DataGrid1.Columns)
                {
                    headerRow.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run(column.Header.ToString())))
                    {
                        FontWeight = System.Windows.FontWeights.Bold,
                        TextAlignment = System.Windows.TextAlignment.Center,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.5)
                    });
                }
                headerGroup.Rows.Add(headerRow);
                table.RowGroups.Add(headerGroup);

                // Add rows to the Table
                TableRowGroup bodyGroup = new TableRowGroup();
                foreach (var item in DataGrid1.Items)
                {
                    DataGridRow row = (DataGridRow)DataGrid1.ItemContainerGenerator.ContainerFromItem(item);
                    if (row != null)
                    {
                        TableRow bodyRow = new TableRow();
                        foreach (var column in DataGrid1.Columns)
                        {
                            var cellValue = column.GetCellContent(item) as TextBlock;
                            if (cellValue != null)
                            {
                                bodyRow.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run(cellValue.Text)))
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0.5)
                                });
                            }
                        }
                        bodyGroup.Rows.Add(bodyRow);
                    }
                }
                table.RowGroups.Add(bodyGroup);

                // Add the table to the FlowDocument
                fd.Blocks.Add(table);

                // Print the FlowDocument
                printDialog.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, "Print DataGrid");
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {

               
                 
                

                
            }
        }
        

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            supname.IsEnabled = true;
            query = "SELECT MAX(billId) AS MaxBillId FROM medicinfo where dbid = '"+Properties.Settings.Default.dbid+"'";
            ds = conn.getData(query);
            maxid = int.Parse(ds.Tables[0].Rows[0][0].ToString());
            var medicList = DataGrid1.ItemsSource as ObservableCollection<MedicInfo>;

            if (medicList == null || medicList.Count == 0)
            {
                MessageBox.Show("No items to insert.");
                return;
            }
            bid = maxid + 1;
            foreach (var medic in medicList.ToList())
            {
                Console.WriteLine(medic.Codeid);
                // Handle null values and provide default values
                string medicName = medic.MedicName ?? string.Empty;
                string buyPrice = medic.BuyPrice ?? string.Empty;
                string sellPrice = medic.SellPrice ?? string.Empty;
                string expiryDate = medic.ExpiryDate ?? string.Empty;
                string manufactureDate = medic.ManufactureDate ?? string.Empty;
                string numberMedic = medic.NumberMedic ?? string.Empty;
                string sname = medic.Sname ?? string.Empty;
                string sb = medic.SillBill ?? string.Empty;
                string bb = medic.BuyBill ?? string.Empty;
                string cid = medic.Codeid ?? string.Empty;

                // Sanitize inputs
                medicName = SqlEscape(medicName);
                buyPrice = SqlEscape(buyPrice);
                sellPrice = SqlEscape(sellPrice);
                expiryDate = SqlEscape(expiryDate);
                manufactureDate = SqlEscape(manufactureDate);
                numberMedic = SqlEscape(numberMedic);
                sname = SqlEscape(sname);
                sb = SqlEscape(sb);
                bb = SqlEscape(bb);

                query = "select * from medicinfo where dbid = '"+Properties.Settings.Default.dbid+"' and mname like '" + medicName + "' AND bprice like '" + buyPrice + "' AND sprice like '" + sellPrice + "' AND exdate like '" + expiryDate + "' AND madate like '" + manufactureDate + "' AND nummedic like '" + numberMedic + "' AND sname like '" + sname + "' AND from_ like '" + supn + "' AND too_ like '" + mname_Copy.Text + "' AND  billId like '" + bid + "' AND codeid like '" + cid + "'";
                ds = conn.getData(query);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    query = "update medicinfo set nummedic = nummedic + '" + numberMedic + "'  where dbid = '"+Properties.Settings.Default.dbid+"' and mname like '" + medicName + "' AND bprice like '" + buyPrice + "' AND sprice like '" + sellPrice + "' AND exdate like '" + expiryDate + "' AND madate like '" + manufactureDate + "' AND nummedic like '" + numberMedic + "' AND sname like '" + sname + "' AND from_ like '" + supn + "' AND too_ like '" + mname_Copy.Text + "' AND billId like '" + bid + "' AND codeid like '" + cid + "'";
                    conn.setData(query);

                }
                else
                {
                    query = "INSERT INTO medicinfo (mname, bprice, sprice, exdate, madate, nummedic, sname,from_,too_,billId,codeid , dbid) " +
                                  $"VALUES ('{medicName}', '{buyPrice}', '{sellPrice}', '{expiryDate}', '{manufactureDate}', '{numberMedic}', '{sname}','{supn}','{mname_Copy.Text}','{bid}','{cid}','"+Properties.Settings.Default.dbid+"')";

                    // Execute the query
                    conn.setData(query);
                }




            }



            // Clear the collection after insertion
            medicList.Clear();
            pricet.Content = "0,00";
            reset_();
            MessageBox.Show("Data inserted and DataGrid cleared successfully.");

            string from_ = supn;
            string too_ = mname_Copy.Text;
            decimal price = totalBuyPrice;
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            query = "insert into bills (from_,too_,Price,bdate,billId,type ,dbid)" +
                               $"VALUES ('{from_}', '{too_}', '{price}','{currentDate}', '{bid}','{"buy"}' ,'"+Properties.Settings.Default.dbid+"')";
            conn.setData(query);
        }

        /*private void sname_KeyUp1(object sender, KeyEventArgs e)
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
        }*/

      /*  private void sname_Loaded1(object sender, RoutedEventArgs e)
        {
            _collectionView = CollectionViewSource.GetDefaultView(GetTooNames());
            sname_Copy.ItemsSource = _collectionView;
        }*/
        public List<string> GetTooNames()
        {
            List<string> storageNames = new List<string>();
            string query = "select DISTINCT too_ from bills where dbid = '"+Properties.Settings.Default.dbid+"'";

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

        /*private void sname_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (supname.SelectedItem != null)
            {
                recn = supname.SelectedItem.ToString();
            }
            else if (!string.IsNullOrEmpty(sname_Copy.Text))
            {
                recn = sname_Copy.Text;
            }
            else
            {
                recn = null; // Or handle null case appropriately
            }
            Console.WriteLine(recn);
        }*/

        private void sname_KeyUp2(object sender, KeyEventArgs e)
        {
            if (_collectionView == null)
                return;

            _collectionView.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(supname.Text))
                    return true;

                return ((string)item).IndexOf(supname.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            _collectionView.Refresh();
            supname.IsDropDownOpen = true;

            // Update supn with the current text in the ComboBox, whether it's selected or typed by the user.
            supn = supname.Text;
            
        }


        private void sname_Loaded2(object sender, RoutedEventArgs e)
        {
            _collectionView = CollectionViewSource.GetDefaultView(GetFromNames());
            supname.ItemsSource = _collectionView;
        }

        private void sname_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            if (supname.SelectedItem != null)
            {
                supn = supname.SelectedItem.ToString();
            }
            else if (!string.IsNullOrEmpty(supname.Text))
            {
                supn = supname.Text;
            }
            else
            {
                supn = null; // Handle the null case appropriately
            }
            Console.WriteLine(supn);
        }

        private void mybutton_Click12(object sender, RoutedEventArgs e)
        {
            fe.Visibility = Visibility.Visible;
           
            // Begin the fade-in animation
            Storyboard fadeInStoryboard = (Storyboard)this.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
        }

        

        private void mname_GotFocus4(object sender, RoutedEventArgs e)
        {
            if (mname_Copy.Text == "")
            {
                
                mname_Copy.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2EA5A3"));

            }

        }

        private void mname_LostFocus4(object sender, RoutedEventArgs e)
        {
            if (mname_Copy.Text == "" )
            {
             
                mname_Copy.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C"));

            }
        }

        private void mname_MouseEnter4(object sender, MouseEventArgs e)
        {
            mname_Copy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDEDED"));
        }

        private void mname_MouseLeave4(object sender, MouseEventArgs e)
        {
            mname_Copy.Background = null;
        }

        private void mname_TextChanged4(object sender, TextChangedEventArgs e)
        {
            if (mname_Copy.Text == "" || mname_Copy.Text == "Too Name...")
            {
                mname_Copy.Text = "";
                mname_Copy.Foreground = new SolidColorBrush(Colors.Black);

            }
            mname_Copy.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF808080"));
        }
        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
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

        private void Button_Clicka(object sender, RoutedEventArgs e)
        {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void bprice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input (no need to allow commas manually)
            e.Handled = !Regex.IsMatch(e.Text, @"\d");
        }


        private async void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(300);
            brcode = InputTextBox.Text;

            if (brcode == "")
            {

               

                InputTextBox.Clear();
                brcode = "";
                check.Content = "✗";
                check.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC72929"));


            }
            else
            {
                check.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF39A58F"));
                check.Content = "✓";
                brcode = InputTextBox.Text;
            }





        }







        private void StartGifAnimation()
        {
            // Run the GIF animation setup on a separate thread

            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1335-qr-code-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage, image1);
            ImageBehavior.SetRepeatBehavior(accountentimage, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-6-shopping-hover-shopping.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage1, image2);
            ImageBehavior.SetRepeatBehavior(accountentimage1, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-49-plus-circle-hover-swirl (1).gif"));
            ImageBehavior.SetAnimatedSource(accountentimage2, image3);
            ImageBehavior.SetRepeatBehavior(accountentimage2, System.Windows.Media.Animation.RepeatBehavior.Forever); 
            var image4 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (1).gif"));
            ImageBehavior.SetAnimatedSource(accountentimage3, image4);
            ImageBehavior.SetRepeatBehavior(accountentimage3, System.Windows.Media.Animation.RepeatBehavior.Forever); 
            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-948-stock-share-hover-pinch (2).gif"));
            ImageBehavior.SetAnimatedSource(accountentimage4, image5);
            ImageBehavior.SetRepeatBehavior(accountentimage4, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image6 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1339-sale-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage5, image6);
            ImageBehavior.SetRepeatBehavior(accountentimage5, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-39-trash-hover-trash-empty.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage6, image7);
            ImageBehavior.SetRepeatBehavior(accountentimage6, System.Windows.Media.Animation.RepeatBehavior.Forever); 
            var image8 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-44-folder-hover-folder.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage7, image8);
            ImageBehavior.SetRepeatBehavior(accountentimage7, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image9 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-29-cross-hover-cross-3 (2).gif"));
            ImageBehavior.SetAnimatedSource(accountentimage9, image9);
            ImageBehavior.SetRepeatBehavior(accountentimage9, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image22 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (2).gif"));
            ImageBehavior.SetAnimatedSource(back, image22);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);
           


        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            pdf_Copy.Focus();
     
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
            StartGifAnimation();
        }

        public void MoveLabelUp(Label label)
        {
            // Create a storyboard
            Storyboard storyboard = new Storyboard();

            // Create a DoubleAnimation to move the label up by 34 units
            DoubleAnimation moveUpAnimation = new DoubleAnimation
            {
                From = 0, // Start from current position
                To = -30, // Move up by 34 units (negative value moves upward)
                Duration = new Duration(TimeSpan.FromSeconds(0.1)) // Duration of the animation
            };

            // Set the target property (Y translation) for the animation
            Storyboard.SetTarget(moveUpAnimation, label);
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            // Add the animation to the storyboard
            storyboard.Children.Add(moveUpAnimation);

            // Start the animation
            storyboard.Begin();
        }

        public void MoveLabelDown(Label label)
        {
            // Create the TranslateTransform if not already defined
            TranslateTransform trans = new TranslateTransform();
            label.RenderTransform = trans;

            // Create the storyboard for moving the label down
            Storyboard storyboard = new Storyboard();

            // Create a DoubleAnimation to move the label down by 34 units
            DoubleAnimation animation = new DoubleAnimation
            {
                From = -30,
                To = 0, // Moving down by 34 units
                Duration = new Duration(TimeSpan.FromSeconds(0.1)) // Duration of the animation
            };

            // Set the target property for the animation (Y-axis translation)
            Storyboard.SetTarget(animation, label);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard to start the animation
            storyboard.Begin();
        }

        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mname.Focus() == false)
            {
                mname_GotFocus(sender, e);
                mname.Focus();
            }
            
        }

        private void label2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mname_Copy.Focus() == false)
            {
                mname_GotFocus4(sender, e);
                mname_Copy.Focus();
            }
        }

        private void label3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (num.Focus() == false)
            {
                num_GotFocus(sender, e);
                num.Focus();
            }
        }

        private void label4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bprice.Focus() == false)
            {
                bprice_GotFocus(sender, e);
                bprice.Focus();
            }
        }

        private void label5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sprice.Focus() == false)
            {
                sprice_GotFocus(sender, e);
                sprice.Focus();
            }
        }
        public void ApplyStoryboard(TextBox textBox)
        {
            var storyboard = (Storyboard)this.Resources["ShakeAndRedBorderStoryboard"];

            // Apply RenderTransform if not already applied
            if (textBox.RenderTransform == null || !(textBox.RenderTransform is TransformGroup))
            {
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform());
                textBox.RenderTransform = transformGroup;
            }

            // Begin the storyboard for the specific TextBox
            Storyboard.SetTarget(storyboard, textBox);
            storyboard.Begin();
        }
        private void UpdateTotalSellPrice()
        {
            // Ensure DataGrid1.ItemsSource is cast to ObservableCollection<MedicInfo>
            var medicList = DataGrid1.ItemsSource as ObservableCollection<MedicInfo>;

            if (medicList == null || medicList.Count == 0)
            {
                price.Content = "0";
                pricet.Content = "0";
                return;
            }

            // Calculate total sell price
            decimal totalSellPrice = 0;
            foreach (var item in medicList)
            {
                if (decimal.TryParse(item.SellPrice, out decimal sellPrice) && int.TryParse(item.NumberMedic, out int quantity))
                {
                    totalSellPrice += sellPrice * quantity; // Multiply by quantity
                }
            }

            // Update price and pricet labels
            price.Content = totalSellPrice.ToString("C"); // Format as currency
            pricet.Content = totalSellPrice.ToString("C");
        }

        private void accountentimage3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mybutton_Click1(sender, e);
        }

        private void accountentimage2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mybutton_Click12(sender, e);
        }

        private void accountentimage4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button_Click_1(sender, e);
        }

        private void accountentimage7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mybutton_Click2(sender, e);
        }

        private void accountentimage6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button_Click(sender, e);
        }

        
        private void supreset() 
        {
            supplieradd.suplocation.Clear();
            supplieradd.supname.Clear();
            supplieradd.supnum.Clear();
        }
       
        private async void close_Click(object sender, MouseButtonEventArgs e)
        {
            sname_Loaded2(sender, e);   
            Storyboard fadeInStoryboard1 = (Storyboard)this.FindResource("FadeOutStoryboard1");
            fadeInStoryboard1.Begin();
            await Task.Delay(400);
            supreset();
            fe.Visibility = Visibility.Collapsed;
            
        }

       
        private void HandleBarcodeInput(string input)
        {
            // Check if this is the first character or a quick burst of characters (barcode scanner)
            if (!inputTimer.IsRunning || inputTimer.ElapsedMilliseconds < barcodeThresholdMs)
            {
                barcodeInput += input;
                inputTimer.Restart();
            }
            else
            {
                // If input is too slow, treat it as normal input and reset barcode input
                barcodeInput = string.Empty;
                inputTimer.Stop();
            }
        }

        private void UserControl_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            // Barcode scanners often send the Enter key at the end of the scan
            if (e.Key == Key.Enter)
            {
                InputTextBox.Text = barcodeInput.Trim();
                InputTextBox.Focus();
                barcodeInput = string.Empty;
                inputTimer.Stop();
                e.Handled = true; // Prevent the Enter key from being processed by other elements
            }
        }

        private void UserControl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HandleBarcodeInput(e.Text);
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

        private async void Button_Clicks(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);

            setting.mainsetting.Visibility = Visibility.Visible;

            setting.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard2"];
            fadeOutStoryboard.Begin();
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
          
            Grid_Loaded(sender, e);
            
       

        }



        private void mname_TextChanged1(object sender, TextChangedEventArgs e)
        {
            if (type.IsFocused)
            {
                if (type.Text == "" || type.Text == "Type Name..." || type.Text == "insert the Type Name!")
                {
                    type.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    type.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void mname_TextChanged2(object sender, TextChangedEventArgs e)
        {
            if (scififcename.IsFocused)
            {
                if (scififcename.Text == "" || scififcename.Text == "Scinfic Name..." || scififcename.Text == "insert the Scitnifc Name!")
                {
                    scififcename.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC1BCBC"));
                }
                else
                {
                    scififcename.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void mname_LostFocus2(object sender, RoutedEventArgs e)
        {
            if (scififcename.Text == "" || scififcename.Text == "insert the Scitnifc Name!")
            {
                MoveLabelDown(label13);
                scififcename.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C"));

            }
        }

        private void mname_GotFocus2(object sender, RoutedEventArgs e)
        {
            if (scififcename.Text == "")
            {

                MoveLabelUp(label13);
                scififcename.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2EA5A3"));
            }
        }

        private void label1_1MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (type.Focus() == false)
            {
                mname_GotFocus2(sender, e);
                type.Focus();
            }

        }

        private void label1_4MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (scififcename.Focus() == false)
            {
               
                scififcename.Focus();
            }

        }

        private void num_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void mybutton_Click11(object sender, RoutedEventArgs e)
        {
            add_Copy3.IsEnabled = false;
            add_Copy.IsEnabled = true;
            add_Copy11.IsEnabled = true;
            accountentimage4.IsEnabled = true;
            bidcreate();
        }
    }
}
