using Org.BouncyCastle.Utilities.Net;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using WpfAnimatedGif;
using Microsoft.Win32; // for SaveFileDialog
using System.Globalization; // for date formatting


namespace OxSystem
{
    /// <summary>
    /// Interaction logic for showaccounts.xaml
    /// </summary>
    public partial class showaccounts : UserControl, INotifyPropertyChanged
    {
        private string comboBoxText;

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
        public static string full = Login_.fullName;


        string query;
        DbConnection conn = new DbConnection();
        DataSet ds;
        DataTable dt;
        public static string dob;
        public static string ad;
        public static string p;
        public static string un;
        public static string fn;
        public static string r;
        public static string n;
        public static string pn;
        public static string em;
        public static string c;
        public static string id;
        public showaccounts()
        {
            InitializeComponent();
            DataContext = this;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            comboBoxText = supnam.Text;
            
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

        private List<int> selectedIds = new List<int>();

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
                string query = $"DELETE FROM users_info WHERE id IN ({idsString})";

                conn.setData(query);

                // Reload the data to reflect changes
                UserControl_Loaded(sender, e);
            }
        }

        private void StartGifAnimation()
        {
            // Run the GIF animation setup on a separate thread

            var image1 = new BitmapImage(new Uri("pack://application:,,,/images/wired-outline-153-bar-chart-hover-diversified.gif"));
            ImageBehavior.SetAnimatedSource(Pharm, image1);
            ImageBehavior.SetRepeatBehavior(Pharm, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image7 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-39-trash-hover-trash-empty.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage6, image7);
            ImageBehavior.SetRepeatBehavior(accountentimage6, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image8 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-44-folder-hover-folder.gif"));
            ImageBehavior.SetAnimatedSource(accountentimage7, image8);
            ImageBehavior.SetRepeatBehavior(accountentimage7, System.Windows.Media.Animation.RepeatBehavior.Forever);
            var image6 = new BitmapImage(new Uri("pack://application:,,,/images/system-regular-18-autorenew-hover-autorenew (2).gif"));
            ImageBehavior.SetAnimatedSource(back, image6);
            ImageBehavior.SetRepeatBehavior(back, System.Windows.Media.Animation.RepeatBehavior.Forever);

        }
        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxText = "Full Name";
            StartGifAnimation();
            Storyboard glowStoryboard = (Storyboard)this.Resources["GlowAnimation"];
            glowStoryboard.Begin();
            query = "select * from users_info";
            ds = conn.getData(query);
            usernum.Content = ds.Tables[0].Rows.Count.ToString();

            query = "select * from users_info where role = 'Admin'";
            ds = conn.getData(query);
            adminnum.Content = ds.Tables[0].Rows.Count.ToString();

            query = "select * from users_info where role ='Pharm'";
            ds = conn.getData(query);
            pharmnum.Content = ds.Tables[0].Rows.Count.ToString(); 
            query = "select * from users_info where role ='Accountent'";
            ds = conn.getData(query);
            accountentnum.Content = ds.Tables[0].Rows.Count.ToString();

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

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (editaccount.Password.Text != "" && editaccount.Address.Text != "" && editaccount.Email.Text != "" && editaccount.PhoneNum.Text != "" && editaccount.full.Text != "" && editaccount.Password.Text != "Password..." && editaccount.Address.Text != "Address..." && editaccount.Email.Text != "Email..." && editaccount.PhoneNum.Text != "7810000000" && editaccount.full.Text != "Fullname..." &&     editaccount.Password.Text != "insert the Password!" && editaccount.Address.Text != "insert the Address!" && editaccount.Email.Text != "insert the Email!" && editaccount.full.Text != "insert the Fullname!")
            {
                editaccount.Password.Foreground = new SolidColorBrush(Colors.Black);
                editaccount.full.Foreground = new SolidColorBrush(Colors.Black);
                editaccount.PhoneNum.Foreground = new SolidColorBrush(Colors.Black);
                editaccount.Address.Foreground = new SolidColorBrush(Colors.Black);
                editaccount.Email.Foreground = new SolidColorBrush(Colors.Black);
                
            }
            editaccount.Username.Text = un;
            editaccount.Password.Text = p;
            editaccount.PhoneNum.Text = pn;
            editaccount.Email.Text = em;
            editaccount.Address.Text = ad;
            editaccount.full.Text = fn;
            editaccount.role.Text = r;



            if (DataGrid.SelectedItem != null)
            {
                var sr = DataGrid.SelectedItem as DataRowView;
                if(sr != null)
                {
                    var fc = sr[3].ToString();
                    passData(fc);
                    UserControl_Loaded(sender,e);
                }
            }
        }
        public async void passData(string fc)
        {
            query = "select * from users_info where id = '" + fc + "'";
            ds = conn.getData(query);
            un= ds.Tables[0].Rows[0][0].ToString();
          
            p = ds.Tables[0].Rows[0][1].ToString();
            
            r = ds.Tables[0].Rows[0][2].ToString();
            
            em = ds.Tables[0].Rows[0][4].ToString();
            
            pn = ds.Tables[0].Rows[0][5].ToString();

            ad = ds.Tables[0].Rows[0][6].ToString();
            
            dob = ds.Tables[0].Rows[0][7].ToString();

            fn = ds.Tables[0].Rows[0][8].ToString();


            editaccount.Username.Text = un;
            editaccount.Password.Text = p;
            editaccount.PhoneNum.Text = pn;
            editaccount.Email.Text = em;
            editaccount.Address.Text = ad;
            editaccount.full.Text = fn;
            editaccount.role.Text = r;
            
            editaccount.editborder.Visibility = Visibility.Collapsed;
            editaccount.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)editaccount.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            editaccount.editborder.Visibility = Visibility.Visible;
            Storyboard moveInStoryboard = (Storyboard)editaccount.FindResource("MoveDownStoryboard");
            moveInStoryboard.Begin();

        }

        private void DataGrid_Selected(object sender, RoutedEventArgs e)
        {
            
        }

        private async void Button_Click1(object sender, RoutedEventArgs e)
        {
           UserControl_Loaded(sender, e);
            Storyboard moveInStoryboard = (Storyboard)editaccount.FindResource("MoveUpStoryboard");
            moveInStoryboard.Begin();
            await Task.Delay(300);
            Storyboard fadeInStoryboard = (Storyboard)editaccount.FindResource("FadeOutStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            editaccount.Visibility = Visibility.Collapsed;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedIds.Clear(); // Clear previous selections

            foreach (var item in DataGrid.SelectedItems)
            {
                var selectedRow = item as DataRowView;
                if (selectedRow != null)
                {
                    int id = Convert.ToInt32(selectedRow["id"]);
                    selectedIds.Add(id);
                }
            }
        }


        private async void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox1.Text == "" || searchBox1.Text == "🔍  Type to search ")
            {
                query = "select * from users_info";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }

            else if ( comboBoxText == "ID")
            {
                query = "select * from users_info where id like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if ( comboBoxText == "Full Name")
            {
                query = "select * from users_info where fullname like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if ( comboBoxText == "Role")
            {
                query = "select * from users_info where role like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if ( comboBoxText == "Email")
            {
                query = "select * from users_info where email like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if ( comboBoxText == "Address")
            {
                query = "select * from users_info where address like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
            else if ( comboBoxText == "Phone Number")
            {
                query = "select * from users_info where phone_num like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }



            else 
            {
              
                query = "select * from users_info where user_name like '" + searchBox1.Text + "%'";
                ds = conn.getData(query);
                await Task.Delay(500);
                DataGrid.ItemsSource = (System.Collections.IEnumerable)ds.Tables[0].DefaultView;
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /*try
            {
                query = "select * from users_info where user_name like '" + searchBox.Text + "%'";
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
            }*/
        }

        private void searchBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "🔍  Type to search " || searchBox1.Text == "")
            {
                searchBox1.Text = "";
                searchBox1.Foreground = new SolidColorBrush(Colors.Black);
                searchBox1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF339FA2"));
            }
            
            
        }

        private void searchBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox1.Text == "")
            {
                searchBox1.Text = "🔍  Type to search ";
                searchBox1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB7B7B7"));
                searchBox1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFABB3B3"));

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
                        BorderThickness = new Thickness(0.2)
                        
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

        private async void reset_Click(object sender, RoutedEventArgs e)
        {
            UserControl_Loaded(sender, e);
            Storyboard moveInStoryboard = (Storyboard)editaccount.FindResource("MoveUpStoryboard");
            moveInStoryboard.Begin();
            await Task.Delay(300);
            Storyboard fadeInStoryboard = (Storyboard)editaccount.FindResource("FadeOutStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            editaccount.Visibility = Visibility.Collapsed;
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            fullnameLabel.Content = full;
        }
       
        private async void pdf_Click1(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            header.Visibility = Visibility.Collapsed;
            glowingBorder.Visibility = Visibility.Collapsed;
            AdminDash ad = new AdminDash();
            ad.SetActiveUserControl(addaccount);
            showacc1.Visibility = Visibility.Collapsed;
            addacc.Visibility = Visibility.Visible;
        }

        private void Button_Clicka(object sender, RoutedEventArgs e)
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

        private async void Button_Clicks(object sender, RoutedEventArgs e)
        { await Task.Delay(200);
            setting.mainsetting.Visibility = Visibility.Visible;

            setting.Visibility = Visibility.Visible;
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard1"];
            fadeOutStoryboard.Begin();
        }

        private void supname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                comboBoxText = (sender as ComboBox).Text;
                Console.WriteLine(comboBoxText);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void searchBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEAEAEA"));
        }

        private void searchBox1_MouseLeave(object sender, MouseEventArgs e)
        {
            searchBox1.Background = new SolidColorBrush(Colors.White);
        }

        private void accountentimage7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pdf_Click(sender,e);
        }

        private void accountentimage6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button_Click(sender,e);
        }

        private void pdf_MouseEnter(object sender, MouseEventArgs e)
        {
         
        }

        private void pdf_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
