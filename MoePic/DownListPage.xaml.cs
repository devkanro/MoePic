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
    public partial class DownListPage : Models.MoePicPage
    {
        public DownListPage()
        {
            InitializeComponent();
            DownloadList.ItemsSource = MoePic.Models.DownloadTaskManger.DownloadQueue;
            CompleteList.ItemsSource = MoePic.Models.DownloadTaskManger.CompleteQueue;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Controls.Logos.Download, "下载中心", false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            if(e.NavigationMode == NavigationMode.New)
            {
                MoePic.Models.DownloadTaskManger.DownloadQueue.CollectionChanged += DownloadQueue_CollectionChanged;
                MoePic.Models.DownloadTaskManger.CompleteQueue.CollectionChanged += CompleteList_CollectionChanged;
                SetNoDataText();
                if(Models.NavigationService.GetNavigateArgs(NavigationContext) != null)
                {
                    viewer.SelectedIndex = (int)Models.NavigationService.GetNavigateArgs(NavigationContext);
                }
            }
            base.OnNavigatedTo(e);
        }

        void SetNoDataText()
        {
            if(MoePic.Models.DownloadTaskManger.DownloadQueue.Count != 0)
            {
                downloadNoData.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                downloadNoData.Visibility = System.Windows.Visibility.Visible;
            }

            if (MoePic.Models.DownloadTaskManger.CompleteQueue.Count != 0)
            {
                completeNoData.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                completeNoData.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void CompleteList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetNoDataText();
        }

        void DownloadQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetNoDataText();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MoePic.Models.DownloadTaskManger.DownloadQueue.CollectionChanged -= DownloadQueue_CollectionChanged;
            MoePic.Models.DownloadTaskManger.CompleteQueue.CollectionChanged -= CompleteList_CollectionChanged;
            base.OnBackKeyPress(e);
        }
    }
}