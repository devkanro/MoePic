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
    public static class ToastService
    {

        public static void Show(Uri image ,String content)
        {
            Show(image, content, null, null, null);
        }

        public static void Show(String content)
        {
            Show(new Uri("/Assets/Logo.png", UriKind.Relative), content, null, null, null);
        }

        public static void Show(Uri image ,String content,EventHandler<EventArgs> click, EventHandler<EventArgs> timeOver, EventHandler<EventArgs> close)
        {
            Popup popup = new Popup();
            ToastBox toast = new ToastBox(popup);
            popup.Child = toast;
            popup.IsOpen = true;
            toast.ShowToast(image, content, click, timeOver, close);
        }

        public static void Show(String content, EventHandler<EventArgs> click, EventHandler<EventArgs> timeOver, EventHandler<EventArgs> close)
        {
            Show(new Uri("/Assets/Logo.png", UriKind.Relative), content, click, timeOver, close);
        }

        
    }
}
