using HtmlAgilityPack;
using iText.IO.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
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

namespace OxSystem
{
    /// <summary>
    /// Interaction logic for setting.xaml
    /// </summary>
    public partial class setting : UserControl
    {
        string checkuncheckmoenycurrency;
        decimal currencybeforesave;
        string checkuncheckchartcurrency;
        string query;
        DataSet ds;
        DbConnection conn = new DbConnection();

        public static string currencyies;
        private decimal defaultCurrency;
        public static string check_currency;
        public setting()
        {
            InitializeComponent();
        }

        private async void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await Task.Delay(200);
            if (sender is Label label && label.Tag is string tag)
            {
                // Hide all grids initially (fade out)
                HideGridWithFadeOut(GeneralGrid);
                HideGridWithFadeOut(ChartsGrid);
                HideGridWithFadeOut(CurrencyGrid);
                HideGridWithFadeOut(ShortcutsGrid);
                HideGridWithFadeOut(NotificationsGrid);
                HideGridWithFadeOut(ChatGrid);
                HideGridWithFadeOut(PersonalGrid);

                // Show the grid corresponding to the clicked label (fade in)
                switch (tag)
                {
                    case "General":
                        ShowGridWithFadeIn(GeneralGrid);
                        break;
                    case "Charts":
                        ShowGridWithFadeIn(ChartsGrid);
                        break;
                    case "Currency":
                        ShowGridWithFadeIn(CurrencyGrid);
                        break;
                    case "Shortcuts":
                        ShowGridWithFadeIn(ShortcutsGrid);
                        break;
                    case "Notifications":
                        ShowGridWithFadeIn(NotificationsGrid);
                        break;
                    case "Chat":
                        ShowGridWithFadeIn(ChatGrid);
                        break;
                    case "Personal":
                        ShowGridWithFadeIn(PersonalGrid);
                        break;
                }
            }
        }

        // Helper method to fade out a grid
        private void HideGridWithFadeOut(UIElement grid)
        {
            if (grid.Visibility == Visibility.Visible)
            {
                Storyboard fadeOutStoryboard = (Storyboard)FindResource("FadeOutStoryboard3");

                // Cast grid to FrameworkElement for the Begin method
                fadeOutStoryboard.Completed += (s, e) => grid.Visibility = Visibility.Collapsed;
                fadeOutStoryboard.Begin(grid as FrameworkElement);
            }
        }

        // Helper method to fade in a grid
        private void ShowGridWithFadeIn(UIElement grid)
        {
            grid.Visibility = Visibility.Visible;

            // Cast grid to FrameworkElement for the Begin method
            Storyboard fadeInStoryboard = (Storyboard)FindResource("FadeInStoryboard3");
            fadeInStoryboard.Begin(grid as FrameworkElement);
        }



        private void SwitchButton_Checked1(object sender, RoutedEventArgs e)
        {
            currency_Copy.Visibility = Visibility.Visible;
            currency.Visibility = Visibility.Collapsed;
            checkuncheckmoenycurrency = "$";

           
        }

        private void SwitchButton_Unchecked1(object sender, RoutedEventArgs e)
        {
            currency.Visibility = Visibility.Visible;
            currency_Copy.Visibility = Visibility.Collapsed;
            checkuncheckmoenycurrency = "IQD";
        }

