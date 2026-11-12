using System;
using System.Collections.Generic;
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

namespace Baev_Shoes41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string generatedCaptcha = "";
        private bool iscaptchaActive = false;
        public AuthPage()
        {
            InitializeComponent();
        }

        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char c1 = chars[random.Next(chars.Length)];
            char c2=chars[random.Next(chars.Length)];
            char c3=chars[random.Next(chars.Length)];
            char c4=chars[random.Next(chars.Length)];

            generatedCaptcha = $"{c1}{c2}{c3}{c4}";
            
            captchaOneWord.Text=c1.ToString();
            captchaTwoWord.Text=c2.ToString();
            captchaThreeWord.Text=c3.ToString();
            captchaFourWord.Text=c4.ToString();

            CaptchaPanel.Visibility = Visibility.Visible;
            CaptchaInputTB.Visibility = Visibility.Visible;
            CaptchaInputTB.Clear();
            iscaptchaActive = true;

        }

        private void LogGHBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
            LoginTB.Text = "";
            PasswordTB.Text = "";
            CaptchaPanel.Visibility = Visibility.Hidden;
            CaptchaInputTB.Visibility = Visibility.Hidden;
            CaptchaInputTB.Clear();
            iscaptchaActive = false;
        }

        private async void LogUsBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PasswordTB.Text;
            if (iscaptchaActive)
            {
                if (CaptchaInputTB.Text.Trim() != generatedCaptcha)
                {
                    MessageBox.Show("Капча введена неверно!", "Повторите вход через 10 секунд..");
                    LogUsBtn.IsEnabled = false;
                    //таймер на 10 сек
                    await Task.Delay(10000);
                    LogUsBtn.IsEnabled = true;

                    GenerateCaptcha();
                    return;
                }
            }
            if (login=="" || password=="")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user = Baev_Shoes41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user!=null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                CaptchaPanel.Visibility = Visibility.Hidden;
                CaptchaInputTB.Visibility = Visibility.Hidden;
                CaptchaInputTB.Clear();
                LoginTB.Text = "";
                PasswordTB.Text = "";
                iscaptchaActive = false;
            }
            else
            {
                if (!iscaptchaActive && LoginTB.Text!="" && PasswordTB.Text!="")
                {
                    MessageBox.Show("Неверные данные, введите капчу");
                    GenerateCaptcha();
                }
                else
                {
                    MessageBox.Show("Введены неверные данные");
                    LogUsBtn.IsEnabled = false;
                    //таймер на 10 сек
                    await Task.Delay(10000);
                    LogUsBtn.IsEnabled = true;
                    GenerateCaptcha();
                }
            }
        }

        private void PasswordTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoginTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
