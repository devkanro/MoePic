using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.Http;

namespace MoePic.Models
{
    public partial class WeiboControl : UserControl
    {
        public WeiboControl()
        {
            InitializeComponent();
            Width = 480;
            Height = DisplaySize.Current.Height;
        }

        public WeiboToken Token { get; set; }

        public String Content { get; set; }

        public void Start(WeiboToken token = null,System.IO.Stream image = null, String url = null)
        {
            Token = token;
            Image = image;
            Url = url;
            InputOut.Begin();
        }

        public System.IO.Stream Image { get; set; }

        public String Url { get; set; }

        public async void Share()
        {

            if(Token == null)
            {
                webViewer.Visibility = System.Windows.Visibility.Visible;
                webViewer.Navigate(new Uri(String.Format(@"https://api.weibo.com/oauth2/authorize?client_id={0}&response_type=code&redirect_uri={1}",ShareAPI.WeiboClientID,ShareAPI.RedirectUri)));
            }
            else
            {
                HttpClient client = new HttpClient();

                System.Net.Http.MultipartFormDataContent postData = new System.Net.Http.MultipartFormDataContent();
                var tokenString = new System.Net.Http.StringContent(Token.access_token);
                tokenString.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                postData.Add(tokenString, "access_token");

                if(Url != null)
                {
                    var shortUrl = await UriHelp.GetShortUrl(Url, Token.access_token);
                    Content += shortUrl;
                }

                var statusString = new System.Net.Http.StringContent(Content);
                statusString.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                postData.Add(statusString, "status");

                if(Image!= null)
                {
                    var picStream = new System.Net.Http.StreamContent(Image);
                    picStream.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
                    postData.Add(picStream, "pic", "share.jpg");

                    HttpResponseMessage response = await client.PostAsync("https://upload.api.weibo.com/2/statuses/upload.json", postData);
                    Image.Close();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ShareResult>(responseBody);
                    if (response.IsSuccessStatusCode && result.error == null)
                    {
                        if(Shared != null)
                        {
                            Shared(this, new ShareCompleteArgs() { RawData = responseBody, Success = true, Result = result });
                        }
                    }
                    else
                    {
                        Shared(this, new ShareCompleteArgs() { RawData = responseBody, Success = false, Result = result });
                    }
                }
                else
                {

                    HttpResponseMessage response = await client.PostAsync("https://upload.api.weibo.com/2/statuses/upload.json", postData);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ShareResult>(responseBody);
                    if (response.IsSuccessStatusCode && result.error == null)
                    {
                        if (Shared != null)
                        {
                            Shared(this, new ShareCompleteArgs() { RawData = responseBody, Success = true, Result = result });
                        }
                    }
                    else
                    {
                        Shared(this, new ShareCompleteArgs() { RawData = responseBody, Success = false, Result = result });
                    }
                }
            }
        }

        private async void webViewer_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().Contains("https://api.weibo.com/oauth2/default.html?code="))
            {
                webViewer.Visibility = System.Windows.Visibility.Collapsed;
                var query = UriHelp.GetQueryString(e.Uri);
                e.Cancel = true;
                //https://api.weibo.com/oauth2/access_token?client_id=YOUR_CLIENT_ID&client_secret=YOUR_CLIENT_SECRET&grant_type=authorization_cod
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                String result = await httpPostRequest.PostDataAsync(String.Format("https://api.weibo.com/oauth2/access_token?client_id={0}&client_secret={1}&grant_type=authorization_code&redirect_uri={2}&code={3}", ShareAPI.WeiboClientID, ShareAPI.WeiboClientSecret, ShareAPI.RedirectUri, query["code"]));
                try
                {
                    Token = Newtonsoft.Json.JsonConvert.DeserializeObject<WeiboToken>(result);
                    Logined(this, new WeiboLoginCompleteArgs() { RawData = result, Success = true, Token = Token });
                    Share();
                }
                catch
                {
                    Logined(this, new WeiboLoginCompleteArgs() { RawData = result, Success = false, Token = null });
                    Shared(this, new ShareCompleteArgs() { Success = false, RawData = null, Result = null });
                }

                
            }
        }

        public event EventHandler<WeiboLoginCompleteArgs> Logined;
        public event EventHandler<ShareCompleteArgs> Shared;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Content = contentText.Text == "" ? "分享图片" : contentText.Text;
            InputIn.Begin();
            Share();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (this.Parent as System.Windows.Controls.Primitives.Popup).IsOpen = false;
            Shared(this, new ShareCompleteArgs() { Success = false });
        }
    }

    public class WeiboLoginCompleteArgs : EventArgs
    {
        public bool Success { get; set; }
        public String RawData { get; set; }
        public WeiboToken Token { get; set; }
    }

    public class ShareCompleteArgs : EventArgs
    {
        public bool Success { get; set; }
        public String RawData { get; set; }

        public ShareResult Result { get; set; }
    }

    public class ShareResult
    {
        public String id { get; set; }
        public string error { get; set; }
        public int error_code { get; set; }
        public String request { get; set; }
    }
}
