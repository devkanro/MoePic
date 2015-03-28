using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using MoePic.Controls;


namespace MoePic.Models
{
    public static class StatusBarService
    {
        static Popup Popup;
        static StatusBar StatusBar;
        static bool ImageSet = false;

        public static void Init(StatusBarView view, bool show = true)
        {
            if (Popup == null)
            {
                Popup popup = Popup = new Popup();
                popup.Child = StatusBar = new StatusBar(view);
                popup.IsOpen = true;
                if (Image != null)
                {
                    StatusBar.Loaded += StatusBar_Loaded;
                }
                StatusBar.WebSiteChange += StatusBar_WebSiteChange;
            }
            if (show)
            {
                ShowStatusBar();
            }
        }

        public static bool Inited
        {
            get
            {
                if(Popup != null && StatusBar != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static void StatusBar_WebSiteChange(object sender, EventArgs e)
        {
            if(WebSiteChange != null)
            {
                WebSiteChange(sender, e);
            }
        }

        public static void Change(StatusBarView view)
        {
            StatusBar.Change(view);
        }

        static System.Windows.Media.Imaging.BitmapImage Image;

        public static void SetDownloadImage(System.Windows.Media.Imaging.BitmapImage image)
        {
            if (StatusBar != null)
            {
                StatusBar.SetDownloadImage(image);
            }
            else
            {
                Image = image;
            }
        }

        static void StatusBar_Loaded(object sender, RoutedEventArgs e)
        {
            if(Image != null)
            {
                StatusBar.SetDownloadImage(Image);
                Image = null;
                StatusBar.Loaded -= StatusBar_Loaded;
            }
        }


        public static void Change(Logos logo, String tilte, bool buttonShow, bool userShow)
        {
            Change(new StatusBarView(logo, tilte, buttonShow, userShow));
        }

        static bool IsShow = false;

        public static void ShowStatusBar()
        {
            if (!IsShow && StatusBar!= null)
            {
                StatusBar.Show();
                IsShow = true;
            }
        }

        public static void HideStatusBar()
        {
            if(IsShow)
            {
                StatusBar.Hide();
                IsShow = false;
            }
        }

        public static event EventHandler WebSiteChange;
    }
}
