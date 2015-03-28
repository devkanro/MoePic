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
    public partial class PasswordPage : PhoneApplicationPage
    {
        public PasswordPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.9;

            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/check.png", UriKind.Relative));
            appBarButton.Click += ApplicationBarIconButton_Click_1;
            appBarButton.Text = AppResources.Confirm;
            ApplicationBar.Buttons.Add(appBarButton);
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/cancel.png", UriKind.Relative));
            appBarButton.Click += ApplicationBarIconButton_Click;
            appBarButton.Text = AppResources.Cancel;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        PasswordOperationType Mode;

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            Models.StatusBarService.ShowStatusBar();
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Models.StatusBarService.HideStatusBar();
            if(e.NavigationMode == NavigationMode.New)
            {
                Mode = (PasswordOperationType)Models.NavigationService.GetNavigateArgs(NavigationContext);
                switch (Mode)
                {
                    case PasswordOperationType.Create:
                        Tilte.Text = MoePic.Resources.AppResources.CreatePassword;
                        now1.Visibility = System.Windows.Visibility.Collapsed;
                        now2.Visibility = System.Windows.Visibility.Collapsed;
                        now3.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case PasswordOperationType.Change:
                        Tilte.Text = MoePic.Resources.AppResources.ChangePassword;
                        break;
                    case PasswordOperationType.Cancel:
                        Tilte.Text = MoePic.Resources.AppResources.ConfirmPassword;
                        new1.Visibility = System.Windows.Visibility.Collapsed;
                        new2.Visibility = System.Windows.Visibility.Collapsed;
                        new3.Visibility = System.Windows.Visibility.Collapsed;
                        confirm1.Visibility = System.Windows.Visibility.Collapsed;
                        confirm2.Visibility = System.Windows.Visibility.Collapsed;
                        confirm3.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
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

        private void new3_Checked(object sender, RoutedEventArgs e)
        {
            newPassword.Visibility = System.Windows.Visibility.Collapsed;
            newPasswordText.Visibility = System.Windows.Visibility.Visible;
        }

        private void new3_Unchecked(object sender, RoutedEventArgs e)
        {
            newPassword.Visibility = System.Windows.Visibility.Visible;
            newPasswordText.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void confirm3_Checked(object sender, RoutedEventArgs e)
        {
            confirmPassword.Visibility = System.Windows.Visibility.Collapsed;
            confirmPasswordText.Visibility = System.Windows.Visibility.Visible;
        }

        private void confirm3_Unchecked(object sender, RoutedEventArgs e)
        {
            confirmPassword.Visibility = System.Windows.Visibility.Visible;
            confirmPasswordText.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            Mode = (PasswordOperationType)Models.NavigationService.GetNavigateArgs(NavigationContext);
            switch (Mode)
            {
                case PasswordOperationType.Create:
                    if(newPassword.Password != confirmPassword.Password)
                    {
                        MessageBox.Show("", MoePic.Resources.AppResources.PassworldNotMatch, MessageBoxButton.OK);
                        return;
                    }
                    if (newPassword.Password.Length < 4)
                    {
                        MessageBox.Show(MoePic.Resources.AppResources.PasswordNotFormatContent, MoePic.Resources.AppResources.PasswordNotFormat, MessageBoxButton.OK);
                        return;
                    }
                    Models.Settings.Current.Password = newPassword.Password;
                    break;
                case PasswordOperationType.Change:
                    if (nowPassword.Password != Models.Settings.Current.Password)
                    {
                        MessageBox.Show("", MoePic.Resources.AppResources.PasswordIncorrect, MessageBoxButton.OK);
                        return;
                    }
                    if(newPassword.Password != confirmPassword.Password)
                    {
                        MessageBox.Show("", MoePic.Resources.AppResources.PassworldNotMatch, MessageBoxButton.OK);
                        return;
                    }
                    if (newPassword.Password.Length < 4)
                    {
                        MessageBox.Show(MoePic.Resources.AppResources.PasswordNotFormatContent, MoePic.Resources.AppResources.PasswordNotFormat, MessageBoxButton.OK);
                        return;
                    }
                    Models.Settings.Current.Password = newPassword.Password;
                    break;
                case PasswordOperationType.Cancel:
                    if (nowPassword.Password != Models.Settings.Current.Password)
                    {
                        MessageBox.Show("", MoePic.Resources.AppResources.PasswordIncorrect, MessageBoxButton.OK);
                        return;
                    }
                    Models.Settings.Current.Password = null;
                    break;
                default:
                    break;
            }

            NavigationService.GoBack();
        }
    }

    public enum PasswordOperationType
    {
        Create,
        Change,
        Cancel
    }
}