        private void ExchangeRateLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            // Ensure that only valid formatting occurs
            if (textBox != null)
            {
                string text = textBox.Text.Replace(",", "");

                // Validate the length and starting digit
                if (text.Length > 4)
                {
                    text = text.Substring(0, 4);
                }

                if (text.Length < 4)
                {
                    text = text.PadLeft(4, '0');
                }

                if (text[0] != '1')
                {
                    text = "1" + text.Substring(1);
                }

                // Convert to decimal and format with commas
                if (decimal.TryParse(text, out decimal value))
                {
                    textBox.Text = string.Format("{0:N0}", value);
                    textBox.SelectionStart = textBox.Text.Length; // Move caret to end
                }
            }
        }


        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void UpdateUIWithDefaultCurrency()
        {
        

        }

        private void SetDefaultCurrencyDisplay()
        {
        }
        private async Task LoadExchangeRate() // Change return type to Task
        {
            string url = "https://www.xe.com/currencyconverter/convert/?Amount=1&From=USD&To=IQD";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Fetch the HTML content
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    // Load the HTML into HtmlAgilityPack
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    // Parse the exchange rate value
                    var rateNode = htmlDocument.DocumentNode.SelectSingleNode("//p[contains(@class, 'sc-e08d6cef-1')]");

                    if (rateNode != null)
                    {
                        // Extract the integer and decimal parts
                        string integerPart = rateNode.SelectSingleNode(".//text()").InnerText.Trim();
                        string decimalPart = rateNode.SelectSingleNode(".//span[@class='faded-digits']").InnerText.Trim();

                        // Remove commas and concatenate parts
                        string exchangeRateText = integerPart.Replace(",", "") + decimalPart;

                        // Convert to decimal and then to integer
                        if (decimal.TryParse(exchangeRateText, out decimal exchangeRate))
                        {
                            // Convert to integer to remove decimal part
                            defaultCurrency = Math.Round(exchangeRate, MidpointRounding.AwayFromZero);

                            currencybeforesave = defaultCurrency;
                            // Store the value in settings
                           /* Properties.Settings.Default.currency = defaultCurrency;
                            Properties.Settings.Default.Save();*/ // Save the settings

                            // Update the UI with the default currency
                            UpdateUIWithDefaultCurrency();
                        }
                        else
                        {
                            // Set default value if parsing fails
                            SetDefaultCurrencyDisplay();
                        }
                    }
                    else
                    {
                        // Set default value if rateNode is null
                        SetDefaultCurrencyDisplay();
                    }
                }
                catch (HttpRequestException e)
                {
                    // Handle HTTP request exceptions and set default values
                    SetDefaultCurrencyDisplay();
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions and set default values
                    SetDefaultCurrencyDisplay();
                }
            }
        }

       

        private async void Button_Clic(object sender, RoutedEventArgs e)
        {
            await LoadExchangeRate();
            ExchangeRateLabel.Text = currencybeforesave.ToString();
            Properties.Settings.Default.currency = currencybeforesave;
            Properties.Settings.Default.Save();


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           


            Username.Text = Properties.Settings.Default.ex1;
            Username1.Text = Properties.Settings.Default.ex2;
            Username2.Text = Properties.Settings.Default.ex3;
            Username3.Text = Properties.Settings.Default.ex4;
            Username5.Text = Properties.Settings.Default.st1;
            Username6.Text = Properties.Settings.Default.st2;
            Username7.Text = Properties.Settings.Default.st3;
            Username8.Text = Properties.Settings.Default.st4;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            Storyboard.SetTargetName(fadeOutStoryboard.Children[0], "mainsetting");
            fadeOutStoryboard.Begin(this);

            await Task.Delay(1100);

            // Remove the animation
            mainsetting.BeginAnimation(UIElement.OpacityProperty, null);

            // Reset Opacity and hide the control
            mainsetting.Opacity = 1;
            mainsetting.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.setting_visible = "1";
            Properties.Settings.Default.Save();


            // Should print 1 now
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ex1 = Username.Text;
            Properties.Settings.Default.ex2 = Username1.Text;
            Properties.Settings.Default.ex3 = Username2.Text;
            Properties.Settings.Default.ex4 = Username3.Text;
            Properties.Settings.Default.st1 = Username5.Text;
            Properties.Settings.Default.st2 = Username6.Text;
            Properties.Settings.Default.st3 = Username7.Text;
            Properties.Settings.Default.st4 = Username8.Text;
            Properties.Settings.Default.Save();



            pharmacist p = new pharmacist();
            p.background_Loaded(sender, e);


        }

        private void SwitchButton_Checked11(object sender, RoutedEventArgs e)
        {
            currency_Copy1.Visibility = Visibility.Visible;
            currency1.Visibility = Visibility.Collapsed;
            checkuncheckchartcurrency = "$";

        }

        private void SwitchButton_Unchecked11(object sender, RoutedEventArgs e)
        {
            currency1.Visibility = Visibility.Visible;
            currency_Copy1.Visibility = Visibility.Collapsed;
            checkuncheckchartcurrency = "IQD";

        }

        private void SwitchButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {

            query = "select * from users_info where id = '" + Login_.iduser + "'";
            ds = conn.getData(query);
            editaccount.Username.Text = ds.Tables[0].Rows[0][0].ToString();
            Console.WriteLine(editaccount.Username.Text);
            editaccount.Password.Text = ds.Tables[0].Rows[0][1].ToString();
            editaccount.PhoneNum.Text = ds.Tables[0].Rows[0][5].ToString();
            editaccount.Email.Text = ds.Tables[0].Rows[0][4].ToString();
            editaccount.Address.Text = ds.Tables[0].Rows[0][6].ToString();
            editaccount.full.Text = ds.Tables[0].Rows[0][8].ToString();
            editaccount.role.Text = ds.Tables[0].Rows[0][2].ToString();

            await Task.Delay(1000);
            editaccount.editborder.Visibility = Visibility.Collapsed;
            editaccount.Visibility = Visibility.Visible;
            Storyboard fadeInStoryboard = (Storyboard)editaccount.FindResource("FadeInStoryboard");
            fadeInStoryboard.Begin();
            await Task.Delay(300);
            editaccount.editborder.Visibility = Visibility.Visible;
            Storyboard moveInStoryboard = (Storyboard)editaccount.FindResource("MoveDownStoryboard");
            moveInStoryboard.Begin();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.chart_currency = checkuncheckchartcurrency;
            Properties.Settings.Default.Save();
            Console.WriteLine(Properties.Settings.Default.chart_currency);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var textBox = ExchangeRateLabel;
            string text = textBox.Text.Replace(",", "");

            if (decimal.TryParse(text, out decimal value))
            {
                // Save the value to settings
                Properties.Settings.Default.currency = value;
                Properties.Settings.Default.Save(); // Save the settings
                Properties.Settings.Default.check_currency = checkuncheckmoenycurrency;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a value starting with 1 and consisting of 4 digits.");
            }
        }

        private void TextBlock_Loaded_1(object sender, RoutedEventArgs e)
        {
            ExchangeRateLabel.Text = Properties.Settings.Default.currency.ToString();
            if (Properties.Settings.Default.check_currency == "$")
            {
                SwitchButton_Copy.IsChecked = true;
                SwitchButton_Checked1(sender, e);
            }
            else
            {
                SwitchButton_Unchecked1(sender, e);
            }
        }

        private void ExchangeRateLabel_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);

        }

        private void TextBlock_Loaded_2(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.chart_currency == "$")
            {
                SwitchButton_Copy1.IsChecked = true;
                SwitchButton_Checked11(sender, e);
            }
            else { SwitchButton_Unchecked11(sender, e);
            }

        }
    }
}
