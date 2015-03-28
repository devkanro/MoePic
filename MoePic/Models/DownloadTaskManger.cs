using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

using Microsoft.Phone.BackgroundTransfer;

using Newtonsoft.Json;

namespace MoePic.Models
{
    public static class DownloadTaskManger
    {
        public static ObservableCollection<DownloadTask> DownloadQueue;
        public static ObservableCollection<DownloadTask> CompleteQueue;
        public static Queue<DownloadTask> WaitingQueue = new Queue<DownloadTask>();
        public static List<DownloadTask> RemoveList = new List<DownloadTask>();
        public static int DownloadCount = 0;

        static DownloadTaskManger()
        {
            Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;
        }

        static void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, Microsoft.Phone.Net.NetworkInformation.NetworkNotificationEventArgs e)
        {
            App.RootFrame.Dispatcher.BeginInvoke(() =>
            {
                switch (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType)
                {
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandCdma:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandGsm:
                        if ((App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking)
                        {
                            (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking = false;
                            NextDownload();
                        }
                        break;
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None:
                        break;
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet:
                        if ((App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi || (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking)
                        {
                            (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking = false;
                            (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi = false;
                            NextDownload();
                        }
                        break;
                    default:
                        break;
                }
            });
        }

        public static void AddDownload(DownloadTask task)
        {
            DownloadQueue.Add(task);
            task.DownloadStatusChanged += task_DownloadStatusChanged;

            WaitingQueue.Enqueue(task);
            NextDownload();
        }

        public static void RemoveDownload(DownloadTask task)
        {
            if(task.Status == DownloadStatus.Downloading)
            {
                task.Cancel();
            }
            RemoveList.Add(task);
            DownloadQueue.Remove(task);
        }

        public static void StartDownload(DownloadTask task)
        {
            if(DownloadQueue.Contains(task))
            {
                if(!WaitingQueue.Contains(task))
                {
                    WaitingQueue.Enqueue(task);
                    NextDownload();
                }
            }
        }

        public static void RemoveDownloadLog(DownloadTask task)
        {
            CompleteQueue.Remove(task);
        }

        public static void RestartDownload(DownloadTask task)
        {
            task.Status = DownloadStatus.Waiting;
            WaitingQueue.Enqueue(task);
            NextDownload();
        }

        public static bool NextDownload()
        {
            if (DownloadCount < Settings.Current.PeerNum && WaitingQueue.Count > 0 && !(App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking && !(App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi)
            {
                if (RemoveList.Contains(WaitingQueue.Peek()))
                {
                    WaitingQueue.Dequeue();
                    NextDownload();
                }
                else
                {
                    WaitingQueue.Dequeue().Start();
                    NextDownload();
                    return true;
                }
            }
            return false;
        }

        static void task_DownloadStatusChanged(object sender, DownloadStatusChangedEventArgs e)
        {
            switch (e.NewStatus)
            {
                case DownloadStatus.Waiting:
                    break;
                case DownloadStatus.Downloading:
                    DownloadCount++;
                    break;
                case DownloadStatus.Saving:
                    break;
                case DownloadStatus.Complete:
                    DownloadQueue.Remove(sender as DownloadTask);
                    CompleteQueue.Insert(0, sender as DownloadTask);
                    DownloadCount--;
                    NextDownload();
                    break;
                case DownloadStatus.Error:
                case DownloadStatus.Unknown:
                    DownloadQueue.Remove(sender as DownloadTask);
                    DownloadQueue.Add(sender as DownloadTask);
                    DownloadCount--;
                    NextDownload();
                    break;
                case DownloadStatus.WaitingForWifi:
                case DownloadStatus.WaitingForNetwork:
                    DownloadQueue.Remove(sender as DownloadTask);
                    DownloadQueue.Add(sender as DownloadTask);
                    WaitingQueue.Enqueue(sender as DownloadTask);
                    DownloadCount--;
                    break;
            }
        }


        public static void ReadDownloadList()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            if (file.FileExists("CompleteList.json"))
            {
                IsolatedStorageFileStream stream = file.OpenFile("CompleteList.json", System.IO.FileMode.Open);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);
                var list = JsonConvert.DeserializeObject<List<DownloadTaskSerializeObject>>(jsonString);
                CompleteQueue = new ObservableCollection<DownloadTask>(
                list.Select(
                (t) =>
                {
                    return new DownloadTask(t);
                }));
                stream.Close();
            }
            else
            {
                CompleteQueue = new ObservableCollection<DownloadTask>();
            }

            if (file.FileExists("DownloadList.json"))
            {
                IsolatedStorageFileStream stream = file.OpenFile("DownloadList.json", System.IO.FileMode.Open);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);
                DownloadQueue = new ObservableCollection<DownloadTask>(
                JsonConvert.DeserializeObject<List<DownloadTaskSerializeObject>>(jsonString).Select((t) => { return new DownloadTask(t); }));
                List<DownloadTask> com = new List<DownloadTask>();
                foreach (var item in DownloadQueue)
                {
                    if (item.Status == DownloadStatus.Waiting)
                    {
                        item.DownloadStatusChanged += task_DownloadStatusChanged;
                        WaitingQueue.Enqueue(item);
                    }
                    else if(item.Status == DownloadStatus.Complete)
                    {
                        CompleteQueue.Add(item);
                        com.Add(item);
                    }
                }
                foreach (var item in com)
                {
                    DownloadQueue.Remove(item);
                }
                com.Clear();
                NextDownload();
                stream.Close();
            }
            else
            {
                DownloadQueue = new ObservableCollection<DownloadTask>();
            }
        }

        public static void SaveDownloadList()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile("DownloadList.json", System.IO.FileMode.Create);
            Byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(DownloadQueue.Select((task) => { return new DownloadTaskSerializeObject(task); })));
            stream.Write(buff, 0, buff.Length);
            stream.Close();
            stream = file.OpenFile("CompleteList.json", System.IO.FileMode.Create);
            buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(CompleteQueue.Select((task) => { return new DownloadTaskSerializeObject(task); })));
            stream.Write(buff, 0, buff.Length);
            stream.Close();
        }
    }

    public class DownloadTaskSerializeObject
    {
        public MoePost Post { get; set; }
        public ImageType Type { get; set; }
        public DownloadStatus Status { get; set; }

        public DownloadTaskSerializeObject()
        {

        }

        public DownloadTaskSerializeObject(DownloadTask task)
        {
            Post = task.Post;
            Type = task.ImageType;
            if (task.Status == DownloadStatus.Waiting || task.Status == DownloadStatus.WaitingForNetwork || task.Status == DownloadStatus.WaitingForWifi || task.Status == DownloadStatus.Saving || task.Status == DownloadStatus.Downloading)
            {
                Status = DownloadStatus.Waiting;
            }
            else
            {
                Status = task.Status;
            }
        }
    }
}
