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
using MoePic.Models;
using MoePic.Controls;

namespace MoePic
{
    public partial class LoginPage : Models.MoePicPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.9;
            ApplicationBar.BackgroundColor = System.Windows.Media.Color.FromArgb(0xFF, 0x00, 0x85, 0xB5);

            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/check.png", UriKind.Relative));
            appBarButton.Click += Login_Click;
            appBarButton.Text = AppResources.Login;
            ApplicationBar.Buttons.Add(appBarButton);
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/cancel.png", UriKind.Relative));
            appBarButton.Click += Cancel_Click;
            appBarButton.Text = AppResources.Cancel;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        void Cancel_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                Models.NavigationService.Navigate("MainPage.xaml");
            }
        }

        bool loging = false;

        async void Login_Click(object sender, EventArgs e)
        {
            if(!loging)
            {
                loging = true;
                Logining.Visibility = System.Windows.Visibility.Visible;
                bool logined = await MoePic.Models.LiveAPI.LoginAsync();
                if (logined)
                {
                    if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) == null)
                    {
                        Models.NavigationService.Navigate("MainPage.xaml");
                    }
                    else
                    {
                        NavigationService.GoBack();
                    }
                }
                else
                {
                    Logining.Visibility = System.Windows.Visibility.Collapsed;
                }
                loging = false;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) == null)
            {
                while(NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
            MoePic.Models.StatusBarService.Init(new Controls.StatusBarView(Controls.Logos.Microsoft, "", false, true));
            MoePic.Models.StatusBarService.Change(new Controls.StatusBarView(Controls.Logos.Microsoft, "", false, true));
            base.OnNavigatedTo(e);
        }
    }
}