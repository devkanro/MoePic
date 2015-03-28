using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Marketplace;
using MoePic.Models;
using MoePic.Resources;

namespace MoePic.Models
{
    public class MoePicPage : PhoneApplicationPage
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if ((App.Current.Resources["RuntimeResources"] as Models.RuntimeResources).RequestPassword)
            {
                Models.NavigationService.Navigate("LockPage.xaml");
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if(MessageBoxService.CloseLast())
            {
                e.Cancel = true;
            }
            base.OnBackKeyPress(e);
        }
    }
}
