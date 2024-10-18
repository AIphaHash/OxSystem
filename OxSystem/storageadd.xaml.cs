using LiveCharts.Maps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using WpfAnimatedGif;
using static OxSystem.main_background;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for storageadd.xaml
    /// </summary>
    /// 
    public class StorageMedicData
    {
        public string Sname { get; set; }
        public int MedicCount { get; set; }
    }
    public partial class storageadd : UserControl
    {


        medic_add ma = new medic_add();
        private string _currentTime;
        private DispatcherTimer _timer;
        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        private DispatcherTimer _timer1;
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

        public static string sn;
        public static string sl;
        public static string ss;
        public static string id;
        public storageadd()
        {
            InitializeComponent();
            LoadStorageChartData();
            LoadChartData();
            DataContext = this;
            _timer1 = new DispatcherTimer();
            _timer1.Interval = TimeSpan.FromSeconds(1);
            _timer1.Tick += Timer_Tick;
            _timer1.Start();

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
        private List<int> selectedIds = new List<int>();

        private void LoadStorageChartData()
        {

            List<StorageMedicData> storageData = GetStorageMedicData1();
            storageChart.DataContext = storageData; // Bind data to the chart
        }

        public List<StorageMedicData> GetStorageMedicData1()
        {
            List<StorageMedicData> dataList = new List<StorageMedicData>();
            string query = @"
            SELECT sname, size
FROM storageinfo;
";

            DataSet ds = conn.getData(query); // Assuming getData returns a DataSet
            DataTable dt = ds.Tables[0]; // Getting the first DataTable from DataSet

            foreach (DataRow row in dt.Rows)
            {
                dataList.Add(new StorageMedicData
                {
                    Sname = row["sname"].ToString(),
                    MedicCount = int.Parse(row["size"].ToString())
                });
            }

            return dataList;
        }
        public List<StorageMedicData> GetStorageMedicData()
        {
            List<StorageMedicData> dataList = new List<StorageMedicData>();
            string query = @"
        SELECT si.sname, SUM(mi.nummedic) AS MedicCount
        FROM storageinfo si
        INNER JOIN medicinfo mi ON si.sname = mi.sname
        GROUP BY si.sname";

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
        private void mybutton_Click(object sender, RoutedEventArgs e)
        {
            string sn = sname.Text;
            string sl = slocation.Text;
            string ss = size.Text;
            
            if (sname.Text != "" & slocation.Text != "" & size.Text != "" & sname.Text != "Storage Name..." & sname.Text != "insert the Storage Name!" & slocation.Text != "Storage Location..." & slocation.Text != "insert the Storage Location!" & size.Text != "9999")
            {

                query = "insert into storageinfo values('" + sn + "' , '" + sl + "' , '" + ss + "');";
                conn.setData(query);

               
            }
            if (sname.Text == "")
            {

                var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                storyboard.Begin(sname, true);
                Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                shakeStoryboard.Begin(label1);

            }
            if (slocation.Text == "")
            {
                var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                storyboard.Begin(slocation, true);
                Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                shakeStoryboard.Begin(label2);
            }
            if (size.Text == "")
            {
                var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                storyboard.Begin(size, true);
                Storyboard shakeStoryboard = (Storyboard)this.Resources["ShakeStoryboard"];
                shakeStoryboard.Begin(label3);
            }
            else
            {
                

            }
            UserControl_Loaded(sender, e);
            reset_();
        }
        private void reset_()
        {
            sname.Clear();
            slocation.Clear();
            size.Clear();
            

        }

        private void searchBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "🔍  Type the Storage Name to search ")
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
                searchBox1.Text = "🔍  Type the Storage Name to search ";
                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
            }
        }

        private async void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            query = "select * from storageinfo where sname like '" + searchBox1.Text + "%'";
            ds = conn.getData(query);
            await Task.Delay(500);
            DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedIds.Clear(); // Clear previous selections

            foreach (var item in DataGrid.SelectedItems)
            {
                var selectedRow = item as DataRowView;
                if (selectedRow != null)
                {
                    int id = Convert.ToInt32(selectedRow["sid"]);
                    selectedIds.Add(id);
                }
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editstorage.sname.Text = sn;
            editstorage.slocation.Text = sl;
            editstorage.size.Text = ss;


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
        public async void passData(string fc)
        {
            query = "select * from storageinfo where sid = '" + fc + "'";
            ds = conn.getData(query);
            sn = ds.Tables[0].Rows[0][1].ToString();

            sl = ds.Tables[0].Rows[0][2].ToString();

            ss = ds.Tables[0].Rows[0][3].ToString();

            


            editstorage.sname.Text = sn;
            editstorage.slocation.Text = sl;
            editstorage.size.Text = ss;
            

            editstorage.editborder.Visibility = Visibility.Collapsed;
            editstorage.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)editstorage.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            editstorage.editborder.Visibility = Visibility.Visible;
            Storyboard moveInStoryboard = (Storyboard)editstorage.FindResource("MoveDownStoryboard");
            moveInStoryboard.Begin();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStorageChartData();
            query = "select * from storageinfo";
            ds = conn.getData(query);
            snum.Content = ds.Tables[0].Rows.Count;
             query = "select * from medicinfo";
            ds = conn.getData(query);
            mnum.Content = ds.Tables[0].Rows.Count;

            query = "SELECT MAX(size) AS LargestSize FROM storageinfo;\r\n";
            ds = conn.getData(query);
            larges.Content = ds.Tables[0].Rows[0][0];
            query = "SELECT MIN(size) AS SmallestSize FROM storageinfo;\r\n";
            ds = conn.getData(query);
            smalls.Content = ds.Tables[0].Rows[0][0];










            fullnameLabel.Content = full;
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
            try
            {
                query = "select * from storageinfo";
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
                string query = $"DELETE FROM storageinfo WHERE sid IN ({idsString})";

                conn.setData(query);

                // Reload the data to reflect changes
                UserControl_Loaded(sender, e);
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

        private void sname_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void size_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void slocation_TextChanged(object sender, TextChangedEventArgs e)
        {
          
        }

        private void sname_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sname.IsFocused)
            {

            }
            else
            {
                sname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF777777"));
            }
            

        }

        private void sname_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sname.IsFocused)
            {

            }
            else
            {
                sname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4E4E4E"));
            }
           

        }

        private void size_MouseEnter(object sender, MouseEventArgs e)
        {
            if (size.IsFocused)
            {

            }
            else
            {
                size.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4E4E4E"));
            }

        }

        private void size_MouseLeave(object sender, MouseEventArgs e)
        {
            if (size.IsFocused)
            {

            }
            else
            {
                size.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF777777"));
            }

        }

        private void slocation_MouseEnter(object sender, MouseEventArgs e)
        {
            if (slocation.IsFocused)
            {

            }
            else
            {
                slocation.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4E4E4E"));
            }

        }

        private void slocation_MouseLeave(object sender, MouseEventArgs e)
        {
            if (slocation.IsFocused)
            {

            }
            else
            {
                slocation.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF777777"));
            }

        }

        private void sname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sname.Text == "")
            {

               MoveLabelDown(label1);
                sname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF777777"));

            }

        }

        private void size_LostFocus(object sender, RoutedEventArgs e)
        {
            if (size.Text == "")
            {

                MoveLabelDown(label3);
                size.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF777777"));

            }

        }

        private void slocation_LostFocus(object sender, RoutedEventArgs e)
        {
            if (slocation.Text == "")
            {

              MoveLabelDown(label2);
                slocation.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF777777"));
            }

        }

        private void slocation_GotFocus(object sender, RoutedEventArgs e)
        {
            if (slocation.Text == "")
            {
                MoveLabelUp(label2);
                slocation.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF409892"));

            }
        }

        private void size_GotFocus(object sender, RoutedEventArgs e)
        {
            if (size.Text == "")
            {
               MoveLabelUp(label3);
                size.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF409892"));

            }
        }

        private void sname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sname.Text == "")
            {
                MoveLabelUp(label1);
                sname.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF409892")); 
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {

        }

       

      

      

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
        public void MoveLabelUp(Label label)
        {
            // Create a storyboard
            Storyboard storyboard = new Storyboard();

            // Create a DoubleAnimation to move the label up by 34 units
            DoubleAnimation moveUpAnimation = new DoubleAnimation
            {
                From = 0, // Start from current position
                To = -28, // Move up by 34 units (negative value moves upward)
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
                From = -22,
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
        private void StartGifAnimation()
        {

            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-428-injection-hover-pinch.gif"));
            ImageBehavior.SetAnimatedSource(salesgif1, image1);
            ImageBehavior.SetRepeatBehavior(salesgif1, System.Windows.Media.Animation.RepeatBehavior.Forever);




            var image2 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-41-home-morph-home-2.gif"));
            ImageBehavior.SetAnimatedSource(salesgif, image2);
            ImageBehavior.SetRepeatBehavior(salesgif, System.Windows.Media.Animation.RepeatBehavior.Forever);





            var image3 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-159-trending-down-hover-trend-down.gif"));
            ImageBehavior.SetAnimatedSource(salesgif2, image3);
            ImageBehavior.SetRepeatBehavior(salesgif2, System.Windows.Media.Animation.RepeatBehavior.Forever); 
            var image4 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-160-trending-up-hover-trend-up.gif"));
            ImageBehavior.SetAnimatedSource(salesgif3, image4);
            ImageBehavior.SetRepeatBehavior(salesgif3, System.Windows.Media.Animation.RepeatBehavior.Forever);


            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-39-trash-hover-trash-empty.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage6, image7);
            ImageBehavior.SetRepeatBehavior(accountentimage6, System.Windows.Media.Animation.RepeatBehavior.Forever);



            var image8 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-44-folder-hover-folder.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage7, image8);
            ImageBehavior.SetRepeatBehavior(accountentimage7, System.Windows.Media.Animation.RepeatBehavior.Forever);

            var image5 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (2).gif"));
            ImageBehavior.SetAnimatedSource(back, image5);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);






        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            StartGifAnimation();
        }

        private void pdf_Click2(object sender, RoutedEventArgs e)
        {

        }

        private async void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserControl_Loaded(sender, e);
            
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

        private void bprice_Copy1_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void bprice_Copy1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (bprice_Copy1.Text == "" || bprice_Copy1.Text == "Search Storage Name...")
            {
                bprice_Copy1.Text = "";
                DataGrid_Loaded(sender, e);
            }
        }

        private void bprice_Copy1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (bprice_Copy1.Text == "" || bprice_Copy1.Text == "Search Storage Name...")
            {
                bprice_Copy1.Text = "Search Storage Name...";
                DataGrid_Loaded(sender, e);
            }
        }

        private async void bprice_Copy1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bprice_Copy1.Text == "Search Storage Name..." || bprice_Copy1.Text == "")
            {
                DataGrid_Loaded(sender, e);

            }
            
               
                else
                {
                    await Task.Delay(300);
                    string searchText = bprice_Copy1.Text.Trim(); // Get the text from the search box and trim any extra spaces

                   query = "select * from storageinfo where sname like '%"+bprice_Copy1.Text+"%'";
               

                    ds = conn.getData(query);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                
            }
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            query = "select * from storageinfo";
            ds = conn.getData(query);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataGrid.ItemsSource = ds.Tables[0].DefaultView;
            }
        }

        private async void delete_Copy_Click(object sender, RoutedEventArgs e)
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
                string query = $"DELETE FROM storageinfo WHERE sid IN ({idsString})";

                conn.setData(query);

                // Reload the data to reflect changes
                UserControl_Loaded(sender, e);
            }
        }

        private void accountentimage6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            delete_Copy_Click(sender, e);
        }

        private void accountentimage7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pdf_Click(sender, e);
        }

        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sname.Focus() == false)
            {
                sname_GotFocus(sender, e);
                sname.Focus();
            }
        }

        private void label2_GotFocus(object sender, RoutedEventArgs e)
        {
            if (slocation.Focus() == false)
            {
                slocation_GotFocus(sender, e);
                slocation.Focus();
            }
        }

        private void label3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (size.Focus() == false)
            {
                size_GotFocus(sender, e);
                size.Focus();
            }
        }

        private void label2_GotFocus(object sender, MouseButtonEventArgs e)
        {
            if (slocation.Focus() == false)
            {
                slocation_GotFocus(sender, e);
                slocation.Focus();
            }
        }
    }
}
