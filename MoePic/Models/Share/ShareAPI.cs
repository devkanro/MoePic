using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using MicroMsg.sdk;

using Newtonsoft.Json;


namespace MoePic.Models
{
    public static class ShareAPI
    {
        //https://api.weibo.com/oauth2/authorize?client_id=YOUR_CLIENT_ID&response_type=code&redirect_uri=YOUR_REGISTERED_REDI
        public static String WeiboClientID = "4115142091";
        public static String WeiboClientSecret = "59e3a3185ace43b261fe503945bae9d7";
        public static String RedirectUri = "https://api.weibo.com/oauth2/default.html";

        public static String WXClientID = "wx30f8bcf233e8cc01";

        public static WeiboClient Client = new WeiboClient();

        public static void Share(String content, System.Windows.Media.Imaging.BitmapImage image, MoePost post)
        {
            Client.Init();
            Client.Share(content, image, post);
            Client.Shared += (s, a) =>
                {
                    if (Shared != null)
                    {
                        Shared(s, a);
                    }
                };
        }

        public static void ShareToWX( MoePost post,BitmapImage image)
        {
            using (var stream = ImageSaveHelp.GetStream(image))
            {
                byte[] buff = new byte[stream.Length];
                stream.Read(buff, 0, buff.Length);
                stream.Close();

                WXImageMessage imageMessage = new WXImageMessage(post.sample_url);
                imageMessage.Title = "Post " + post.id.ToString();
                imageMessage.Description = "来自 MoePic 的图片分享";
                imageMessage.ThumbData = ImageSaveHelp.GetThumbData(image);

                try
                {
                    SendMessageToWX.Req req = new SendMessageToWX.Req(imageMessage, SendMessageToWX.Req.WXSceneTimeline);
                    IWXAPI api = WXAPIFactory.CreateWXAPI(WXClientID);
                    api.SendReq(req);
                }
                catch (Exception ex)
                {
                    Models.MessageBoxService.Show("分享到微信失败", "以下是错误信息:\r\n" + ex.Message, true, new Controls.Command(Resources.AppResources.Confirm, null));
                }
            }
        }

        public static void ShareToQzone(MoePost post)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri(String.Format("http://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?url={0}&desc={1}&title={2}&site=MoePic&pics={3}", System.Net.HttpUtility.UrlEncode(UriHelp.GetShareUrl(post)), System.Net.HttpUtility.UrlEncode("来自 MoePic 的分享"), System.Net.HttpUtility.UrlEncode("Post " + post.id.ToString()), System.Net.HttpUtility.UrlEncode(post.preview_url)));
            task.Show();
        }

        public static event EventHandler<ShareCompleteArgs> Shared;
    }

    public class WeiboClient
    {
        public event EventHandler<ShareCompleteArgs> Shared;

        public bool IsLogin { get; private set; }

        public WeiboToken Token { get; private set; }

        public void Init()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if(file.FileExists("weibo.token"))
            {
                var stream = file.OpenFile("weibo.token", FileMode.Open);
                byte[] buff = new byte[stream.Length];
                stream.Read(buff, 0, buff.Length);
                stream.Close();
                var data = Encoding.UTF8.GetString(buff, 0, buff.Length);
                try
                {
                    Token = JsonConvert.DeserializeObject<WeiboToken>(data);
                }
                catch
                {
                    IsLogin = false;
                }
                IsLogin = true;
            }
            else
            {
                IsLogin = false;
            }
        }

        public void Share(String content, System.Windows.Media.Imaging.BitmapImage image, MoePost post)
        {
            WeiboControl loginControl = new WeiboControl();
            Popup popup = new Popup()
            {
                Child = loginControl,
            };
            popup.IsOpen = true;
            loginControl.Shared += (s, a) =>
                {
                    popup.IsOpen = false;
                    if(a.Success)
                    {
                        ToastService.Show(new Uri(post.preview_url), "图片已分享到微博");
                    }
                    else
                    {
                        if(a.Result != null)
                        {
                            if (a.Result.error_code == 21327)
                            {
                                ToastService.Show(new Uri(post.preview_url), "登录凭据已过期,请重新登录后分享.");

                            }
                            else
                            {
                                ToastService.Show(new Uri(post.preview_url), "图片分享失败,错误码:" + a.Result.error_code.ToString());
                            }
                        }
                    }
                    if(Shared != null)
                    {
                        Shared(s, a);
                    }
                };
            if(Token == null)
            {
                loginControl.Logined += (s, a) =>
                {
                    if(a.Success)
                    {
                        Token = a.Token; 
                        SaveToken();
                    }
                    else
                    {
                        ToastService.Show(new Uri(post.preview_url), "登录微博失败.");
                    }
                };
            }
            loginControl.Start(Token, ImageSaveHelp.GetStream(image), UriHelp.GetShareUrl(post));
        }

        public void SaveToken()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            var stream = file.OpenFile("weibo.token", FileMode.Create);
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Token));
            stream.Write(buff, 0, buff.Length);
            stream.Close();
        }
    }

    public class WeiboToken
    {
        public String access_token { get; set; }
        public int remind_in { get; set; }
        public int expires_in { get; set; }
    }
}
