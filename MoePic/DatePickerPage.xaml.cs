using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic
{
    public partial class DatePickerPage : PhoneApplicationPage
    {
        public static DateTime Date { get; set; }
        public static bool IsCancel { get; set; }

        public static void Show(DateTime selectedDate)
        {
            Date = selectedDate.Date;
            Models.NavigationService.Navigate("DatePickerPage.xaml");
        }

        public DatePickerPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            date.SelectedValue = Date;
            IsCancel = false;
            Models.StatusBarService.HideStatusBar();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Models.StatusBarService.ShowStatusBar();
            if (DateSelected != null)
            {
                DateSelected(this, new EventArgs());
            }
            base.OnNavigatingFrom(e);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            Date = date.SelectedValue;
            NavigationService.GoBack();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            IsCancel = true;
            NavigationService.GoBack();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            IsCancel = true;
        }

        public static event EventHandler DateSelected;

    }

}