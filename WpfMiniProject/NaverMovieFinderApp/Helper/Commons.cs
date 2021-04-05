using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NaverMovieFinderApp
{
    public class Commons
    {
        // 즐겨찾기 여부 플래그
        public static bool IsFavorite = false;

        public static bool IsDelete = false; // 즐겨찾기 삭제와 보기 플래그
        
        // NLog 정적객체
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<MessageDialogResult> ShowMessageAsync(
            string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }

        public static string GetOpenApiResult(string openApiUrl, string clientID, string clientSecret)
        {
            string result = "";

            try
            {
                WebRequest request = WebRequest.Create(openApiUrl);
                request.Headers.Add("X-Naver-Client-Id", clientID);
                request.Headers.Add("X-Naver-Client-Secret", clientSecret);

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                result = reader.ReadToEnd();

                reader.Close();
                stream.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"예외발생 : {ex}");
            }

            return result;
        }

        public static string StripHtmlTag(string text)
        {
            return Regex.Replace(text, @"<(.|\n)*?>", ""); // HTML 태그 삭제하는 정규표현식
        }

        public static string StripPipe(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            else
                return text.Substring(0, text.LastIndexOf("|")).Replace("|", ", ");
        }
    }
}
