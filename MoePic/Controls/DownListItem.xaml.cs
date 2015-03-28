using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using MoePic.Models;

namespace MoePic.Controls
{
    public partial class DownListItem : ListBoxItem
    {
        public DownListItem()
        {
            InitializeComponent();
        }

        public MoePic.Models.DownloadTask Task
        {
            get { return (MoePic.Models.DownloadTask)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Task.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(MoePic.Models.DownloadTask), typeof(DownListItem), new PropertyMetadata(TaskChangedCallback));

        String SizeText;
        long Size;
        bool MB;

        public static void TaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DownListItem).Size = GetSize(e.NewValue as DownloadTask);

            switch ((d as DownListItem).Task.ImageType)
            {
                case ImageType.Sample:
                    (d as DownListItem).Tilte.Text = String.Format("{0}.Sample.JPG", (d as DownListItem).Task.Post.id);
                    break;
                case ImageType.PNG:
                    (d as DownListItem).Tilte.Text = String.Format("{0}.PNG", (d as DownListItem).Task.Post.id);
                    break;
                case ImageType.JPG:
                    (d as DownListItem).Tilte.Text = String.Format("{0}.JPG", (d as DownListItem).Task.Post.id);
                    break;
                default:
                    break;
            }

            if((d as DownListItem).Size > 1024 * 1024)
            {
                (d as DownListItem).MB = true;
                (d as DownListItem).SizeText = String.Format("{0:F2}MB", 1.0 * (d as DownListItem).Size / (1024 * 1024));
            }
            else
            {
                (d as DownListItem).MB = false;
                (d as DownListItem).SizeText = String.Format("{0}KB", (d as DownListItem).Size / 1024);
            }


            (d as DownListItem).SmapleImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri((e.NewValue as DownloadTask).Post.preview_url));

            if((e.NewValue as DownloadTask).Post.preview_url.Contains("yande"))
            {
                (d as DownListItem).InfoText.Text = MoePic.Resources.AppResources.From + "Yande.re";
            }
            else
            {
                (d as DownListItem).InfoText.Text = MoePic.Resources.AppResources.From + "Konachan";
            }

