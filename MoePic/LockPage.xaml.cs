using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoePic.Resources;

namespace MoePic
{
    public partial class LockPage : PhoneApplicationPage
    {
        public LockPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }


        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.9;

            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/check.png", UriKind.Relative));
            appBarButton.Click += ApplicationBarIconButton_Click;
            appBarButton.Text = AppResources.Confirm;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Models.StatusBarService.HideStatusBar();
            base.OnNavigatedTo(e);
        }

        private void now3_Checked(object sender, RoutedEventArgs e)
        {
            nowPassword.Visibility = System.Windows.Visibility.Collapsed;
            nowPasswordText.Visibility = System.Windows.Visibility.Visible;
        }

        private void now3_Unchecked(object sender, RoutedEventArgs e)
        {
            nowPassword.Visibility = System.Windows.Visibility.Visible;
            nowPasswordText.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            App.Exit();
            base.OnBackKeyPress(e);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (nowPassword.Password != Models.Settings.Current.Password && nowPasswordText.Text != Models.Settings.Current.Password)
            {
                MessageBox.Show("", MoePic.Resources.AppResources.PasswordIncorrect, MessageBoxButton.OK);
                return;
            }
            (App.Current.Resources["RuntimeResources"] as Models.RuntimeResources).RequestPassword = false;
            NavigationService.GoBack();
            Models.StatusBarService.ShowStatusBar();
        }
    }
}