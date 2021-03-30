using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfSMSApp.View
{
    /// <summary>
    /// LoginView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginView : MetroWindow
    {
        public LoginView()
        {
            InitializeComponent();
            Commons.LOGGER.Info("LoginView 초기화"); 
        }

        private async void BtnCancel_Click(object sender, RoutedEventArgs e) // mahapp ShowMessageAsync 쓰려고 이 구조로 표시 / 그냥 메세지박스는 async 필요x
        {
            var result = await this.ShowMessageAsync("종료", "프로그램 종료할까요?",  // async와 await는 쌍으로 적어줘야됨
                MessageDialogStyle.AffirmativeAndNegative, null);

            if (result == MessageDialogResult.Affirmative)
            {
                Commons.LOGGER.Info("프로그램 종료");
                Application.Current.Shutdown(); // 프로그램 종료
            }
                
            
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtUserEmail.Focus();

            LblReslt.Visibility = Visibility.Hidden;
        }

        private void TxtUserEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TxtPassword.Focus();
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnLogin_Click(sender, e); // 로그인버튼 클릭
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LblReslt.Visibility = Visibility.Hidden; // 결과 레이블 숨김

            if (string.IsNullOrEmpty(TxtUserEmail.Text) || string.IsNullOrEmpty(TxtPassword.Password))
            {
                LblReslt.Visibility = Visibility.Visible;
                LblReslt.Content = "아이디나 패스워드를 입력하세요.";
                Commons.LOGGER.Warn("아이디/패스워드 미입력 접속시도");
                return;
            }
            try
            {
                var email = TxtUserEmail.Text;
                var password = TxtPassword.Password;

                var mdHash = MD5.Create();
                password = Commons.GetMd5Hash(mdHash, password);

                var isOurUser = Logic.DataAcess.GetUsers()
                    .Where(u => u.UserEmail.Equals(email) && u.UserPassword.Equals(password) 
                                && u.UserActivated == true).Count();

                if (isOurUser == 0)
                {
                    LblReslt.Visibility = Visibility.Visible;
                    LblReslt.Content = "아이디나 패스워드가 일치하지 않습니다.";
                    Commons.LOGGER.Warn("아이디/패스워드 불일치");
                    return;
                }
                else
                {
                    Commons.LOGINED_USER = Logic.DataAcess.GetUsers().Where(u => u.UserEmail.Equals(email)).FirstOrDefault();
                    Commons.LOGGER.Info($"{email} 접속성공");
                    this.Visibility = Visibility.Hidden;
                }
                  
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
                await this.ShowMessageAsync("예외", $"예외발생 {ex}");
            }
        }
    }
}
