using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Data;
using System.Globalization;

namespace MoePic.Models
{
    public static class CDNHelper
    {
        public static Uri GetCDNUri(String url,bool konachanControl = false)
        {
            Uri uri = new Uri(url);
            if(Settings.Current.EnableCDN)
            {
                uri = new Uri(String.Format("{0}{1}", uri.Host.Contains("yande") ? "http://yandere.sinaapp.com" : (konachanControl ? "http://konachan.com" : "http://moepic.sinaapp.com"), uri.PathAndQuery));
            }
            return uri;
        }

        
    }

    public class CDNUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri uri = new Uri(value as String);
            return Settings.Current.EnableCDN ? String.Format("{0}{1}", uri.Host.Contains("yande") ? "http://yandere.sinaapp.com" : "http://moepic.sinaapp.com", uri.PathAndQuery) : value as String;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri uri = new Uri(value as String);
            return Settings.Current.EnableCDN ? String.Format("{0}{1}", uri.Host.Contains("yandere") ? "https://yande.re" : "http://konachan.com", uri.PathAndQuery) : value as String;
        }
    }
}
