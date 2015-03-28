using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Threading.Tasks;
using System.ComponentModel;

using Microsoft.Phone.BackgroundTransfer;

namespace MoePic.Models
{
    public class DownloadTask : DependencyObject
    {
        public DownloadTask(MoePost post, ImageType type)
        {
            Post = post;
            ImageType = type;
            Status = DownloadStatus.Waiting;
        }

        public DownloadTask(DownloadTaskSerializeObject task)
        {
            Post = task.Post;
            ImageType = task.Type;
            Status = task.Status;
        }

        public MoePost Post
        {
            get { return (MoePost)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Post.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostProperty =
            DependencyProperty.Register("Post", typeof(MoePost), typeof(DownloadTask), null);


        public void Cancel()
        {
            if(WebClient.IsBusy)
            {
                WebClient.CancelAsync();
            }
        }

        public DownloadStatus Status
        {
            get { return (DownloadStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(DownloadStatus), typeof(DownloadTask),new PropertyMetadata(StatusChangedCallback));
        
        public static void StatusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((d as DownloadTask).DownloadStatusChanged != null)
            {
                (d as DownloadTask).DownloadStatusChanged(d,new DownloadStatusChangedEventArgs((DownloadStatus)e.OldValue,(DownloadStatus)e.NewValue));
            }
        }


        public ImageType ImageType
        {
            get { return (ImageType)GetValue(ImageTypeProperty); }
            set { SetValue(ImageTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageTypeProperty =
            DependencyProperty.Register("ImageType", typeof(ImageType), typeof(DownloadTask), null);


        public WebClient WebClient = new WebClient();

        public void Restart()
        {
            if (WebClient.IsBusy)
            {
                WebClient.CancelAsync();
            }
            DownloadTaskManger.StartDownload(this);
        }

        public static String GetImageUri(MoePost post ,ImageType type)
        {
            switch (type)
            {
                case ImageType.Sample:
                    return post.sample_url;
                case ImageType.PNG:
                    return post.file_url;
                case ImageType.JPG:
                    return post.jpeg_url;
            }
            return null;
        }

        public void OnDownloadStatusChanged()
        {
            if (DownloadStatusChanged != null)
            {
                DownloadStatusChanged(this, new DownloadStatusChangedEventArgs(DownloadStatus.Waiting, Status));
            }
        }

        public void Start()
        {
            if(!WebClient.IsBusy && CheckNetwork())
            {
                Status = DownloadStatus.Downloading;
                WebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                WebClient.OpenReadCompleted += WebClient_OpenReadCompleted;
                WebClient.OpenReadAsync(CDNHelper.GetCDNUri(GetImageUri(Post, ImageType)));
            }
        }

        async void WebClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Result != null)
                {
                    Status = DownloadStatus.Saving;
                    if(Post.preview_url.Contains("yande"))
                    {
                        await ImageSaveHelp.SaveImage(String.Format("{0} - {1}{2}","yande.re",Post.id,Path.GetExtension(GetImageUri(Post,ImageType))), e.Result);
                    }
                    else
                    {
                        await ImageSaveHelp.SaveImage(String.Format("{0} - {1}.{2}", "Konachan", Post.id, Path.GetExtension(GetImageUri(Post, ImageType))), e.Result);
                    }
                    ToastService.Show(new Uri(Post.preview_url), Resources.AppResources.SaveImage, (s, a) => { NavigationService.Navigate("DownListPage.xaml", 1); }, null, null);
                    Status = DownloadStatus.Complete;
                    if (DownloadCompleted != null)
                    {
                        DownloadCompleted(this, new EventArgs());
                    }
                }
                else
                {
                    if (e.Error != null)
                    {
                        Status = DownloadStatus.Error;
                        Error = e.Error;
                        ToastService.Show(new Uri(Post.preview_url), Resources.AppResources.DownloadBreak, (s, a) => { NavigationService.Navigate("DownListPage.xaml", 0); }, null, null);
                    }
                    else
                    {
                        Status = DownloadStatus.Unknown;
                        ToastService.Show(new Uri(Post.preview_url), Resources.AppResources.DownloadBreak, (s, a) => { NavigationService.Navigate("DownListPage.xaml", 0); }, null, null);
                    }
                    if (DownloadFailed != null)
                    {
                        DownloadFailed(this, new EventArgs());
                    }
                }
            }
            else
            {
                if(DownloadCanceled != null)
                {
                    DownloadCanceled(this, new EventArgs());
                }
            }

        }

        public event EventHandler<DownloadStatusChangedEventArgs> DownloadStatusChanged;

        public Exception Error
        {
            get { return (Exception)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Error.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register("Error", typeof(Exception), typeof(DownloadTask), null);

        

        public event EventHandler DownloadCompleted;
        public event EventHandler DownloadFailed;
        public event EventHandler DownloadCanceled;

        void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
            TotalBytesToReceive = e.TotalBytesToReceive;
            BytesReceived = e.BytesReceived;
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(this, e);
            }
        }



        public long BytesReceived
        {
            get { return (long)GetValue(BytesReceivedProperty); }
            set { SetValue(BytesReceivedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BytesReceived.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BytesReceivedProperty =
            DependencyProperty.Register("BytesReceived", typeof(long), typeof(DownloadTask), null);

        

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(int), typeof(DownloadTask), null);

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;


        public long TotalBytesToReceive
        {
            get { return (long)GetValue(TotalBytesToReceiveProperty); }
            set { SetValue(TotalBytesToReceiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalBytesToReceive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalBytesToReceiveProperty =
            DependencyProperty.Register("TotalBytesToReceive", typeof(long), typeof(DownloadTask), null);


        public bool CheckNetwork()
        {
            switch (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType)
            {
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandCdma:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandGsm:
                    if (Settings.Current.IsWifiOnly)
                    {
                        (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi = true;
                        Status = DownloadStatus.WaitingForWifi;
                        return false;
                    }
                    else
                    {
                        (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi = false;
                        (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking = false;
                        return true;
                    }
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None:
                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking = true;
                    Status = DownloadStatus.WaitingForNetwork;
                    return false;
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211:
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet:

                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi = false;
                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking = false;
                    return true;
                default:

                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi = false;
                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking = false;
                    return true;
            }
        }
    }

    public class DownloadStatusChangedEventArgs : EventArgs
    {
        public DownloadStatusChangedEventArgs(DownloadStatus oldStatus , DownloadStatus newStatus)
        {
            NewStatus = newStatus;
            OldStatus = oldStatus;
        }

        public DownloadStatus NewStatus{get;private set;}

        public DownloadStatus OldStatus{get;private set;}
    }

    public enum DownloadStatus
    {
        Waiting,
        Downloading,
        Saving,
        Complete,
        Error,
        WaitingForWifi,
        WaitingForNetwork,
        Unknown
    }

    public enum ImageType
    {
        Sample,
        JPG,
        PNG,
    }
}
