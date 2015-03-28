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
    public partial class HistoryPage : Models.MoePicPage
    {
        public HistoryPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                viewer.Source = Models.HistoryHelp.PostHistory;
            }
            MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Controls.Logos.History, MoePic.Resources.AppResources.History, false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            base.OnNavigatedTo(e);
        }
    }
}