








using iText.Forms.Form.Element;
using Org.BouncyCastle.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
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
using System.Xml.Linq;
using WpfAnimatedGif;
using static OxSystem.sellmedic;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for sellmedic.xaml
    /// </summary>
    /// 
    public partial class sellmedic : UserControl

    {
        private string barcodeInput = string.Empty; // To accumulate the barcode input
        private Stopwatch inputTimer = new Stopwatch(); // To measure the time between key inputs
        private const int barcodeThresholdMs = 50; // Threshold time in milliseconds

        private SolidColorBrush numMedicBackground;
        private Dictionary<string, TextBlock> cardQuantities = new Dictionary<string, TextBlock>();

        private Dictionary<string, Border> cardBorders = new Dictionary<string, Border>();


        public class MedicItem
        {
            public string Mname { get; set; }
            public string Bprice { get; set; }
            public string Sprice { get; set; }
            public int Quantity { get; set; }
        }

        private decimal totalBuyPrice = 0;
        public int maxid;
        public int bid;
        private string _currentTime;
        private DispatcherTimer _timer;
        public static string brcode;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        public static string sn;
        public static string mn;
        public static Int64 tq;
        public static Int64 nq;
        public static string exdate;
        public static string madate;
        public static string full = Login_.fullName;
        public ObservableCollection<MedicWithFlag> TransferredMedicList { get; set; }

        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        public sellmedic()
        {
            InitializeComponent();



            TransferredMedicList = new ObservableCollection<MedicWithFlag>();
            DataGrid1.ItemsSource = TransferredMedicList;
            DataContext = this;
            UpdateCurrentTime();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }
        private void UpdateCurrentTime()
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
        }
        public class MedicWithFlag : INotifyPropertyChanged
        {
            private string _mname;
            private decimal _bprice;
            private decimal _sprice;
            private int _flag;

            public MedicWithFlag()
            {
                Flag = 1; // Initialize Flag to 1
            }

            public string mname
            {
                get => _mname;
                set
                {
                    _mname = value;
                    OnPropertyChanged(nameof(mname));
                }
            }

            public decimal bprice
            {
                get => _bprice;
                set
                {
                    _bprice = value;
                    OnPropertyChanged(nameof(bprice));
                }
            }

            public decimal sprice
            {
                get => _sprice;
                set
                {
                    _sprice = value;
                    OnPropertyChanged(nameof(sprice));
                }
            }

            public int Flag
            {
                get => _flag;
                set
                {
                    _flag = value;
                    OnPropertyChanged(nameof(Flag));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void InitializeCardQuantities()
        {
            // Initialize cardQuantities with TextBlock elements
            foreach (var itemName in cardQuantities.Keys.ToList()) // Iterate over the keys of cardQuantities
            {
                // Get the TextBlock for the medic card based on the itemName
                TextBlock quantityTextBlock = cardQuantities[itemName]; // Retrieve the TextBlock from the dictionary

                // Optionally, you can perform additional operations or validations here
                if (quantityTextBlock != null)
                {
                    // Update or initialize the TextBlock as needed
                    // For example, setting some initial values or styles
                }
            }
        }

        private TextBlock GetQuantityTextBlockForCard(string medicName)
        {
            // Implement this method to retrieve the TextBlock for the given medicName from your card UI
            // For example, you could use a visual tree search or maintain a mapping
            return null;
        }
        private async void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(300);
            //query = "select sname from storageinfo where sname like '" + sname.SelectedItem + "%'";
            ds = conn.getData(query);
            List<string> storagenames = new List<string>();
            try
            {


                if (ds != null && ds.Tables.Count > 0)
                {

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

        private void searchBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "" || searchBox1.Text == "🔍  Type to search")
            {
                UserControl_Loaded(sender, e);
                searchBox1.Text = "🔍  Type to search";
                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
                searchBox1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFBFBF"));


            }
        }

        private void searchBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "🔍  Type to search" || searchBox1.Text == "")
            {
                searchBox1.Text = "";
                searchBox1.Foreground = new SolidColorBrush((Color)Colors.Black);

                searchBox1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DB1AF"));

            }


        }
        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(quantityTextBox.Text, out int currentValue))
            {
                quantityTextBox.Text = (currentValue + 1).ToString();
            }
            else
            {
                quantityTextBox.Text = "1";
            }
        }

        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(quantityTextBox.Text, out int currentValue))
            {
                if (currentValue > 1)
                {
                    quantityTextBox.Text = (currentValue - 1).ToString();
                }
                else
                {
                    quantityTextBox.Text = "1";
                }
            }
            else
            {
                quantityTextBox.Text = "1";
            }
        }



        private void pdf_Click(object sender, RoutedEventArgs e)
        {
            /* if (DataGrid.SelectedItems.Count > 0)
             {
                 var targetCollection = DataGrid1.ItemsSource as ObservableCollection<MedicWithFlag>;
                 if (targetCollection == null)
                 {
                     targetCollection = new ObservableCollection<MedicWithFlag>();
                     DataGrid1.ItemsSource = targetCollection;
                 }

                 // Read the quantity from the TextBox
                 if (int.TryParse(quantityTextBox.Text, out int quantity) && quantity > 0)
                 {
                     foreach (DataRowView selectedItem in DataGrid.SelectedItems)
                     {
                         string medicName = selectedItem["mname"].ToString();
                         decimal buyPrice = Convert.ToDecimal(selectedItem["bprice"]);
                         decimal sellPrice = Convert.ToDecimal(selectedItem["sprice"]);

                         // Check the currency and adjust buyPrice and sellPrice accordingly
                         if (pharmacist.check_currency == "IQD")
                         {
                             buyPrice = Math.Round(buyPrice * int.Parse(pharmacist.currencyies), 2);
                             sellPrice = Math.Round(sellPrice * int.Parse(pharmacist.currencyies), 2);
                         }
                         // If check_currency is "$", buyPrice and sellPrice remain unchanged

                         var existingItem = targetCollection.FirstOrDefault(item => item.mname == medicName);
                         if (existingItem != null)
                         {
                             existingItem.Flag += quantity; // Add the specified quantity
                         }
                         else
                         {
                             targetCollection.Add(new MedicWithFlag
                             {
                                 mname = medicName,
                                 bprice = buyPrice,
                                 sprice = sellPrice,
                                 Flag = quantity // Set the quantity
                             });
                         }
                     }

                     // Update the total sell price after adding items
                    *//* pharmacist parentWindow = (pharmacist)Window.GetWindow(this);
                     parentWindow?.disablecheck();*//*
                     UpdateTotalSellPrice();
                 }
                 else
                 {
                     MessageBox.Show("Please enter a valid quantity.");
                 }
             }
             else
             {
                 MessageBox.Show("Please select at least one item to transfer.");
             }*/
        }






        private int GetQuantityFromTextBox()
        {
            if (int.TryParse(quantityTextBox.Text, out int quantity))
            {
                return quantity > 0 ? quantity : 1;
            }
            return 1; // Default to 1 if input is invalid
        }




        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Ensure DataGrid1 has selected items
            if (DataGrid1.SelectedItems.Count > 0)
            {
                // Cast ItemsSource to List<MedicItem>
                var targetCollection = DataGrid1.ItemsSource as List<MedicItem>;

                if (targetCollection != null)
                {
                    // Iterate through the selected items
                    foreach (var selectedItem in DataGrid1.SelectedItems.Cast<MedicItem>().ToList())
                    {
                        // Find the item in the collection
                        var item = targetCollection.FirstOrDefault(x => x.Mname == selectedItem.Mname && x.Bprice == selectedItem.Bprice && x.Sprice == selectedItem.Sprice);

                        if (item != null)
                        {
                            // Remove the item from the DataGrid immediately
                            targetCollection.Remove(item);

                            // Update the quantity on the card item
                            if (cardQuantities.TryGetValue(item.Mname, out TextBlock quantityTextBlock))
                            {
                                // Return the quantity to the card
                                int currentQuantity = int.Parse(quantityTextBlock.Text);
                                int newQuantity = currentQuantity + item.Quantity;
                                quantityTextBlock.Text = newQuantity.ToString();

                                // Call UpdateNumMedicColor to update the card's background color based on the new quantity
                                UpdateNumMedicColor(item.Mname, newQuantity);
                            }
                        }
                    }

                    // Refresh DataGrid1 to reflect the changes
                    DataGrid1.Items.Refresh();

                    // Update the total sell price after modifying the collection
                    UpdateTotalSellPrice();

                    // Optionally handle the case where DataGrid1 is empty
                    if (targetCollection.Count == 0)
                    {
                        // Example: Notify user or perform some action
                        // pharmacist parentWindow = (pharmacist)Window.GetWindow(this);
                        // parentWindow?.enablecheck();
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve the data source for DataGrid1. Please check the data binding.");
                }
            }
            else
            {
                MessageBox.Show("Please select at least one item to remove.");
            }

            // Ensure the total sell price is updated again
            UpdateTotalSellPrice();
        }








        private void UserControl_Loaded(object sender, RoutedEventArgs e)

        {
            fullnameLabel.Content = full;
            /*
            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            query = "select sname from storageinfo";
            ds = conn.getData(query);
            List<string> storagenames = new List<string>();
            try
            {
                if (ds != null && ds.Tables.Count > 0 && sname != null)
                {
                    sname.ItemsSource = ds.Tables[0].DefaultView;
                    sname.DisplayMemberPath = "sname";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            // Update the total sell price initially
            UpdateTotalSellPrice();*/
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void searchBox_TextChanged1(object sender, TextChangedEventArgs e)
        {
            try
            {

                if (mfilter.Content.ToString() == "No Medic Filter")
                {
                    mfilter.Content = "mname";
                }
                if (storage.Content.ToString() == "No Storage Filter")
                {
                    storage.Content = "%";
                }
                Console.WriteLine(mfilter.Content.ToString());
                Console.WriteLine(storage.Content.ToString());


                string filterText = searchBox1.Text.Trim();

                // Clear existing items in CardContainer to prevent duplication
                CardContainer.Children.Clear();
                cardBorders.Clear();
                cardQuantities.Clear();
                if (mfilter.Content.ToString() == "No Medic Filter" && storage.Content.ToString() == "No Storage Filter")
                {



                    if (filterText == "🔍  Type to search" || string.IsNullOrWhiteSpace(filterText))
                    {
                        // Load all items if the filter text is empty or contains the placeholder text
                        ItemsCards_Loaded(sender, e);
                    }
                    else
                    {
                        query = $@"
        SELECT mname, bprice, sprice, exdate, nummedic 
        FROM medicinfo 
        WHERE nummedic > 0 AND mname LIKE '{filterText}%';";

                        ds = conn.getData(query); // Fetch the filtered data from the database

                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                // Get the value of nummedic
                                int nummedic = Convert.ToInt32(row["nummedic"]);
                                string itemName = row["mname"].ToString();

                                // Determine color for the number of medics border based on nummedic value
                                Brush numMedicBackground;
                                if (nummedic > 10)
                                {
                                    numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF39A08B")); // More than 10
                                }
                                else if (nummedic > 5)
                                {
                                    numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFB25F")); // Between 6 and 10
                                }
                                else
                                {
                                    numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA04949")); // 5 or less
                                }

                                // Create a Border for each card (with white background)
                                Border mainBorder = new Border
                                {
                                    Width = 260,
                                    Height = 180,
                                    CornerRadius = new CornerRadius(10), // Set corner radius to 10
                                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDFECE9")), // Main card background is white
                                    BorderBrush = Brushes.White,
                                    BorderThickness = new Thickness(1),
                                    Margin = new Thickness(7),
                                    Opacity = 1.0, // Set the initial opacity for click animation
                                    RenderTransformOrigin = new Point(0.5, 0.5), // Center the scaling animation
                                    RenderTransform = new TransformGroup
                                    {
                                        Children = new TransformCollection
                {
                    new ScaleTransform() // Add a ScaleTransform to be animated
                }
                                    }
                                };
                                // Add mouse event handlers for hover effect and click animation
                                mainBorder.MouseEnter += (s, args) =>
                                {
                                    mainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFD4CF")); // Hover color
                                };
                                mainBorder.MouseLeave += (s, args) =>
                                {
                                    mainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDFECE9")); // Original color
                                };
                                mainBorder.MouseLeftButtonUp += (s, args) =>
                                {
                                    // Trigger click animation
                                    AnimateBorder(mainBorder);

                                    // Handle click logic
                                    int quantity;
                                    bool isQuantityValid = int.TryParse(quantityTextBox.Text, out quantity);

                                    if (!isQuantityValid || quantity <= 0)
                                    {
                                        MessageBox.Show("Please enter a valid quantity.");
                                        return;
                                    }

                                    // Check if the quantity is sufficient
                                    int currentQuantity = int.Parse(cardQuantities[itemName].Text);
                                    if (quantity > currentQuantity)
                                    {
                                        MessageBox.Show("Not enough quantity available.");
                                        return;
                                    }

                                    // Create a new object for DataGrid item
                                    var dataItem = new MedicItem
                                    {
                                        Mname = row["mname"].ToString(),
                                        Bprice = row["bprice"].ToString(),
                                        Sprice = row["sprice"].ToString(),
                                        Quantity = quantity
                                    };

                                    // Update the DataGrid
                                    UpdateDataGrid(row, quantity);

                                    // Update the card quantity
                                    currentQuantity -= quantity;
                                    cardQuantities[itemName].Text = currentQuantity.ToString();
                                    UpdateTotalSellPrice();
                                };

                                // Create a StackPanel to hold the card content
                                StackPanel stackPanel = new StackPanel
                                {
                                    Orientation = Orientation.Vertical,
                                    Margin = new Thickness(10)
                                };

                                // Medic name (bold)
                                TextBlock medicName = new TextBlock
                                {
                                    Text = itemName,
                                    FontWeight = FontWeights.Bold,
                                    Foreground = Brushes.Black,
                                    FontSize = 25,
                                    TextAlignment = TextAlignment.Center,
                                    Margin = new Thickness(0, 0, 0, 10) // Add margin at the bottom
                                };

                                // Create a Grid for bprice and sprice
                                Grid priceGrid = new Grid();
                                priceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                priceGrid.ColumnDefinitions.Add(new ColumnDefinition());

                                // Buy price
                                TextBlock buyPrice = new TextBlock
                                {
                                    Text = "Buy: " + row["bprice"].ToString(),
                                    Foreground = Brushes.Black,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    FontSize = 13,
                                    FontWeight = FontWeights.Medium,
                                    TextAlignment = TextAlignment.Center,
                                    Margin = new Thickness(0, 0, 5, 0) // Add margin for spacing between buy and sell prices
                                };
                                Grid.SetColumn(buyPrice, 0);

                                // Sell price
                                TextBlock sellPrice = new TextBlock
                                {
                                    Text = "Sell: " + row["sprice"].ToString(),
                                    Foreground = Brushes.Black,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    FontSize = 13,
                                    FontWeight = FontWeights.Medium,
                                    TextAlignment = TextAlignment.Center
                                };
                                Grid.SetColumn(sellPrice, 1);

                                // Add bprice and sprice to the price grid
                                priceGrid.Children.Add(buyPrice);
                                priceGrid.Children.Add(sellPrice);

                                // Expiration date
                                TextBlock exDate = new TextBlock
                                {
                                    Text = "Expires: " + Convert.ToDateTime(row["exdate"]).ToString("yyyy-MM-dd"),
                                    Foreground = Brushes.Black,
                                    Margin = new Thickness(0, 10, 0, 0),
                                    FontSize = 15,
                                    FontWeight = FontWeights.Medium,
                                    TextAlignment = TextAlignment.Center
                                };

                                // Create a Border specifically for the nummedic text
                                Border numMedicBorder = new Border
                                {
                                    Background = numMedicBackground, // Background color based on nummedic value
                                    CornerRadius = new CornerRadius(10), // Rounded corners for nummedic border
                                    Padding = new Thickness(5), // Padding inside the border
                                    Margin = new Thickness(0, 25, 0, 0), // Margin for spacing above
                                    HorizontalAlignment = HorizontalAlignment.Center, // Center the number in the card
                                    Width = 120 // Set fixed width to ensure it's wide enough regardless of text length
                                };

                                // Number of medics (bold)
                                TextBlock numMedic = new TextBlock
                                {
                                    Text = nummedic.ToString(),
                                    FontWeight = FontWeights.Bold,
                                    Foreground = Brushes.White, // White text for readability
                                    FontSize = 19,
                                    TextAlignment = TextAlignment.Center
                                };

                                // Add the nummedic TextBlock inside its Border
                                numMedicBorder.Child = numMedic;

                                // Add all the elements to the StackPanel
                                stackPanel.Children.Add(medicName);   // Top element (mname)
                                stackPanel.Children.Add(priceGrid);   // Price grid
                                stackPanel.Children.Add(exDate);      // Expiration date
                                stackPanel.Children.Add(numMedicBorder); // Number of medics within its colored border

                                // Add StackPanel to the main Border
                                mainBorder.Child = stackPanel;

                                // Store references to update later
                                cardBorders[itemName] = mainBorder;
                                cardQuantities[itemName] = numMedic;

                                // Add the main Border (card) to the WrapPanel (CardContainer)
                                CardContainer.Children.Add(mainBorder);
                            }
                        }
                    }
                }
                else
                {
                    if (filterText == "🔍  Type to search" || string.IsNullOrWhiteSpace(filterText))
                    {
                        // Load all items if the filter text is empty or contains the placeholder text
                        ItemsCards_Loaded(sender, e);
                    }
                    else
                    {

                        query = $@"
        SELECT mname, bprice, sprice, exdate, nummedic 
        FROM medicinfo 
        WHERE nummedic > 0 AND {mfilter.Content.ToString()} LIKE '{filterText}%' AND sname like '" + storage.Content.ToString() + "';";

                        ds = conn.getData(query); // Fetch the filtered data from the database

                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                // Get the value of nummedic
                                int nummedic = Convert.ToInt32(row["nummedic"]);
                                string itemName = row["mname"].ToString();

                                // Determine color for the number of medics border based on nummedic value
                                Brush numMedicBackground;
                                if (nummedic > 10)
                                {
                                    numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF39A08B")); // More than 10
                                }
                                else if (nummedic > 5)
                                {
                                    numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFB25F")); // Between 6 and 10
                                }
                                else
                                {
                                    numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA04949")); // 5 or less
                                }

                                // Create a Border for each card (with white background)
                                Border mainBorder = new Border
                                {
                                    Width = 260,
                                    Height = 180,
                                    CornerRadius = new CornerRadius(10), // Set corner radius to 10
                                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDFECE9")), // Main card background is white
                                    BorderBrush = Brushes.White,
                                    BorderThickness = new Thickness(1),
                                    Margin = new Thickness(7),
                                    Opacity = 1.0, // Set the initial opacity for click animation
                                    RenderTransformOrigin = new Point(0.5, 0.5), // Center the scaling animation
                                    RenderTransform = new TransformGroup
                                    {
                                        Children = new TransformCollection
                {
                    new ScaleTransform() // Add a ScaleTransform to be animated
                }
                                    }
                                };
                                // Add mouse event handlers for hover effect and click animation
                                mainBorder.MouseEnter += (s, args) =>
                                {
                                    mainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFD4CF")); // Hover color
                                };
                                mainBorder.MouseLeave += (s, args) =>
                                {
                                    mainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDFECE9")); // Original color
                                };
                                mainBorder.MouseLeftButtonUp += (s, args) =>
                                {
                                    // Trigger click animation
                                    AnimateBorder(mainBorder);

                                    // Handle click logic
                                    int quantity;
                                    bool isQuantityValid = int.TryParse(quantityTextBox.Text, out quantity);

                                    if (!isQuantityValid || quantity <= 0)
                                    {
                                        MessageBox.Show("Please enter a valid quantity.");
                                        return;
                                    }

                                    // Check if the quantity is sufficient
                                    int currentQuantity = int.Parse(cardQuantities[itemName].Text);
                                    if (quantity > currentQuantity)
                                    {
                                        MessageBox.Show("Not enough quantity available.");
                                        return;
                                    }

                                    // Create a new object for DataGrid item
                                    var dataItem = new MedicItem
                                    {
                                        Mname = row["mname"].ToString(),
                                        Bprice = row["bprice"].ToString(),
                                        Sprice = row["sprice"].ToString(),
                                        Quantity = quantity
                                    };

                                    // Update the DataGrid
                                    UpdateDataGrid(row, quantity);

                                    // Update the card quantity
                                    currentQuantity -= quantity;
                                    cardQuantities[itemName].Text = currentQuantity.ToString();
                                    UpdateTotalSellPrice();
                                };

                                // Create a StackPanel to hold the card content
                                StackPanel stackPanel = new StackPanel
                                {
                                    Orientation = Orientation.Vertical,
                                    Margin = new Thickness(10)
                                };

                                // Medic name (bold)
                                TextBlock medicName = new TextBlock
                                {
                                    Text = itemName,
                                    FontWeight = FontWeights.Bold,
                                    Foreground = Brushes.Black,
                                    FontSize = 25,
                                    TextAlignment = TextAlignment.Center,
                                    Margin = new Thickness(0, 0, 0, 10) // Add margin at the bottom
                                };

                                // Create a Grid for bprice and sprice
                                Grid priceGrid = new Grid();
                                priceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                priceGrid.ColumnDefinitions.Add(new ColumnDefinition());

                                // Buy price
                                TextBlock buyPrice = new TextBlock
                                {
                                    Text = "Buy: " + row["bprice"].ToString(),
                                    Foreground = Brushes.Black,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    FontSize = 13,
                                    FontWeight = FontWeights.Medium,
                                    TextAlignment = TextAlignment.Center,
                                    Margin = new Thickness(0, 0, 5, 0) // Add margin for spacing between buy and sell prices
                                };
                                Grid.SetColumn(buyPrice, 0);

                                // Sell price
                                TextBlock sellPrice = new TextBlock
                                {
                                    Text = "Sell: " + row["sprice"].ToString(),
                                    Foreground = Brushes.Black,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    FontSize = 13,
                                    FontWeight = FontWeights.Medium,
                                    TextAlignment = TextAlignment.Center
                                };
                                Grid.SetColumn(sellPrice, 1);

                                // Add bprice and sprice to the price grid
                                priceGrid.Children.Add(buyPrice);
                                priceGrid.Children.Add(sellPrice);

                                // Expiration date
                                TextBlock exDate = new TextBlock
                                {
                                    Text = "Expires: " + Convert.ToDateTime(row["exdate"]).ToString("yyyy-MM-dd"),
                                    Foreground = Brushes.Black,
                                    Margin = new Thickness(0, 10, 0, 0),
                                    FontSize = 15,
                                    FontWeight = FontWeights.Medium,
                                    TextAlignment = TextAlignment.Center
                                };

                                // Create a Border specifically for the nummedic text
                                Border numMedicBorder = new Border
                                {
                                    Background = numMedicBackground, // Background color based on nummedic value
                                    CornerRadius = new CornerRadius(10), // Rounded corners for nummedic border
                                    Padding = new Thickness(5), // Padding inside the border
                                    Margin = new Thickness(0, 25, 0, 0), // Margin for spacing above
                                    HorizontalAlignment = HorizontalAlignment.Center, // Center the number in the card
                                    Width = 120 // Set fixed width to ensure it's wide enough regardless of text length
                                };

                                // Number of medics (bold)
                                TextBlock numMedic = new TextBlock
                                {
                                    Text = nummedic.ToString(),
                                    FontWeight = FontWeights.Bold,
                                    Foreground = Brushes.White, // White text for readability
                                    FontSize = 19,
                                    TextAlignment = TextAlignment.Center
                                };

                                // Add the nummedic TextBlock inside its Border
                                numMedicBorder.Child = numMedic;

                                // Add all the elements to the StackPanel
                                stackPanel.Children.Add(medicName);   // Top element (mname)
                                stackPanel.Children.Add(priceGrid);   // Price grid
                                stackPanel.Children.Add(exDate);      // Expiration date
                                stackPanel.Children.Add(numMedicBorder); // Number of medics within its colored border

                                // Add StackPanel to the main Border
                                mainBorder.Child = stackPanel;

                                // Store references to update later
                                cardBorders[itemName] = mainBorder;
                                cardQuantities[itemName] = numMedic;

                                // Add the main Border (card) to the WrapPanel (CardContainer)
                                CardContainer.Children.Add(mainBorder);
                            }
                        }
                    }
                }
            }
            catch
            {

            }

        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            // Ensure DataGrid1 has selected items
            if (DataGrid1.SelectedItems.Count > 0)
            {
                var targetCollection = DataGrid1.ItemsSource as List<MedicItem>;

                if (targetCollection != null)
                {
                    foreach (var selectedItem in DataGrid1.SelectedItems.Cast<MedicItem>().ToList())
                    {
                        var item = targetCollection.FirstOrDefault(x => x.Mname == selectedItem.Mname && x.Bprice == selectedItem.Bprice && x.Sprice == selectedItem.Sprice);

                        if (item != null)
                        {
                            if (item.Quantity > 1)
                            {
                                item.Quantity--;
                            }
                            else
                            {
                                targetCollection.Remove(item);
                            }

                            if (cardQuantities.TryGetValue(item.Mname, out TextBlock quantityTextBlock))
                            {
                                int newQuantity = int.Parse(quantityTextBlock.Text) + 1;
                                quantityTextBlock.Text = newQuantity.ToString();

                                // Update the color based on the new quantity
                                UpdateNumMedicColor(item.Mname, newQuantity);
                            }
                        }
                    }

                    // Refresh DataGrid1 to reflect the changes
                    DataGrid1.Items.Refresh();
                    UpdateTotalSellPrice();

                    // Handle the case where DataGrid1 is empty
                    if (targetCollection.Count == 0)
                    {
                        // Example: Notify user or perform some action
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve the data source for DataGrid1. Please check the data binding.");
                }
            }
            else
            {
                MessageBox.Show("Please select at least one item to modify.");
            }

            UpdateTotalSellPrice();
        }











        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Retrieve the maximum billId from the database
            query = "SELECT MAX(billId) AS MaxBillId FROM bills";
            ds = conn.getData(query);
            maxid = int.Parse(ds.Tables[0].Rows[0][0].ToString());

            // Cast ItemsSource to List<MedicItem>
            var medicList = DataGrid1.ItemsSource as List<MedicItem>;

            if (medicList != null)
            {
                // Calculate the new billId
                bid = maxid + 1;

                // Update the quantities in the database and on the cards
                foreach (var medic in medicList)
                {
                    // Construct query to update the database
                    query = $"UPDATE medicinfo SET nummedic = nummedic - {medic.Quantity} WHERE mname = '{medic.Mname}'";

                    // Execute the query
                    try
                    {
                        conn.setData(query);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while updating medic: {ex.Message}");
                    }

                    // Update the quantity on the card item


                }

                // Convert price to correct currency value
                decimal priceValue;
                string priceText = pricet.Content?.ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(priceText))
                {
                    MessageBox.Show("Price text is null or empty.");
                    return; // Exit the method if priceText is invalid
                }

                // Check currency and adjust priceValue accordingly
                if (pharmacist.check_currency == "IQD")
                {
                    // Remove commas and convert to decimal
                    priceText = priceText.Replace(",", "").Replace(" IQD", ""); // Remove " IQD" if it exists
                    if (decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out priceValue))
                    {
                        // Convert to USD by dividing by 1300
                        priceValue /= int.Parse(pharmacist.currencyies ?? "1300"); // Default to 1300 if null
                        priceValue = Math.Round(priceValue, 2); // Round to 2 decimal places
                    }
                    else
                    {
                        MessageBox.Show("Failed to parse the price.");
                        return; // Exit the method if parsing fails
                    }
                }
                else
                {
                    // For other currencies like USD
                    if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.CurrentCulture, out priceValue))
                    {
                        MessageBox.Show("Failed to parse the price.");
                        return; // Exit the method if parsing fails
                    }
                }

                // Prepare SQL query to insert a new bill
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string by_ = Login_.username;
                query = "INSERT INTO bills (from_, too_, Price, bdate, billId, type, by_, currency) " +
                        $"VALUES ('Pharmacy', 'Customer', '{priceValue}', '{currentDate}', '{bid}', 'sell', '{by_}', '{pharmacist.currencyies}')";

                // Execute the query
                try
                {
                    conn.setData(query);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while inserting the bill: {ex.Message}");
                }

                // Notify success
                MessageBox.Show("All medics have been updated successfully.");

                // Clear the DataGrid and reset UI elements
                UserControl_Loaded(sender, e);
                ClearDataGrid1();
                pricee.Content = "0,00";
                pricet.Content = "0,00";
            }
            else
            {
                MessageBox.Show("No medics to update.");
            }
            ItemsCards_Loaded(sender, e); // Reload cards to update quantities based on the database

        }


        private void ClearDataGrid1()
        {
            // Ensure DataGrid1 has items
            if (DataGrid1.ItemsSource != null)
            {
                // Cast ItemsSource to a List
                var itemsSource = DataGrid1.ItemsSource as IList<MedicItem>;

                if (itemsSource != null)
                {
                    // Clear all items from the list
                    itemsSource.Clear();

                    // Notify DataGrid1 that its ItemsSource has changed
                    DataGrid1.ItemsSource = null;
                    DataGrid1.ItemsSource = itemsSource;
                }
                else
                {
                    MessageBox.Show("Failed to retrieve the data source for DataGrid1.");
                }
            }
        }

        private void searchBox1_LostFocus1(object sender, RoutedEventArgs e)
        {

        }

        private void searchBox1_GotFocus1(object sender, RoutedEventArgs e)
        {

        }

        private void searchBox1_GotFocus2(object sender, RoutedEventArgs e)
        {

        }

        private void searchBox1_LostFocus2(object sender, RoutedEventArgs e)
        {

        }

        private async void searchBox_TextChanged2(object sender, TextChangedEventArgs e)
        {

        }




        private void back_Copy1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                brcode = InputTextBox.Text;

            }
        }
        private void pdf_Click1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Clicka(object sender, RoutedEventArgs e)
        {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void sname_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void sname_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void sname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }






        private void Button_Click_2(object sender, RoutedEventArgs e)
        {



        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private async void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(100); // Simulate delay for barcode reader input
            string brcode = InputTextBox.Text;

            if (!string.IsNullOrEmpty(brcode))
            {
                try
                {
                    query = $"SELECT * FROM medicinfo WHERE codeid = '{brcode}'";
                    ds = conn.getData(query);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var selectedItem = ds.Tables[0].Rows[0];
                        int availableQuantity = Convert.ToInt32(selectedItem["nummedic"]); // Get available quantity
                        int requestedQuantity = 1;

                        // Check if the available quantity is 0
                        if (availableQuantity == 0)
                        {
                            MessageBox.Show("This item is out of stock.");
                        }
                        else
                        {
                            // Handle item click if quantity is greater than 0
                            HandleItemClick(selectedItem, requestedQuantity);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No item found with the provided barcode.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                    Debug.WriteLine($"Exception: {ex}");
                }
            }

            InputTextBox.Clear();
        }




        private void HandleItemClick(DataRow row, int requestedQuantity)
        {
            string itemName = row["mname"].ToString();
            int availableQuantity = int.Parse(cardQuantities[itemName].Text);

            // Check if the quantity is valid
            if (requestedQuantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            // Check if the quantity is sufficient
            if (requestedQuantity > availableQuantity)
            {
                MessageBox.Show("Not enough quantity available.");
                return;
            }

            // Create a new object for DataGrid item
            var dataItem = new MedicItem
            {
                Mname = row["mname"].ToString(),
                Bprice = row["bprice"].ToString(),
                Sprice = row["sprice"].ToString(),
                Quantity = requestedQuantity
            };

            // Update the DataGrid
            UpdateDataGrid(row, requestedQuantity);

            // Update the card quantity
            availableQuantity -= requestedQuantity;
            cardQuantities[itemName].Text = availableQuantity.ToString();
            UpdateTotalSellPrice();

            // Update the background color based on the new quantity
            UpdateNumMedicColor(itemName, availableQuantity);
        }

        private void ItemsCards_Loaded(object sender, RoutedEventArgs e)
        {

            DataGrid1.ItemsSource = null;

            // Clear existing items in CardContainer to prevent duplication
            CardContainer.Children.Clear();
            cardBorders.Clear();
            cardQuantities.Clear();

            query = @"
SELECT mname, bprice, sprice, exdate, nummedic 
FROM medicinfo 
WHERE nummedic > 0;";

            ds = conn.getData(query); // Fetch the data from the database

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    // Get the value of nummedic
                    int nummedic = Convert.ToInt32(row["nummedic"]);
                    string itemName = row["mname"].ToString();

                    // Determine color for the number of medics border based on nummedic value
                    if (nummedic > 10)
                    {
                        numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF39A08B")); // More than 10
                    }
                    else if (nummedic > 5)
                    {
                        numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFB25F")); // Between 6 and 10
                    }
                    else
                    {
                        numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA04949")); // 5 or less
                    }

                    // Create a Border for each card (with white background)
                    Border mainBorder = new Border
                    {
                        Width = 260,
                        Height = 180,
                        CornerRadius = new CornerRadius(10), // Set corner radius to 10
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDFECE9")), // Main card background is white
                        BorderBrush = Brushes.White,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(7),
                        Opacity = 1.0, // Set the initial opacity for click animation
                        RenderTransformOrigin = new Point(0.5, 0.5), // Center the scaling animation
                        RenderTransform = new TransformGroup
                        {
                            Children = new TransformCollection
                {
                    new ScaleTransform() // Add a ScaleTransform to be animated
                }
                        }
                    };
                    // Add mouse event handlers for hover effect and click animation
                    mainBorder.MouseEnter += (s, args) =>
                    {
                        mainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFD4CF")); // Hover color
                    };
                    mainBorder.MouseLeave += (s, args) =>
                    {
                        mainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDFECE9")); // Original color
                    };
                    mainBorder.MouseLeftButtonUp += (s, args) =>
                    {
                        // Trigger click animation
                        AnimateBorder(mainBorder);

                        // Handle click logic
                        int quantity;
                        bool isQuantityValid = int.TryParse(quantityTextBox.Text, out quantity);

                        if (isQuantityValid)
                        {
                            HandleItemClick(row, quantity);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid quantity.");
                        }
                    };


                    // Create a StackPanel to hold the card content
                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(10)
                    };

                    // Medic name (bold)
                    TextBlock medicName = new TextBlock
                    {
                        Text = itemName,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Black,
                        FontSize = 25,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 10) // Add margin at the bottom
                    };

                    // Create a Grid for bprice and sprice
                    Grid priceGrid = new Grid();
                    priceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    priceGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    // Buy price
                    TextBlock buyPrice = new TextBlock
                    {
                        Text = "Buy: " + row["bprice"].ToString(),
                        Foreground = Brushes.Black,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 13,
                        FontWeight = FontWeights.Medium,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 5, 0) // Add margin for spacing between buy and sell prices
                    };
                    Grid.SetColumn(buyPrice, 0);

                    // Sell price
                    TextBlock sellPrice = new TextBlock
                    {
                        Text = "Sell: " + row["sprice"].ToString(),
                        Foreground = Brushes.Black,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 13,
                        FontWeight = FontWeights.Medium,
                        TextAlignment = TextAlignment.Center
                    };
                    Grid.SetColumn(sellPrice, 1);

                    // Add bprice and sprice to the price grid
                    priceGrid.Children.Add(buyPrice);
                    priceGrid.Children.Add(sellPrice);

                    // Expiration date
                    TextBlock exDate = new TextBlock
                    {
                        Text = "Expires: " + Convert.ToDateTime(row["exdate"]).ToString("yyyy-MM-dd"),
                        Foreground = Brushes.Black,
                        Margin = new Thickness(0, 10, 0, 0),
                        FontSize = 15,
                        FontWeight = FontWeights.Medium,
                        TextAlignment = TextAlignment.Center
                    };

                    // Create a Border specifically for the nummedic text
                    Border numMedicBorder = new Border
                    {
                        Background = numMedicBackground, // Background color based on nummedic value
                        CornerRadius = new CornerRadius(10), // Rounded corners for nummedic border
                        Padding = new Thickness(5), // Padding inside the border
                        Margin = new Thickness(0, 25, 0, 0), // Margin for spacing above
                        HorizontalAlignment = HorizontalAlignment.Center, // Center the number in the card
                        Width = 120 // Set fixed width to ensure it's wide enough regardless of text length
                    };

                    // Number of medics (bold)
                    TextBlock numMedic = new TextBlock
                    {
                        Text = nummedic.ToString(),
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White, // White text for readability
                        FontSize = 19,
                        TextAlignment = TextAlignment.Center
                    };

                    // Add the nummedic TextBlock inside its Border
                    numMedicBorder.Child = numMedic;

                    // Add all the elements to the StackPanel
                    stackPanel.Children.Add(medicName);   // Top element (mname)
                    stackPanel.Children.Add(priceGrid);   // Price grid
                    stackPanel.Children.Add(exDate);      // Expiration date
                    stackPanel.Children.Add(numMedicBorder); // Number of medics within its colored border

                    // Add StackPanel to the main Border
                    mainBorder.Child = stackPanel;

                    // Store references to update later
                    cardBorders[itemName] = mainBorder;
                    cardQuantities[itemName] = numMedic;

                    // Add the main Border (card) to the WrapPanel (CardContainer)
                    CardContainer.Children.Add(mainBorder);
                }
            }
        }
        private void UpdateNumMedicColor(string itemName, int nummedic)
        {
            // Determine the background color based on the nummedic value
            Brush numMedicBackground;
            if (nummedic > 10)
            {
                numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF39A08B")); // More than 10
            }
            else if (nummedic > 5)
            {
                numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFB25F")); // Between 6 and 10
            }
            else
            {
                numMedicBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA04949")); // 5 or less
            }

            // Update the border background color for the specific item
            if (cardBorders.ContainsKey(itemName))
            {
                Border mainBorder = cardBorders[itemName]; // Retrieve the corresponding main border

                if (mainBorder.Child is StackPanel stackPanel)
                {
                    // Loop through the children to find the nummedic border
                    foreach (var child in stackPanel.Children)
                    {
                        if (child is Border border && border.Child is TextBlock textBlock && textBlock.Text == nummedic.ToString())
                        {
                            // Found the nummedic Border, update its background
                            border.Background = numMedicBackground;
                            break;
                        }
                    }
                }
            }
        }



        // Method to handle the click animation
        private void AnimateBorder(Border targetBorder)
        {
            if (targetBorder != null)
            {
                // Set the RenderTransformOrigin to the center
                targetBorder.RenderTransformOrigin = new Point(0.5, 0.5);

                // Retrieve the storyboard from resources
                Storyboard storyboard = (Storyboard)this.Resources["ClickEffectStoryboard"];

                if (storyboard != null)
                {
                    // Apply the storyboard to the target border
                    Storyboard.SetTarget(storyboard, targetBorder);

                    // Start the animation
                    storyboard.Begin();
                }
                else
                {
                    // Handle the case where the storyboard is not found
                    MessageBox.Show("ClickEffectStoryboard resource not found.");
                }
            }
        }



        // Method to update the DataGrid
        private void UpdateDataGrid(DataRow row, int quantity)
        {
            var itemsSource = DataGrid1.ItemsSource as IList<MedicItem>;

            if (itemsSource == null)
            {
                itemsSource = new List<MedicItem>();
                DataGrid1.ItemsSource = itemsSource;
            }

            var existingItem = itemsSource.FirstOrDefault(item => item.Mname == row["mname"].ToString());

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                DataGrid1.Items.Refresh();
            }
            else
            {
                var dataItem = new MedicItem
                {
                    Mname = row["mname"].ToString(),
                    Bprice = row["bprice"].ToString(),
                    Sprice = row["sprice"].ToString(),
                    Quantity = quantity
                };

                itemsSource.Add(dataItem);
                DataGrid1.Items.Refresh();
            }
        }



        private void UpdateCardQuantity(DataRow row, int quantity)
        {
            foreach (Border border in CardContainer.Children.OfType<Border>())
            {
                StackPanel stackPanel = border.Child as StackPanel;
                if (stackPanel != null)
                {
                    TextBlock medicName = stackPanel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Text == row["mname"].ToString());
                    if (medicName != null)
                    {
                        Border numMedicBorder = stackPanel.Children.OfType<Border>().FirstOrDefault(b => b.Background.ToString() == ((SolidColorBrush)new BrushConverter().ConvertFromString("#FFDFECE9")).ToString());
                        if (numMedicBorder != null)
                        {
                            TextBlock numMedic = numMedicBorder.Child as TextBlock;
                            int currentQuantity = Convert.ToInt32(numMedic.Text);

                            if (currentQuantity - quantity > 0)
                            {
                                numMedic.Text = (currentQuantity - quantity).ToString();
                            }
                            else
                            {
                                CardContainer.Children.Remove(border);
                                DataGrid1.Items.Refresh();
                            }
                        }
                        break;
                    }
                }
            }
        }









        private void ItemsCards_Loaded_1(object sender, RoutedEventArgs e)
        {

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

            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-948-stock-share-hover-pinch (2).gif"));
            ImageBehavior.SetAnimatedSource(accountentimage4, image5);
            ImageBehavior.SetRepeatBehavior(accountentimage4, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image6 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1339-sale-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage5, image6);
            ImageBehavior.SetRepeatBehavior(accountentimage5, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-solid-39-trash-hover-trash-empty.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage6, image7);
            ImageBehavior.SetRepeatBehavior(accountentimage6, System.Windows.Media.Animation.RepeatBehavior.Forever);

            var image9 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-751-share-hover-pointing.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage8, image9);
            ImageBehavior.SetRepeatBehavior(accountentimage8, System.Windows.Media.Animation.RepeatBehavior.Forever);


        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
            StartGifAnimation();
        }

        private void searchBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE4E4E4"));
        }

        private void searchBox1_MouseLeave(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush(Colors.White);
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            filter1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7F7F7F"));
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            filter1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF49B3A2"));
        }

        private async void filter1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            filtersell childControl = new filtersell(this);
            childControl.Visibility = Visibility.Collapsed;

            // Add it to the Grid (assuming your Grid has x:Name="gridContainer" in XAML)
            gridContainer.Children.Add(childControl);
            childControl.Visibility = Visibility.Visible;

            childControl.f.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)this.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
        }




        private async void bprice_Copy1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bprice_Copy1.Text == "" || bprice_Copy1.Text == "Search Medic Name...")
            {
                query = "SELECT * FROM medicinfo";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid1.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else
            {
                query = "SELECT * FROM medicinfo WHERE mname LIKE '" + bprice_Copy1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid1.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }

            // Optionally update DataGrid with new data
            var dataItems = ds.Tables[0].AsEnumerable().Select(row => new MedicItem
            {
                Mname = row["mname"].ToString(),
                Bprice = row["bprice"].ToString(),
                Sprice = row["sprice"].ToString(),
                Quantity = 0 // Set initial quantity to 0 or other default value
            }).ToList();

            DataGrid1.ItemsSource = dataItems;
        }


        private void bprice_Copy1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (bprice_Copy1.Text == "")
            {
                bprice_Copy1.Text = "Search Medic Name...";
                bprice_Copy1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
            }
        }

        private void bprice_Copy1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (bprice_Copy1.Text == "Search Medic Name...")
            {
                bprice_Copy1.Text = "";
                bprice_Copy1.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (bprice_Copy1.Text == "")
            {
                bprice_Copy1.Text = "";
                bprice_Copy1.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void UpdateTotalSellPrice()
        {
            // Check if DataGrid1 has items
            var items = DataGrid1.ItemsSource as List<MedicItem>;
            if (items == null || !items.Any())
            {
                pricee.Content = "0";
                pricet.Content = "0";
                return;
            }

            // Calculate total sell price
            decimal totalSellPrice = 0;
            foreach (var item in items)
            {
                decimal sellPrice;
                if (decimal.TryParse(item.Sprice, out sellPrice))
                {
                    totalSellPrice += sellPrice * item.Quantity; // Multiply by quantity
                }
            }

            // Update price and pricet labels
            pricee.Content = totalSellPrice.ToString();
            pricet.Content = totalSellPrice.ToString();
        }

        private void bprice_Copy1_MouseEnter(object sender, MouseEventArgs e)
        {
            bprice_Copy1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9FC7B5"));
        }

        private void bprice_Copy1_MouseLeave(object sender, MouseEventArgs e)
        {
            bprice_Copy1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE9FBF3"));

        }

        private void mybutton_Copy1_Click(object sender, MouseButtonEventArgs e)
        {
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

                fd.Blocks.Add(new System.Windows.Documents.Paragraph(new Run($"Date and Time: {DateTime.Now:f}\n"))
                {
                    FontSize = 14,
                    FontWeight = System.Windows.FontWeights.Bold,
                    TextAlignment = System.Windows.TextAlignment.Center
                });

                // Add the total price


                // Create a WPF Table (System.Windows.Documents.Table)
                System.Windows.Documents.Table table = new System.Windows.Documents.Table();
                table.CellSpacing = 0;
                table.BorderThickness = new Thickness(0.5);
                table.BorderBrush = Brushes.Black;

                // Calculate the total width of all columns
                double totalColumnWidth = 0;
                foreach (var column in DataGrid1.Columns)
                {
                    totalColumnWidth += column.ActualWidth;
                }

                // Ensure that the total column width does not exceed printable area
                double availableWidth = printDialog.PrintableAreaWidth - 100; // Subtract margins
                double scaleFactor = availableWidth / totalColumnWidth;

                // Add columns to the Table
                foreach (var column in DataGrid1.Columns)
                {
                    table.Columns.Add(new TableColumn() { Width = new GridLength(column.ActualWidth * scaleFactor) });
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
                    else
                    {
                        DataGrid1.ScrollIntoView(item);
                        row = (DataGridRow)DataGrid1.ItemContainerGenerator.ContainerFromItem(item);

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
                }
                table.RowGroups.Add(bodyGroup);

                // Add the table to the FlowDocument
                fd.Blocks.Add(table);

                // Print the FlowDocument
                printDialog.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, "Print DataGrid");
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
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
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
            fadeOutStoryboard.Begin();
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }

}






