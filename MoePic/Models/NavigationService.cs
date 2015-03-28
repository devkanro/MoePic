using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MoePic;

namespace MoePic.Models
{
    
    public static class NavigationService
    {

        /// <summary>
        /// 导航到一个新的页面
        /// </summary>
        /// <param name="uri">页面名称或者路径</param>
        public static void Navigate(String uri)
        {
            App.RootFrame.Navigate(new Uri("/" + uri, UriKind.Relative));
        }

        /// <summary>
        /// 导航到一个新的页面,并提供参数
        /// </summary>
        /// <param name="uri">页面名称或者路径</param>
        /// <param name="args">需要传递的参数</param>
        public static void Navigate(String uri, object args)
        {
            string uriString = String.Format("/{0}?ArgsKey={1}", uri, (App.Current.Resources["RuntimeResources"] as RuntimeResources).ArgsCount);
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).NavigateArgs.Add((App.Current.Resources["RuntimeResources"] as RuntimeResources).ArgsCount++, args);
            App.RootFrame.Navigate(new Uri(uriString, UriKind.Relative));
        }

        /// <summary>
        /// 获取导航参数
        /// </summary>
        /// <param name="content">导航的操作状态</param>
        /// <returns>导航参数</returns>
        public static object GetNavigateArgs(System.Windows.Navigation.NavigationContext content)
        {
            int argsKey = 0;
            if(content.QueryString.ContainsKey("ArgsKey") && int.TryParse(content.QueryString["ArgsKey"],out argsKey))
            {
                return (App.Current.Resources["RuntimeResources"] as RuntimeResources).NavigateArgs[argsKey];
            }
            return null;
        }

        public static void Clear()
        {
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).ArgsCount = 0;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).NavigateArgs.Clear();
        }

        public static System.Windows.Navigation.JournalEntry RemoveBackEntry()
        {
            if(CanGoBack)
            {
                return App.RootFrame.RemoveBackEntry();
            }
            return null;
        }

        public static void RemoveAllBackEntry()
        {
            while (CanGoBack)
            {
                App.RootFrame.RemoveBackEntry();
            }
        }

        public static bool CanGoBack
        {
            get
            {
                return App.RootFrame.CanGoBack;
            }
        }

        public static void GoBack()
        {
            if(CanGoBack)
            {
                
                App.RootFrame.GoBack();
            }
        }
    }
}
