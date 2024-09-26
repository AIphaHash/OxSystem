using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : UserControl
    {
        private SqlConnection conn;
        private string connectionString = "your_connection_string_here";
        private DataSet ds;
        private Int64 quantity;
        private Int64 newQuantity;
        private int totalAmount = 0;
        public test()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox8.Text))
            {
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    textBox8.IsEnabled = false;
                    string query = "select quantity from medic where mname = '" + textBox3.Text + "'";
                    ds = fn.getData(query);
                    quantity = Int64.Parse(ds.Tables[0].Rows[0][0].ToString());
                    newQuantity = quantity - Int64.Parse(textBox5.Text);
                    if (newQuantity >= 0)
                    {
                        var newRow = new
                        {
                            MedicName = textBox3.Text,
                            Date = dateTimePicker1.Text,
                            OtherInfo1 = textBox4.Text,
                            Quantity = textBox5.Text,
                            OtherInfo2 = textBox6.Text,
                            BuyerName = textBox8.Text
                        };

                        var dataGridItems = dataGridView1.ItemsSource as List<dynamic> ?? new List<dynamic>();
                        dataGridItems.Add(newRow);
                        dataGridView1.ItemsSource = null;
                        dataGridView1.ItemsSource = dataGridItems;

                        totalAmount += int.Parse(textBox6.Text);
                        label10.Content = totalAmount.ToString();

                        query = "update medic set quantity = '" + newQuantity + "' where mname = '" + textBox3.Text + "'";
                        fn.setData(query, "Medicn Added.");
                        query = "select currentsiz from storage where sname = '" + listBox2.Items[0].ToString() + "'";
                        ds = fn.getData(query);
                        int currentsiz = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                        currentsiz += int.Parse(textBox5.Text);
                        query = "update storage set currentsiz = '" + currentsiz + "' where sname = '" + listBox2.Items[0].ToString() + "'";
                        fn.setData(query, "Storage size updated");
                    }
                    else
                    {
                        MessageBox.Show("Medcine is out of stock \n Only " + quantity + " left", "Warning !!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    clearAll();
                    sellMedcin_Load(this, null);
                }
                else
                {
                    MessageBox.Show("Select medicine first", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Enter buyer name", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }
        private void clearAll()
        {
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox8.Clear();
            textBox8.IsEnabled = true;
        }

        private void sellMedcin_Load(object sender, RoutedEventArgs e)
        {
            // Implement your load logic here
        }

        private void pictureBox8_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    public class fn
    {
        public static DataSet getData(string query)
        {
            // Implement your data retrieval logic here
            return new DataSet();
        }

        public static void setData(string query, string message)
        {
            // Implement your data update logic here
        }
    }
}
