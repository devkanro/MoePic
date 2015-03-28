using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoePic.Controls;
using MoePic.Models;
using MoePic.Resources;

namespace MoePic
{
    public partial class SplashScreen : Models.MoePicPage
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(StatusBarService.Inited)
            {
                StatusBarService.HideStatusBar();
            }
            base.OnNavigatedTo(e);
#if PREVIEW
            if(DateTime.Now > new DateTime(2015,3,1))
            {
                System.Windows.MessageBox.Show("MoePic Preview!已过期,且不再受支持,请到商店获取发行版MoePic.", "Preview已过期", MessageBoxButton.OK);
                App.Exit();
            }
            else
            {
                Init();
            }
#else
            Init();
#endif
        }

        public async void Init()
        {
            loading.Text = MoePic.Resources.AppResources.LoadSettings;

            await Task.Run(() =>
            {
                App.RootFrame.Dispatcher.BeginInvoke(() =>
                {

                    try
                    {
                        MoePic.Models.FavoriteHelp.ReadFavorite();
                    }
                    catch (Exception)
                    {
                        ToastService.Show(MoePic.Resources.AppResources.FavListDamage);
                        FavoriteHelp.FavoriteList = new System.Collections.ObjectModel.ObservableCollection<MoePost>();
                        FavoriteHelp.FavoriteTagList = new System.Collections.ObjectModel.ObservableCollection<MoeTag>();
                    }

                    try
                    {
                        MoePic.Models.Settings.ReadSettings();
                    }
                    catch (Exception)
                    {
                        ToastService.Show(MoePic.Resources.AppResources.SettingsDamage);
                        Settings.Init();
                    }

                    try
                    {
                        MoePic.Models.DownloadTaskManger.ReadDownloadList();
                    }
                    catch (Exception ex)
                    {
                        ToastService.Show(MoePic.Resources.AppResources.DownloadListDamage);
                        DownloadTaskManger.CompleteQueue = new System.Collections.ObjectModel.ObservableCollection<DownloadTask>();
                        DownloadTaskManger.DownloadQueue = new System.Collections.ObjectModel.ObservableCollection<DownloadTask>();
                    }

                    try
                    {
                        MoePic.Models.HistoryHelp.Read();
                    }
                    catch (Exception ex)
                    {
                        ToastService.Show("浏览历史文件已损坏,将初始化浏览历史.");
                        HistoryHelp.PostHistory = new System.Collections.ObjectModel.ObservableCollection<MoePost>();
                        HistoryHelp.SearchHistory = new System.Collections.ObjectModel.ObservableCollection<string>();
                    }
                });
            });

            if (Settings.Current.Password != null)
            {
                (App.Current.Resources["RuntimeResources"] as RuntimeResources).RequestPassword = true;
            }


            if (!IsolatedStorageSettings.ApplicationSettings.Contains(App.GetVersion()))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(App.GetVersion(), true);
                IsolatedStorageSettings.ApplicationSettings.Save();
                MessageBoxService.Show("有什么新功能?", "MoePic 已更新至Ver.0.4.4.0,以下是新加的功能:\r\n01.开放无限试用版,但是只提供基本的浏览功能.\r\n02.正式版用户将得到全部功能的支持,可使用CDN等拓展功能.\r\n03.登录步骤转至后台,加快启动速度.", true, new Command(MoePic.Resources.AppResources.Confirm, null));
            }

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("CDN"))
            {
                IsolatedStorageSettings.ApplicationSettings.Add("CDN", true);
                IsolatedStorageSettings.ApplicationSettings.Save();
                MessageBoxService.Show("关于CDN服务的说明", "MoePic 在Ver.0.4.3.3版本加入了CDN服务,该服务可以对中国内地用户的访问进行加速,国外用户访问可能变慢,而且CDN服务目前处于实验阶段,在使用中可能出现某些未知的问题,例如:无法载入某些图片,图片列表不更新,请谨慎使用.", true, new Command(MoePic.Resources.AppResources.Confirm, null));
            }

            LoadNotice();

            await Task.Run(() =>
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                if (file.FileExists("NotFirstRun"))
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (NavigationContext.QueryString.ContainsKey("goto"))
                        {
                            switch (NavigationContext.QueryString["goto"])
                            {
                                case "post":
                                    if (NavigationContext.QueryString.ContainsKey("id") && NavigationContext.QueryString.ContainsKey("web"))
                                    {
                                        NavigationService.Navigate(new Uri(String.Format("/PostViewPage.xaml?id={0}&web={1}", NavigationContext.QueryString["id"], NavigationContext.QueryString["web"]), UriKind.Relative));
                                    }
                                    else
                                    {
                                        Models.NavigationService.Navigate("MainPage.xaml");
                                    }
                                    break;
                                case "pool":
                                    if (NavigationContext.QueryString.ContainsKey("id") && NavigationContext.QueryString.ContainsKey("web"))
                                    {
                                        NavigationService.Navigate(new Uri(String.Format("/PoolViewPage.xaml?id={0}&web={1}", NavigationContext.QueryString["id"], NavigationContext.QueryString["web"]), UriKind.Relative));
                                    }
                                    else
                                    {
                                        Models.NavigationService.Navigate("MainPage.xaml");
                                    }
                                    break;
                                default:
                                    Models.NavigationService.Navigate("MainPage.xaml");
                                    break;
                            }
                        }
                        else
                        {
                            Models.NavigationService.Navigate("MainPage.xaml");
                        }
                    });
                }
                else
                {
                    file.CreateFile("NotFirstRun").Close();
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (NavigationContext.QueryString.ContainsKey("goto"))
                        {
                            switch (NavigationContext.QueryString["goto"])
                            {
                                case "post":
                                    if (NavigationContext.QueryString.ContainsKey("id") && NavigationContext.QueryString.ContainsKey("web"))
                                    {
                                        NavigationService.Navigate(new Uri(String.Format("/PostViewPage.xaml?id={0}&web={1}", NavigationContext.QueryString["id"], NavigationContext.QueryString["web"]), UriKind.Relative));
                                    }
                                    else
                                    {
                                        Models.NavigationService.Navigate("MainPage.xaml");
                                    }
                                    break;
                                case "pool":
                                    if (NavigationContext.QueryString.ContainsKey("id") && NavigationContext.QueryString.ContainsKey("web"))
                                    {
                                        NavigationService.Navigate(new Uri(String.Format("/PoolViewPage.xaml?id={0}&web={1}", NavigationContext.QueryString["id"], NavigationContext.QueryString["web"]), UriKind.Relative));
                                    }
                                    else
                                    {
                                        Models.NavigationService.Navigate("MainPage.xaml");
                                    }
                                    break;
                                default:
                                    Models.NavigationService.Navigate("MainPage.xaml");
                                    break;
                            }
                        }
                        else
                        {
                            if((App.Current as App).IsTrial)
                            {
                                Models.NavigationService.Navigate("MainPage.xaml");
                            }
                            else
                            {
                                Models.NavigationService.Navigate("LoginPage.xaml");
                            }
                        }
                    });
                }
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            base.OnNavigatedFrom(e);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            (sender as System.Windows.Threading.DispatcherTimer).Stop();
            MessageBoxService.Show(MoePic.Resources.AppResources.LoginTimeOutTile, MoePic.Resources.AppResources.LoginTimeOutContent, true, new Command(MoePic.Resources.AppResources.Retry, (s, a) => { (sender as System.Windows.Threading.DispatcherTimer).Start(); }), new Command(MoePic.Resources.AppResources.Cancel, (s, a) => { Models.NavigationService.Navigate("MainPage.xaml"); }));
        }

        void LoadNotice()
        {
            Task task = new Task(async () =>
            {
                HttpPostRequest http = new HttpPostRequest();
                var data = await http.PostDataAsync("http://higan.sinaapp.com/moepic.txt");
                Dispatcher.BeginInvoke(async () =>
                {
                    var notice = Newtonsoft.Json.JsonConvert.DeserializeObject<Notice>(data);
                    notice.Show();
                    if((App.Current as App).IsTrial)
                    {
                        LiveAPI.ClientID = "000000004813DB3B";
                    }
                    else
                    {
                        await LiveAPI.Initialize("000000004813DB3B");
                    }
                });
            });

            task.Start();
        }

        
    }

    public class Notice
    {
        public String Key { get; set; }
        public String Tilte { get; set; }
        public String Content { get; set; }
        public List<NoticeButton> Buttons { get; set; }

        public void Show()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(Key))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(Key, true);
                IsolatedStorageSettings.ApplicationSettings.Save();
                var buttons = Buttons.Select(b => new Command(b.Button, (s, a) => { var task = new Microsoft.Phone.Tasks.WebBrowserTask() { Uri = new Uri(b.Url) }; task.Show(); })).ToList();
                buttons.Add(new Command(MoePic.Resources.AppResources.Confirm, null));
                MessageBoxService.Show(Tilte, Content, true, buttons.ToArray());
            }
        }
    }

    public class NoticeButton
    {
        public String Button { get; set; }
        public String Url { get; set; }
    }
}