using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MoePic
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //Shows the rate reminder message, according to the settings of the RateReminder.
            (App.Current as App).rateReminder.Notify();
        }

        /// <summary>
        /// Navigates to about page.
        /// </summary>
        private void GoToAbout(object sender, GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
