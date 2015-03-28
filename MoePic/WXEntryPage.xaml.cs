using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MicroMsg.sdk;
namespace MoePic
{
    public partial class WXEntryPage : MicroMsg.sdk.WXEntryBasePage
    {
        public WXEntryPage()
        {
            InitializeComponent();
        }

        public override void On_SendMessageToWX_Response(SendMessageToWX.Resp response)
        {
            if(NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new Uri("/SplashScreen.xaml", UriKind.Relative));
                NavigationService.RemoveBackEntry();
            }
        }
    }
}