            switch ((e.NewValue as  DownloadTask).Status)
            {
                case DownloadStatus.Waiting:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                    (d as DownListItem).ProgressBar.IsIndeterminate = true;
                    (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.Waiting;

                    ContextMenu menu = new ContextMenu();
                    MenuItem item1 = new MenuItem();
                    item1.Header = MoePic.Resources.AppResources.CancelDownload;
                    item1.Click += (s,a) => 
                    {
                        DownloadTaskManger.RemoveDownload(e.NewValue as  DownloadTask);
                    };
                    menu.Items.Add(item1);
                    ContextMenuService.SetContextMenu(d, menu);
                    break;
                case DownloadStatus.Downloading:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                    (d as DownListItem).ProgressBar.IsIndeterminate = false;
                    (d as DownListItem).ProgressText.Text = String.Format((d as DownListItem).MB ?  "{0:F2}/{1}" : "{0}/{1}", (d as DownListItem).MB ? 1.0 * (e.NewValue as DownloadTask).BytesReceived / (1024 * 1024) : 1.0 * (e.NewValue as DownloadTask).BytesReceived / 1024, (d as DownListItem).SizeText);
                    (d as DownListItem).ProgressBar.Value = (e.NewValue as DownloadTask).Progress;

                    menu = new ContextMenu();
                    item1 = new MenuItem();
                    item1.Header = MoePic.Resources.AppResources.CancelDownload;
                    item1.Click += (s,a) => 
                    {
                        DownloadTaskManger.RemoveDownload(e.NewValue as  DownloadTask);
                    };
                    menu.Items.Add(item1);
                    ContextMenuService.SetContextMenu(d, menu);
                    break;
                case DownloadStatus.Saving:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                    (d as DownListItem).ProgressBar.IsIndeterminate = true;
                    (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.Saving;
                    break;
                case DownloadStatus.Complete:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Collapsed;
                    (d as DownListItem).ProgressText.Text = (d as DownListItem).SizeText;

                    
                    menu = new ContextMenu();
                    item1 = new MenuItem();
                    item1.Header = MoePic.Resources.AppResources.DelDownloadHis;
                    item1.Click += (s,a) => 
                    {
                        DownloadTaskManger.RemoveDownloadLog(e.NewValue as DownloadTask);
                    };
                    menu.Items.Add(item1);
                    ContextMenuService.SetContextMenu(d, menu);
                    break;
                case DownloadStatus.Error:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Collapsed;
                    (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention;

                    
                    menu = new ContextMenu();
                    item1 = new MenuItem();
                    item1.Header = MoePic.Resources.AppResources.RestartDownload;
                    item1.Click += (s,a) => 
                    {
                        DownloadTaskManger.RestartDownload(e.NewValue as DownloadTask);
                    };
                    menu.Items.Add(item1);
                    ContextMenuService.SetContextMenu(d, menu);
                    break;
                case DownloadStatus.WaitingForWifi:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                    (d as DownListItem).ProgressBar.IsIndeterminate = true;
                    (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention2;

                    
                    menu = new ContextMenu();
                    item1 = new MenuItem();
                    item1.Header = MoePic.Resources.AppResources.CancelDownload;
                    item1.Click += (s,a) => 
                    {
                        DownloadTaskManger.RemoveDownload(e.NewValue as  DownloadTask);
                    };
                    menu.Items.Add(item1);
                    ContextMenuService.SetContextMenu(d, menu);
                    break;
                case DownloadStatus.WaitingForNetwork:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                    (d as DownListItem).ProgressBar.IsIndeterminate = true;
                    (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention2;

                    
                    menu = new ContextMenu();
                    item1 = new MenuItem();
                    item1.Header = MoePic.Resources.AppResources.CancelDownload;
                    item1.Click += (s,a) => 
                    {
                        DownloadTaskManger.RemoveDownload(e.NewValue as  DownloadTask);
                    };
                    menu.Items.Add(item1);
                    ContextMenuService.SetContextMenu(d, menu);
                    break;
                case DownloadStatus.Unknown:
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Collapsed;
                    (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention;

                    
                    menu = new ContextMenu();
                    item1 = new MenuItem();
                    item1.Header = MoePic.Resources.AppResources.CancelDownload;
                    item1.Click += (s,a) => 
                    {
                        DownloadTaskManger.RestartDownload(e.NewValue as  DownloadTask);
                    };
                    menu.Items.Add(item1);
                    ContextMenuService.SetContextMenu(d, menu);
                    break;
                default:
                    break;
            }

            (e.NewValue as DownloadTask).DownloadStatusChanged += (s, a) =>
                {
                    switch (a.NewStatus)
                    {
                        case DownloadStatus.Waiting:
                            (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                            (d as DownListItem).ProgressBar.IsIndeterminate = true;
                            (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.Waiting;

                            ContextMenu menu = new ContextMenu();
                            MenuItem item1 = new MenuItem();
                            item1.Header = MoePic.Resources.AppResources.CancelDownload;
                            item1.Click += (s1, a1) =>
                            {
                                DownloadTaskManger.RemoveDownload(e.NewValue as DownloadTask);
                            };
                            menu.Items.Add(item1);
                            ContextMenuService.SetContextMenu(d, menu);
                            break;
                        case DownloadStatus.Downloading:

                            menu = new ContextMenu();
                            item1 = new MenuItem();
                            item1.Header = MoePic.Resources.AppResources.CancelDownload;
                            item1.Click += (s1, a1) =>
                            {
                                DownloadTaskManger.RemoveDownload(e.NewValue as DownloadTask);
                            };
                            menu.Items.Add(item1);
                            ContextMenuService.SetContextMenu(d, menu);
                            break;
                        case DownloadStatus.Saving:
                            (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                            (d as DownListItem).ProgressBar.IsIndeterminate = true;
                            (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.Saving;
                            break;
                        case DownloadStatus.Complete:
                            (d as DownListItem).ProgressBar.Visibility = Visibility.Collapsed;
                            (d as DownListItem).ProgressText.Text = (d as DownListItem).SizeText;


                            menu = new ContextMenu();
                            item1 = new MenuItem();
                            item1.Header = MoePic.Resources.AppResources.DelDownloadHis;
                            item1.Click += (s1, a1) =>
                            {
                                DownloadTaskManger.RemoveDownloadLog(e.NewValue as DownloadTask);
                            };
                            menu.Items.Add(item1);
                            ContextMenuService.SetContextMenu(d, menu);
                            break;
                        case DownloadStatus.Error:
                            (d as DownListItem).ProgressBar.Visibility = Visibility.Collapsed;
                            (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention;

                            menu = new ContextMenu();
                            item1 = new MenuItem();
                            item1.Header = MoePic.Resources.AppResources.RestartDownload;
                            item1.Click += (s1, a1) =>
                            {
                                DownloadTaskManger.RestartDownload(e.NewValue as DownloadTask);
                            };
                            menu.Items.Add(item1);
                            ContextMenuService.SetContextMenu(d, menu);
                            break;
                        case DownloadStatus.WaitingForWifi:
                            (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                            (d as DownListItem).ProgressBar.IsIndeterminate = true;
                            (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention2;

                            menu = new ContextMenu();
                            item1 = new MenuItem();
                            item1.Header = MoePic.Resources.AppResources.CancelDownload;
                            item1.Click += (s1, a1) =>
                            {
                                DownloadTaskManger.RemoveDownload(e.NewValue as DownloadTask);
                            };
                            menu.Items.Add(item1);
                            ContextMenuService.SetContextMenu(d, menu);
                            break;
                        case DownloadStatus.WaitingForNetwork:
                            (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                            (d as DownListItem).ProgressBar.IsIndeterminate = true;
                            (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention2;

                            menu = new ContextMenu();
                            item1 = new MenuItem();
                            item1.Header = MoePic.Resources.AppResources.CancelDownload;
                            item1.Click += (s1, a1) =>
                            {
                                DownloadTaskManger.RemoveDownload(e.NewValue as DownloadTask);
                            };
                            menu.Items.Add(item1);
                            ContextMenuService.SetContextMenu(d, menu);
                            break;
                        case DownloadStatus.Unknown:
                            (d as DownListItem).ProgressBar.Visibility = Visibility.Collapsed;
                            (d as DownListItem).ProgressText.Text = MoePic.Resources.AppResources.NeedAttention;

                            menu = new ContextMenu();
                            item1 = new MenuItem();
                            item1.Header = MoePic.Resources.AppResources.RestartDownload;
                            item1.Click += (s1, a1) =>
                            {
                                DownloadTaskManger.RestartDownload(e.NewValue as DownloadTask);
                            };
                            menu.Items.Add(item1);
                            ContextMenuService.SetContextMenu(d, menu);
                            break;
                        default:
                            break;
                    }
                };
            (e.NewValue as DownloadTask).DownloadProgressChanged += (s, a) =>
                {
                    (d as DownListItem).ProgressBar.Visibility = Visibility.Visible;
                    (d as DownListItem).ProgressBar.IsIndeterminate = false;
                    (d as DownListItem).ProgressText.Text = String.Format((d as DownListItem).MB ? "{0:F2}/{1}" : "{0}/{1}", (d as DownListItem).MB ? 1.0 * a.BytesReceived / (1024 * 1024) : 1.0 * a.BytesReceived / 1024, (d as DownListItem).SizeText);
                    (d as DownListItem).ProgressBar.Value = a.ProgressPercentage;
                };
        }

        

        public static long GetSize(DownloadTask task)
        {
            switch (task.ImageType)
            {
                case ImageType.Sample:
                    return task.Post.sample_file_size;
                case ImageType.PNG:
                    return task.Post.file_size;
                case ImageType.JPG:
                    return task.Post.jpeg_file_size == 0 ? task.Post.file_size : task.Post.jpeg_file_size;
                default:
                    return 0;
            }
        }

        private void listBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            switch (Task.Status)
            {
                case DownloadStatus.Waiting:
                case DownloadStatus.Downloading:
                case DownloadStatus.Saving:
                case DownloadStatus.Complete:
                    MoePic.Models.NavigationService.Navigate("PostViewPage.xaml", Task.Post);
                    break;
                case DownloadStatus.Error:
                    MessageBoxService.Show(MoePic.Resources.AppResources.DownloadErrorTitle, String.Format("{0}\r\n{1}", MoePic.Resources.AppResources.DownloadErrorContent, Task.Error.Message), true, new Command(MoePic.Resources.AppResources.Cancel, null));
                    break;
                case DownloadStatus.Unknown:
                    MessageBoxService.Show(MoePic.Resources.AppResources.DownloadUnknowErrorTitle, String.Format("{0}\r\n", MoePic.Resources.AppResources.DownloadUnknowErrorContent), true, new Command(MoePic.Resources.AppResources.Cancel, null));
                    break;
                case DownloadStatus.WaitingForWifi:
                    MessageBoxService.Show(MoePic.Resources.AppResources.DownloadBreakTitle, MoePic.Resources.AppResources.DownloadBreakWifiContent, true, new Command(MoePic.Resources.AppResources.Cancel, null));
                    break;
                case DownloadStatus.WaitingForNetwork:
                    MessageBoxService.Show(MoePic.Resources.AppResources.DownloadBreakTitle, MoePic.Resources.AppResources.DownloadBreakNetContent, true, new Command(MoePic.Resources.AppResources.Cancel, null));
                    break;
                default:
                    break;
            }
        }

    }
}