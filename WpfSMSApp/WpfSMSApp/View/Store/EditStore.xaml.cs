using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls.Dialogs;

namespace WpfSMSApp.View.Store
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditStore : Page
    {
        private int StoreID { get; set; }
        private Model.Store CurrentStore {get;set;}

        public EditStore()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 추가생성자 StoreList에서 storeId를 받아옴
        /// </summary>
        /// <param name="storeId"></param>
        public EditStore(int storeId) : this()
        {
            StoreID = storeId;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;
            TxtStoreID.Text = TxtStoreName.Text = TxtStoreLocation.Text = "";

            try
            {
                // Store테이블에서 내용 읽음
                CurrentStore = Logic.DataAccess.GetStores().Where(s => s.StoreID.Equals(StoreID)).FirstOrDefault();
                TxtStoreID.Text = CurrentStore.StoreID.ToString();
                TxtStoreName.Text = CurrentStore.StoreName;
                TxtStoreLocation.Text = CurrentStore.StoreLocation;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error( $"EditStore.xaml.cs Page_Loaded 예외발생 : {ex}");
                Commons.ShowMessageAsync("예외발생", $"예외발생 : {ex}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

       bool IsValid = true; // 지역변수 -> 전역변수

        public bool IsValidInput()
        {
            if (string.IsNullOrEmpty(TxtStoreName.Text))
            {
                LblStoreName.Visibility = Visibility.Visible;
                LblStoreName.Text = "창고명을 입력하세요";
                IsValid = false;
            }
            else
            {
                var cnt = Logic.DataAccess.GetStores().Where(u => u.StoreName.Equals(TxtStoreName.Text)).Count();
                if (cnt > 0)
                {
                    LblStoreName.Visibility = Visibility.Visible;
                    LblStoreName.Text = "이미 있는 창고명입니다";
                    IsValid = false;
                }
            }
            if (string.IsNullOrEmpty(TxtStoreLocation.Text))
            {
                LblStoreLocation.Visibility = Visibility.Visible;
                LblStoreLocation.Text = "창고위치를 입력하세요";
                IsValid = false;
            }
            return IsValid;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true; // 입력된 값이 모두 만족하는지 판별하는 플래그
            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;

            IsValid = IsValidInput(); // 유효성 체크(필수)

            if (IsValid)
            {
                //MessageBox.Show("DB 입력처리!");
                CurrentStore.StoreName = TxtStoreName.Text;
                CurrentStore.StoreLocation = TxtStoreLocation.Text;

                try
                {
                    var result = Logic.DataAccess.SetStore(CurrentStore);
                    if (result == 0)
                    {
                        // 수정 안됨
                        Commons.LOGGER.Error("AddStore.xaml.cs 창고정보 수정오류 발생");
                        Commons.ShowMessageAsync("오류", "저장시 오류가 발생했습니다");
                    }
                    else
                    {
                        NavigationService.Navigate(new StoreList());
                    }
                }
                catch (Exception ex)
                {
                    Commons.LOGGER.Error($"예외발생 : {ex}");
                }
            }
        }

        private void TxtStoreName_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }

        private void TxtStoreLocation_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }

        private void BtnEditStore_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
