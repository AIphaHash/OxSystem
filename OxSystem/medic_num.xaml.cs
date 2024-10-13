using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using System.IO;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using Org.BouncyCastle.Math;
using WpfAnimatedGif;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Remoting.Messaging;
namespace OxSystem
{
    /// <summary>
    /// Interaction logic for medic_num.xaml
    /// </summary>
    public partial class medic_num : UserControl
    {
        private string comboBoxText;
        private string _currentTime;
        private DispatcherTimer _timer;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        private List<int> selectedIds = new List<int>();
        public static string medicn;
        public static string buyp;
        public static string sellp;
        public static string expired;
        public static string manud;
        public static string numberm;
        public static string storagen;
        public static string full = Login_.fullName;

        public static string id;
        public medic_num()
        {
            InitializeComponent();
            DataContext = this;
            UpdateCurrentTime();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public async void passData(string fc)
        {
            query = "select * from medicinfo where mid = '" + fc + "'";
            ds = conn.getData(query);
            medicn = ds.Tables[0].Rows[0][1].ToString();

            buyp = ds.Tables[0].Rows[0][2].ToString();

            sellp = ds.Tables[0].Rows[0][3].ToString();

            expired = ds.Tables[0].Rows[0][4].ToString();

            manud = ds.Tables[0].Rows[0][5].ToString();
            numberm = ds.Tables[0].Rows[0][6].ToString();

            storagen = ds.Tables[0].Rows[0][7].ToString();

            


            editmedic.Username.Text = medicn;
            editmedic.Password.Text = numberm;
            
            editmedic.Address.Text = buyp;
            editmedic.full.Text = sellp;
            
           
            editmedic.editborder.Visibility = Visibility.Collapsed;
            editmedic.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)editmedic.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            editmedic.editborder.Visibility = Visibility.Visible;
            Storyboard moveInStoryboard = (Storyboard)editmedic.FindResource("MoveDownStoryboard");
            moveInStoryboard.Begin();

        }
        private void medic_num_Loaded(object sender, RoutedEventArgs e)
        {
           


        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                query = "select * from medicinfo WHERE exdate > GETDATE();";
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

        private async void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox1.Text == "" || searchBox1.Text == "🔍  Type to search ")
            {
                query = "select * from medicinfo";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }

            else if (comboBoxText == "ID")
            {
                query = "select * from medicinfo where mid like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Medic Name")
            {
                query = "select * from medicinfo where mname like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Buy Price")
            {
                query = "select * from medicinfo where bprice like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Sell Price")
            {
                query = "select * from medicinfo where email like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Expire Date")
            {
                query = "select * from medicinfo where exdate like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Manufacture Date")
            {
                query = "select * from medicinfo where madate like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Medic Num")
            {
                query = "select * from medicinfo where nummedic like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if (comboBoxText == "Storage Name")
            {
                query = "select * from medicinfo where sname like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }



            else
            {

                query = "select * from medicinfo where mname like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
        }

        private async void pdf_Click(object sender, RoutedEventArgs e)
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
                foreach (var column in DataGrid.Columns)
                {
                    totalColumnWidth += column.ActualWidth;
                }

                // Ensure that the total column width does not exceed printable area
                double availableWidth = printDialog.PrintableAreaWidth - 100; // Subtract margins
                double scaleFactor = availableWidth / totalColumnWidth;

                // Add columns to the Table
                foreach (var column in DataGrid.Columns)
                {
                    table.Columns.Add(new TableColumn() { Width = new GridLength(column.ActualWidth * scaleFactor) });
                }

                // Add headers to the Table
                TableRowGroup headerGroup = new TableRowGroup();
                TableRow headerRow = new TableRow();
                foreach (var column in DataGrid.Columns)
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
                foreach (var item in DataGrid.Items)
                {
                    DataGridRow row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromItem(item);
                    if (row != null)
                    {
                        TableRow bodyRow = new TableRow();
                        foreach (var column in DataGrid.Columns)
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
                        DataGrid.ScrollIntoView(item);
                        row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromItem(item);

                        if (row != null)
                        {
                            TableRow bodyRow = new TableRow();
                            foreach (var column in DataGrid.Columns)
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
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

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editmedic.Username.Text = medicn;
            editmedic.Password.Text = numberm;
           
            editmedic.Address.Text = buyp;
            editmedic.full.Text = sellp;
         



            if (DataGrid.SelectedItem != null)
            {
                var sr = DataGrid.SelectedItem as DataRowView;
                if (sr != null)
                {
                    var fc = sr[0].ToString();
                    passData(fc);
                    UserControl_Loaded(sender, e);
                }
            }
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

        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
             query = @"
SELECT TOP 1
    sname,
    size
FROM 
    storageinfo
ORDER BY 
    size DESC;";

            ds = conn.getData(query);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0]["sname"];
                if (result != DBNull.Value)
                {
                    string mostStorage = result.ToString(); // Convert to string
                    mosts.Content = mostStorage; // Set to label
                }
                else
                {
                    mosts.Content = "No storage data"; // Default if no data
                }
            }
            else
            {
                mosts.Content = "No storage data"; // Default if no records or result is null
            }


            query = @"
SELECT TOP 1
    FROM_ AS SupplierName,
    COUNT(*) AS SupplyCount
FROM 
    bills
GROUP BY 
    FROM_
ORDER BY 
    SupplyCount DESC;";

            ds = conn.getData(query);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0]["SupplierName"];
                if (result != DBNull.Value)
                {
                    string mostSupplier = result.ToString(); // Convert to string
                    mostsup.Content = mostSupplier; // Set to label
                }
                else
                {
                    mostsup.Content = "No supplier data"; // Default if no data
                }
            }
            else
            {
                mostsup.Content = "No supplier data"; // Default if no records or result is null
            }


            query = @"
SELECT 
    ISNULL(SUM(nummedic), 0) AS TotalMedics
FROM 
    medicinfo;";

            ds = conn.getData(query);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0]["TotalMedics"];
                if (result != DBNull.Value)
                {
                    int totalMedics = Convert.ToInt32(result); // Convert to an integer
                    totalmedic.Content = totalMedics.ToString(); // Update label with the total number of medics
                }
                else
                {
                    totalmedic.Content = "0"; // Default if there are no medics
                }
            }
            else
            {
                totalmedic.Content = "0"; // Default if no records or result is null
            }


