using SciChart.Core.Extensions;
using Syncfusion.Windows.Shared;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace OxSystem
{

    public partial class Login_ : Window
    {
        medic_add ma = new medic_add();
        public static string username;
        public static string finaldbname = "final_proj_db";
        public string password_;
        string query;
        DbConnection conn_ = new DbConnection();
        DataSet ds;
        private Border buttonBorder;
        public static string fullName;
        public static string iduser;

        public Login_()
        {
            InitializeComponent();
            StartOpacityAnimation();
        }



        private void StartTextAnimation(TextBlock textBlock, string textToAnimate)
        {
            textBlock.Text = string.Empty;
            var sb = new StringBuilder();
            var storyboard = new Storyboard();
            var duration = new Duration(TimeSpan.FromMilliseconds(50 * textToAnimate.Length));
            var timeOffset = 0.0;

            foreach (char c in textToAnimate)
            {
                var doubleAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = duration,
                    BeginTime = TimeSpan.FromMilliseconds(timeOffset)
                };

                timeOffset += 20;
                sb.Append(c);

                var stringAnimation = new StringAnimationUsingKeyFrames();
                stringAnimation.KeyFrames.Add(new DiscreteStringKeyFrame(sb.ToString(), KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeOffset))));

                Storyboard.SetTarget(stringAnimation, textBlock);
                Storyboard.SetTargetProperty(stringAnimation, new PropertyPath(TextBlock.TextProperty));
                storyboard.Children.Add(stringAnimation);
            }

            storyboard.Begin();
        }

        private async void mybutton_Click(object sender, RoutedEventArgs e)
        {
           string currentTime = DateTime.Now.ToString("h:mm tt");
            DateTime currentDateOnly = DateTime.Today;

            username = Username.Text;
            password_ = Password.Text;



            query = "select * from users_info where role = 'Admin'";

            ds = conn_.getData(query);

            if (ds.Tables[0].Rows.Count == 0)
            {
                query = "select fullname,id from users_info where user_name = '" + Username.Text + "'";
                ds = conn_.getData(query);
                fullName = ds.Tables[0].Rows[0][0].ToString();
                iduser = ds.Tables[0].Rows[0][1].ToString();
                if (username == "root" && password_ == "root")
                {
                    await Task.Delay(200);
                    AdminDash ad = new AdminDash();
                    ad.Show();
                    this.Close();
                    fullName = "root";
                }
                else if (username != "root" && password_ != "root")
                {


                    query = "select * from users_info where user_name = '" + username + "'  ";
                    ds = conn_.getData(query);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        string role = ds.Tables[0].Rows[0][2].ToString();
                        if (role == "Pharm")
                        {
                            await Task.Delay(200);


                            query = @"UPDATE state
                SET state.state = CASE 
                    WHEN (CONVERT(TIME, shifts.shift_start) < CONVERT(TIME, shifts.shfit_end) AND CONVERT(TIME, '" + currentTime + @"') 
                        BETWEEN CONVERT(TIME, shifts.shift_start) AND CONVERT(TIME, shifts.shfit_end)) 
                    OR (CONVERT(TIME, shifts.shift_start) > CONVERT(TIME, shifts.shfit_end) AND (CONVERT(TIME, '" + currentTime + @"') 
                        >= CONVERT(TIME, shifts.shift_start) OR CONVERT(TIME, '" + currentTime + @"') < CONVERT(TIME, shifts.shfit_end))) 
                    THEN 'upseen'
                    ELSE 'active'
                END
                FROM state
                INNER JOIN users_info ON state.userid = users_info.id
                INNER JOIN shifts ON users_info.id = state.userid
                WHERE users_info.id = '" + iduser + @"' 
                AND shifts.shid = (
                    SELECT TOP 1 shiftid 
                    FROM shifts 
                    WHERE userid = '" + iduser + @"'
                )";
                            conn_.setData(query);

                            query = "select * from statehistroy where userid = '" + iduser + "' and statedate = '" + currentDateOnly + "'";
                            ds = conn_.getData(query);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                query = "insert into statehistroy values ('" + iduser + "' , '" + currentDateOnly + "' , 'upseen')";
                                conn_.setData(query);
                            }

                            pharmacist p = new pharmacist();
                            p.Show();
                            this.Close();
                        }
                        else if (role == "Accountent")
                        {
                            await Task.Delay(200);


                            query = @" UPDATE state
                SET state.state = CASE 
                    WHEN (CONVERT(TIME, shifts.shift_start) < CONVERT(TIME, shifts.shfit_end) AND CONVERT(TIME, '" + currentTime + @"') 
                        BETWEEN CONVERT(TIME, shifts.shift_start) AND CONVERT(TIME, shifts.shfit_end)) 
                    OR (CONVERT(TIME, shifts.shift_start) > CONVERT(TIME, shifts.shfit_end) AND (CONVERT(TIME, '" + currentTime + @"') 
                        >= CONVERT(TIME, shifts.shift_start) OR CONVERT(TIME, '" + currentTime + @"') < CONVERT(TIME, shifts.shfit_end))) 
                    THEN 'upseen'
                    ELSE 'active'
                END
                FROM state
                INNER JOIN users_info ON state.userid = users_info.id
                INNER JOIN shifts ON users_info.id = state.userid
                WHERE users_info.id = '" + iduser + @"' 
                AND shifts.shid = (
                    SELECT TOP 1 shiftid 
                    FROM shifts 
                    WHERE userid = '" + iduser + @"'
                )";
                            conn_.setData(query);

                            query = "select * from statehistroy where userid = '" + iduser + "' and statedate = '" + currentDateOnly + "'";
                            ds = conn_.getData(query);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                query = "insert into statehistroy values ('" + iduser + "' , '" + currentDateOnly + "' , 'upseen')";
                                conn_.setData(query);
                            }

                            Accountent ac = new Accountent();
                            ac.Show();
                            this.Close();
                        }
                    }

                    else
                    {
                        var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                        storyboard.Begin(Username, true);

                        storyboard.Begin(Password, true);
                        animatedTextBlock3.Visibility = Visibility.Collapsed;
                        StartTextAnimation(animatedTextBlock, "Username is Wrong Check your info!");
                        mybutton.IsEnabled = false;
                        await Task.Delay(1000);
                        mybutton.IsEnabled = true;
                    }
                    query = "select * from users_info where password = '" + password_ + "' ";
                    ds = conn_.getData(query);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        string role = ds.Tables[0].Rows[0][2].ToString();
                        if (role == "Pharm")
                        {
                            await Task.Delay(200);


                            query = @"UPDATE state
                SET state.state = CASE 
                    WHEN (CONVERT(TIME, shifts.shift_start) < CONVERT(TIME, shifts.shfit_end) AND CONVERT(TIME, '" + currentTime + @"') 
                        BETWEEN CONVERT(TIME, shifts.shift_start) AND CONVERT(TIME, shifts.shfit_end)) 
                    OR (CONVERT(TIME, shifts.shift_start) > CONVERT(TIME, shifts.shfit_end) AND (CONVERT(TIME, '" + currentTime + @"') 
                        >= CONVERT(TIME, shifts.shift_start) OR CONVERT(TIME, '" + currentTime + @"') < CONVERT(TIME, shifts.shfit_end))) 
                    THEN 'upseen'
                    ELSE 'active'
                END
                FROM state
                INNER JOIN users_info ON state.userid = users_info.id
                INNER JOIN shifts ON users_info.id = state.userid
                WHERE users_info.id = '" + iduser + @"' 
                AND shifts.shid = (
                    SELECT TOP 1 shiftid 
                    FROM shifts 
                    WHERE userid = '" + iduser + @"'
                )";
                            conn_.setData(query);

                            query = "select * from statehistroy where userid = '" + iduser + "' and statedate = '" + currentDateOnly + "'";
                            ds = conn_.getData(query);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                query = "insert into statehistroy values ('" + iduser + "' , '" + currentDateOnly + "' , 'upseen')";
                                conn_.setData(query);
                            }

                            pharmacist p = new pharmacist();
                            p.Show();
                            this.Close();
                        }
                        else if (role == "Accountent")
                        {
                            await Task.Delay(200);

                            query = @"UPDATE state
                SET state.state = CASE 
                    WHEN (CONVERT(TIME, shifts.shift_start) < CONVERT(TIME, shifts.shfit_end) AND CONVERT(TIME, '" + currentTime + @"') 
                        BETWEEN CONVERT(TIME, shifts.shift_start) AND CONVERT(TIME, shifts.shfit_end)) 
                    OR (CONVERT(TIME, shifts.shift_start) > CONVERT(TIME, shifts.shfit_end) AND (CONVERT(TIME, '" + currentTime + @"') 
                        >= CONVERT(TIME, shifts.shift_start) OR CONVERT(TIME, '" + currentTime + @"') < CONVERT(TIME, shifts.shfit_end))) 
                    THEN 'upseen'
                    ELSE 'active'
                END
                FROM state
                INNER JOIN users_info ON state.userid = users_info.id
                INNER JOIN shifts ON users_info.id = state.userid
                WHERE users_info.id = '" + iduser + @"' 
                AND shifts.shid = (
                    SELECT TOP 1 shiftid 
                    FROM shifts 
                    WHERE userid = '" + iduser + @"'
                )";
                            conn_.setData(query);

                            query = "select * from statehistroy where userid = '" + iduser + "' and statedate = '" + currentDateOnly + "'";
                            ds = conn_.getData(query);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                query = "insert into statehistroy values ('" + iduser + "' , '" + currentDateOnly + "' , 'upseen')";
                                conn_.setData(query);
                            }

                            Accountent ac = new Accountent();
                            ac.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                        storyboard.Begin(Username, true);

                        storyboard.Begin(Password, true);
                        animatedTextBlock3.Visibility = Visibility.Collapsed;
                        StartTextAnimation(animatedTextBlock, "Password is Wrong Check your info!");
                        mybutton.IsEnabled = false;
                        await Task.Delay(700);
                        mybutton.IsEnabled = true;
                    }
                }
            }
            else
            {

                query = "select fullname,id from users_info where user_name = '" + Username.Text + "'";
                ds = conn_.getData(query);
                fullName = ds.Tables[0].Rows[0][0].ToString();
                iduser = ds.Tables[0].Rows[0][1].ToString();
                bool usernameExists = false;
                bool passwordCorrect = false;


                query = "SELECT * FROM users_info WHERE user_name = '" + Username.Text + "'";
                ds = conn_.getData(query);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    usernameExists = true;


                    query = "SELECT * FROM users_info WHERE user_name = '" + Username.Text + "' AND password = '" + Password.Text + "'";
                    ds = conn_.getData(query);

                    if (ds.Tables[0].Rows.Count != 0)
                    {

                        string role = ds.Tables[0].Rows[0]["role"].ToString();
                        if (role == "Admin")
                        {
                            await Task.Delay(200);

                            query = @"UPDATE state
                SET state.state = CASE 
                    WHEN (CONVERT(TIME, shifts.shift_start) < CONVERT(TIME, shifts.shfit_end) AND CONVERT(TIME, '" + currentTime + @"') 
                        BETWEEN CONVERT(TIME, shifts.shift_start) AND CONVERT(TIME, shifts.shfit_end)) 
                    OR (CONVERT(TIME, shifts.shift_start) > CONVERT(TIME, shifts.shfit_end) AND (CONVERT(TIME, '" + currentTime + @"') 
                        >= CONVERT(TIME, shifts.shift_start) OR CONVERT(TIME, '" + currentTime + @"') < CONVERT(TIME, shifts.shfit_end))) 
                    THEN 'upseen'
                    ELSE 'active'
                END
                FROM state
                INNER JOIN users_info ON state.userid = users_info.id
                INNER JOIN shifts ON users_info.id = state.userid
                WHERE users_info.id = '" + iduser + @"' 
                AND shifts.shid = (
                    SELECT TOP 1 shiftid 
                    FROM shifts 
                    WHERE userid = '" + iduser + @"'
                )";
                            conn_.setData(query);

                            query = "select * from statehistroy where userid = '" + iduser + "' and statedate = '" + currentDateOnly + "'";
                            ds = conn_.getData(query);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                query = "insert into statehistroy values ('" + iduser + "' , '" + currentDateOnly + "' , 'upseen')";
                                conn_.setData(query);
                            }

                            query = "select fullname,id from users_info where user_name = '" + Username.Text + "'";
                            ds = conn_.getData(query);
                            fullName = ds.Tables[0].Rows[0][0].ToString();
                            iduser = ds.Tables[0].Rows[0][1].ToString();
                            AdminDash ad = new AdminDash();
                            loading l = new loading();
                            ad.Show();
                            this.Close();
                            /* l.Show();
                             await Task.Delay(4000);
                             l.Close();*/
                            // Console.WriteLine(fullName);
                        }
                        else if (role == "Pharm")
                        {
                            await Task.Delay(200);

                            query = @"UPDATE state
                SET state.state = CASE 
                    WHEN (CONVERT(TIME, shifts.shift_start) < CONVERT(TIME, shifts.shfit_end) AND CONVERT(TIME, '" + currentTime + @"') 
                        BETWEEN CONVERT(TIME, shifts.shift_start) AND CONVERT(TIME, shifts.shfit_end)) 
                    OR (CONVERT(TIME, shifts.shift_start) > CONVERT(TIME, shifts.shfit_end) AND (CONVERT(TIME, '" + currentTime + @"') 
                        >= CONVERT(TIME, shifts.shift_start) OR CONVERT(TIME, '" + currentTime + @"') < CONVERT(TIME, shifts.shfit_end))) 
                    THEN 'upseen'
                    ELSE 'active'
                END
                FROM state
                INNER JOIN users_info ON state.userid = users_info.id
                INNER JOIN shifts ON users_info.id = state.userid
                WHERE users_info.id = '" + iduser + @"' 
                AND shifts.shid = (
                    SELECT TOP 1 shiftid 
                    FROM shifts 
                    WHERE userid = '" + iduser + @"'
                )";
                            conn_.setData(query);


                            query = "select * from statehistroy where userid = '" + iduser + "' and statedate = '" + currentDateOnly + "'";
                            ds = conn_.getData(query);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                query = "insert into statehistroy values ('" + iduser + "' , '" + currentDateOnly + "' , 'upseen')";
                                conn_.setData(query);
                            }

                            pharmacist p = new pharmacist();
                            p.Show();
                            this.Close();
                        }
                        else if (role == "Accountent")
                        {
                            await Task.Delay(200);


                            query = @"UPDATE state
                SET state.state = CASE 
                    WHEN (CONVERT(TIME, shifts.shift_start) < CONVERT(TIME, shifts.shfit_end) AND CONVERT(TIME, '" + currentTime + @"') 
                        BETWEEN CONVERT(TIME, shifts.shift_start) AND CONVERT(TIME, shifts.shfit_end)) 
                    OR (CONVERT(TIME, shifts.shift_start) > CONVERT(TIME, shifts.shfit_end) AND (CONVERT(TIME, '" + currentTime + @"') 
                        >= CONVERT(TIME, shifts.shift_start) OR CONVERT(TIME, '" + currentTime + @"') < CONVERT(TIME, shifts.shfit_end))) 
                    THEN 'upseen'
                    ELSE 'active'
                END
                FROM state
                INNER JOIN users_info ON state.userid = users_info.id
                INNER JOIN shifts ON users_info.id = state.userid
                WHERE users_info.id = '" + iduser + @"' 
                AND shifts.shid = (
                    SELECT TOP 1 shiftid 
                    FROM shifts 
                    WHERE userid = '" + iduser + @"'
                )";
                            conn_.setData(query);


                            query = "select * from statehistroy where userid = '"+iduser+"' and statedate = '"+currentDateOnly+"'";
                            ds = conn_.getData(query) ;
                            if (ds.Tables[0].Rows.Count == 0) {
                                query = "insert into statehistroy values ('" + iduser + "' , '" + currentDateOnly + "' , 'upseen')";
                                conn_.setData(query);
                            }
                            

                            query = "select fullname,id from users_info where user_name = '" + Username.Text + "'";
                            ds = conn_.getData(query);
                            fullName = ds.Tables[0].Rows[0][0].ToString();
                            iduser = ds.Tables[0].Rows[0][1].ToString();
                            Accountent ac = new Accountent();
                            ac.Show();
                            this.Close();
                        }
                    }
                }

                if (!usernameExists && !passwordCorrect)
                {
                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(Username, true);
                    Storyboard ShakeLabel = (Storyboard)this.Resources["ShakeLabel"];
                    ShakeLabel.Begin(label1);
                    ShakeLabel.Begin(label2);
                    storyboard.Begin(Password, true);
                    animatedTextBlock3.Visibility = Visibility.Collapsed;
                    StartTextAnimation(animatedTextBlock2, "Error Accourd!");
                    StartTextAnimation(animatedTextBlock, "Username and Password are Wrong, Check your info!");
                    mybutton.IsEnabled = false;
                    await Task.Delay(700);
                    mybutton.IsEnabled = true;
                }
                else if (!usernameExists)
                {
                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    storyboard.Begin(Username, true);
                    Storyboard ShakeLabel = (Storyboard)this.Resources["ShakeLabel"];
                    ShakeLabel.Begin(label1);
                    animatedTextBlock3.Visibility = Visibility.Collapsed;
                    StartTextAnimation(animatedTextBlock2, "Error Accourd!");
                    StartTextAnimation(animatedTextBlock, "Username is Wrong Check your info!");
                    mybutton.IsEnabled = false;
                    await Task.Delay(700);
                    mybutton.IsEnabled = true;
                }
                else if (!passwordCorrect)
                {
                    var storyboard = (Storyboard)this.FindResource("ShakeAndRedBorder");
                    Storyboard ShakeLabel = (Storyboard)this.Resources["ShakeLabel"];
                    ShakeLabel.Begin(label2);
                    storyboard.Begin(Password, true);
                    animatedTextBlock3.Visibility = Visibility.Collapsed;
                    StartTextAnimation(animatedTextBlock2, "Error Accourd!");
                    StartTextAnimation(animatedTextBlock, "Password is Wrong Check your info!");
                    mybutton.IsEnabled = false;
                    await Task.Delay(700);
                    mybutton.IsEnabled = true;
                }
            }
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            Username.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF369198"));
            if (Username.Text == "")
            {
                ma.MoveLabelUp(label1);
            }

        }

        private void Username_LostFocus(object sender, RoutedEventArgs e)
        {
            Username.BorderBrush = new SolidColorBrush(Colors.Black);
            if (Username.Text == "")
            {

                ma.MoveLabelDown(label1);
            }


        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            Password.BorderBrush = new SolidColorBrush(Colors.Black);

            if (Password.Text == "")
            {

                ma.MoveLabelDown(label2);

            }


        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            Password.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF369198"));
            if (Password.Text == "")
            {
                ma.MoveLabelUp(label2);
            }
        }

        private void Username_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Username.IsFocused == false)
            {
                Username.BorderBrush = new SolidColorBrush(Colors.Black);
            }


        }

        private void Username_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Username.IsFocused == false)
            {
                Username.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF636363"));
            }
        }
        private void StartOpacityAnimation()
        {
            Storyboard GlowingAnimation = (Storyboard)FindResource("GlowingAnimation");
            GlowingAnimation.Begin();
        }
        private void Password_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Password.IsFocused == false)
            {
                Password.BorderBrush = new SolidColorBrush(Colors.Black);
            }
        }

        private void Password_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Password.IsFocused == false)
            {
                Password.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF636363"));
            }
        }



        private void mybutton_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                ControlTemplate template = button.Template;
                buttonBorder = template.FindName("buttonBorder", button) as Border;
            }
        }

        private void mybutton_MouseEnter(object sender, MouseEventArgs e)
        {
            mybutton.Foreground = new SolidColorBrush(Colors.Black);
            Button button = sender as Button;
            Border border = (Border)button.Template.FindName("buttonBorder", button);

            if (border != null)
            {
                Storyboard enterStoryboard = (Storyboard)button.Resources["MouseEnterStoryboard"];
                if (enterStoryboard != null)
                {
                    enterStoryboard.Begin();
                }
            }
        }

        private void mybutton_MouseLeave(object sender, MouseEventArgs e)
        {
            mybutton.Foreground = new SolidColorBrush(Colors.White);
            Button button = sender as Button;
            Border border = (Border)button.Template.FindName("buttonBorder", button);
            if (border != null)
            {
                Storyboard leaveStoryboard = (Storyboard)button.Resources["MouseLeaveStoryboard"];
                if (leaveStoryboard != null)
                {
                    leaveStoryboard.Begin();
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartTextAnimation(animatedTextBlock3, "Welcome Back! :)");
        }
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            passwordreset.Visibility = Visibility.Visible;
            Storyboard popUpStoryboard = (Storyboard)FindResource("PopUpStoryboard");
            popUpStoryboard.Begin();
        }

        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Username.Focus() == false)
            {
                Username_GotFocus(sender, e);
                Username.Focus();
            }
        }

        private void label2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Password.Focus() == false)
            {
                password_GotFocus(sender, e);
                Password.Focus();
            }
        }


    }
}
