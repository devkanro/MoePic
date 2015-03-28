using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MoePic.Models
{
    public static class ExHentaiAPI
    {
        public static async Task<String> LoginAsync(String user,String password)
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://forums.e-hentai.org/index.php?act=Login&CODE=01&CookieDate=1 ");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var query = Encoding.UTF8.GetBytes(string.Format("UserName={0}&PassWord={1}&x=0&y=0"));
            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(query, 0, query.Length);
            }

            string cookie;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                cookie = response.get_Headers().get_Item("Set-Cookie");
            }
        }
    }
}
