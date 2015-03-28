using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MoePic.Models
{
    public static class UriHelp
    {
        public static Dictionary<String, String> GetQueryString(Uri uri)
        {
            string query = uri.Query.Remove(0,1);
            var querys = query.Split('&');
            Dictionary<String, String> queryString = new Dictionary<string, string>();
            foreach (var item in querys)
            {
                var keyValue = item.Split('=');
                queryString.Add(keyValue[0], keyValue.Length > 1 ? keyValue[1] : null);
            }
            return queryString;
        }

        public static String GetShareUrl(MoePost post)
        {
            return String.Format("http://higan.sinaapp.com/MoePic.php?type=post&id={0}&web={1}", post.id, post.preview_url.Contains("yande") ? "yandere" : "konachan");
        }

        public async static Task<String> GetShortUrl(String url, String token)
        {
            HttpPostRequest request = new HttpPostRequest();
            String data = await request.PostDataAsync(String.Format("https://api.weibo.com/2/short_url/shorten.json?access_token={0}&url_long={1}", token, System.Net.HttpUtility.UrlEncode(url)), true, false, "GET");
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ChangeResult>(data);
            if(result.error == null)
            {
                return result.urls[0].url_short;
            }
            else
            {
                return null;
            }
        }

        public class ShortUrl
        {
            public String url_short { get; set; }
            public String url_long { get; set; }
            public int type { get; set; }
            public bool result { get; set; }
        }

        public class ChangeResult
        {
            public List<ShortUrl> urls { get; set; }
            public string error { get; set; }

            public int error_code { get; set; }
            public String request { get; set; }
        }
    }

}