            if (Properties.Settings.Default.chart_currency == "$")
            {
                 query = @"
SELECT 
    ISNULL(MAX(sprice), 0) AS MostExpensivePrice
FROM 
    medicinfo;";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["MostExpensivePrice"];
                    if (result != DBNull.Value)
                    {
                        decimal mostExpensivePrice = Convert.ToDecimal(result); // Convert to decimal
                        moste.Content = mostExpensivePrice.ToString("C", CultureInfo.CurrentCulture); // Format as currency
                    }
                    else
                    {
                        moste.Content = "$0.00"; // Default if no data
                    }
                }
                else
                {
                    moste.Content = "$0.00"; // Default if no records or result is null
                }
                

            }
            else
            {
                query = @"
SELECT 
    ISNULL(MAX(sprice), 0) AS MostExpensivePrice
FROM 
    medicinfo;";

                ds = conn.getData(query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["MostExpensivePrice"];
                    if (result != DBNull.Value)
                    {
                        decimal mostExpensivePrice = Convert.ToDecimal(result); // Convert to decimal
                        moste.Content = mostExpensivePrice.ToString("N0", CultureInfo.CurrentCulture); // Format as currency
                    }
                    else
                    {
                        moste.Content = "0"; // Default if no data
                    }
                }
                else
                {
                    moste.Content = "0"; // Default if no records or result is null
                }
            }

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
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }


        private void Button_Clicka(object sender, RoutedEventArgs e)
        {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

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

        private void pdf_Click1(object sender, RoutedEventArgs e)
        {
            medicnum.Visibility = Visibility.Collapsed;
            medicadd1.Visibility = Visibility.Visible;
        }



        private void StartGifAnimation()
        {

            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-1356-wooden-box-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(salesgif1, image1);
            ImageBehavior.SetRepeatBehavior(salesgif1, System.Windows.Media.Animation.RepeatBehavior.Forever);




            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-268-avatar-man-hover-glance.gif"));
            ImageBehavior.SetAnimatedSource(salesgif2, image2);
            ImageBehavior.SetRepeatBehavior(salesgif2, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-16-assessment-hover-assessment.gif"));
            ImageBehavior.SetAnimatedSource(salesgif3, image3);
            ImageBehavior.SetRepeatBehavior(salesgif3, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image4 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-298-coins-hover-jump.gif"));
            ImageBehavior.SetAnimatedSource(salesgif4, image4);
            ImageBehavior.SetRepeatBehavior(salesgif4, System.Windows.Media.Animation.RepeatBehavior.Forever);






            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-39-trash-hover-trash-empty.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage6, image7);
            ImageBehavior.SetRepeatBehavior(accountentimage6, System.Windows.Media.Animation.RepeatBehavior.Forever);



            var image8 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-44-folder-hover-folder.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage7, image8);
            ImageBehavior.SetRepeatBehavior(accountentimage7, System.Windows.Media.Animation.RepeatBehavior.Forever);







        }

        private void Border_Loaded_1(object sender, RoutedEventArgs e)
        {
            fullnameLabel.Content = full;
            StartGifAnimation();
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void supname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                comboBoxText = (sender as ComboBox).Text;
                Console.WriteLine(comboBoxText);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void pdf_Click2(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            AdminDash ad = new AdminDash();
            ad.SetActiveUserControl(medicadd);
            medicnum.Visibility = Visibility.Collapsed;
            medicadd.Visibility = Visibility.Visible;
        }

        private void accountentimage7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pdf_Click(sender, e);
        }

        private void accountentimage6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button_Click(sender, e);
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private async void Button_Clicks(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);

            setting.mainsetting.Visibility = Visibility.Visible;

            setting.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
            fadeOutStoryboard.Begin();
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
