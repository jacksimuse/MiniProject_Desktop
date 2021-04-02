using MahApps.Metro.Controls;
using NaverMovieFinderApp.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace NaverMovieFinderApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            StsResult.Content = "";

            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                StsResult.Content = "검색할 영화명을 입력 후, 검색버튼을 눌러주세요";
                return;
            }

            //Commons.ShowMessageAsync("결과", $"{TxtMovieName.Text}");
            try
            {
                ProcSearchNaverApi(TxtMovieName.Text);
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.Logger.Error($"예외발생 : {ex}");
            }
            
        }

        private void ProcSearchNaverApi(string movieName)
        {
            string clientID = "ZrW5YejTfOktXlA19v1H";
            string clientSecret = "qX4dunM8w8";
            string openApiUrl = $"https://openapi.naver.com/v1/search/movie?start=1&display=30&query={movieName}";

            string resJson = Commons.GetOpenApiResult(openApiUrl, clientID, clientSecret);
            var parsedJson = JObject.Parse(resJson);

            int total = Convert.ToInt32(parsedJson["total"]);
            int display = Convert.ToInt32(parsedJson["display"]);

            StsResult.Content = $"{total} 중 {display} 호출 성공";

            var items = parsedJson["items"];
            var json_array = (JArray)items;

            List<Movieitem> movieItems = new List<Movieitem>();

            foreach (var item in json_array)
            {
                Movieitem movie = new Movieitem(
                    Commons.StripHtmlTag(item["title"].ToString()),
                    item["link"].ToString(),
                    item["image"].ToString(),
                    item["subtitle"].ToString(),
                    item["pubDate"].ToString(),
                    Commons.StripPipe(item["director"].ToString()),
                    Commons.StripPipe(item["actor"].ToString()),
                    item["userRating"].ToString() );
                movieItems.Add(movie);
            }

            this.DataContext = movieItems;
        }

        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnSearch_Click(sender, e);
        }

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (GrdData.SelectedItem == null)
            {
            }

            if (GrdData.SelectedItem is Movieitem)
            {
                var movie = GrdData.SelectedItem as Movieitem;
                //Commons.ShowMessageAsync("결과", $"{movie.Image}");
                if (string.IsNullOrEmpty(movie.Image))
                {
                    ImgPoster.Source = new BitmapImage(new Uri("No_Picutre.jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImgPoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void BtnAddWatchList_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택하세요(복수선택 가능)");
                return;
            }

            List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();

            foreach (Movieitem item in GrdData.SelectedItems)
            {
                NaverFavoriteMovies temp = new NaverFavoriteMovies()
                {
                    Title = item.Title,
                    Link = item.Link,
                    Image = item.Image,
                    SubTitle = item.SubTitle,
                    PubDate = item.PubDate,
                    Director = item.Director,
                    Actor = item.Actor,
                    UserRating = item.UserRating,
                    RegDate = DateTime.Now
                };

                list.Add(temp);
            }

            try
            {
                using (var ctx = new OpenApiLabEntities())
                {
                    ctx.Set<NaverFavoriteMovies>().AddRange(list);
                    ctx.SaveChanges();
                }

                Commons.ShowMessageAsync("저장", "즐겨찾기 추가");
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.Logger.Error($"예외발생 : {ex}");

            }

        }

        private void BtnViewWatchList_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtMovieName.Text = "";

            List<Movieitem> listData = new List<Movieitem>();
            List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();


            try
            {
                using (var ctx = new OpenApiLabEntities())
                {
                    list = ctx.NaverFavoriteMovies.ToList();
                }
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.Logger.Error($"예외발생 : {ex}");
            }

            foreach (var item in list)
            {
                listData.Add(new Movieitem(
                    item.Title,
                    item.Link,
                    item.Image,
                    item.SubTitle,
                    item.PubDate,
                    item.Director,
                    item.Actor,
                    item.UserRating
                    ));
            }

            this.DataContext = listData;
        }

        private void BtnDeleteWatchList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnNaverMovieLink_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
