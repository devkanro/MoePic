using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoePic.Resources;
using System.IO.IsolatedStorage;
using MoePic.Controls;
using MoePic.Models;

namespace MoePic
{
    public partial class MainPage : Models.MoePicPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while(NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            StatusBarView view = new StatusBarView(MoePic.Models.Settings.Current.WebSiteLogo, MoePic.Models.Settings.Current.WebSiteLogo == Controls.Logos.Yandere ? "yande.re" : "Konachan", true, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.WebSiteChange += StatusBarService_WebSiteChange;
            MoePic.Models.StatusBarService.Change(view);
            fav.Source = Settings.Current.SaveRAM ? MoePic.Models.FavoriteHelp.FavoriteList.Select(post =>  CDNHelper.GetCDNUri(post.preview_url)).ToList() : MoePic.Models.FavoriteHelp.FavoriteList.Select(post => CDNHelper.GetCDNUri(post.sample_url)).ToList();
            fav.StartAnime();
            
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                SetWhatsNew();
                UpdataBookmark();
            }
        }

        List<MoePic.Models.MoePost> NewPostsList;

        public async void SetWhatsNew()
        {
            if (MoePic.Models.Settings.Current.Website == "K")
            {
                if (MoePic.Models.Settings.Current.LastPostK > 0)
                {
                    NewPostsList = await MoePic.Models.MoebooruAPI.GetPostsFromMin(MoePic.Models.Settings.Current.LastPostK, 1, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
                    whatsNew.Source = new List<string>(NewPostsList.Select((post) => CDNHelper.GetCDNUri(post.preview_url).OriginalString));
                }
            }
            else
            {
                if (MoePic.Models.Settings.Current.LastPostY > 0)
                {
                    NewPostsList = await MoePic.Models.MoebooruAPI.GetPostsFromMin(MoePic.Models.Settings.Current.LastPostY, 1, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
                    whatsNew.Source = new List<string>(NewPostsList.Select((post) => CDNHelper.GetCDNUri(post.preview_url).OriginalString));
                }
            }
        }

        void StatusBarService_WebSiteChange(object sender, EventArgs e)
        {
            switch(Viewer.SelectedIndex)
            {
                case 0:
                    PostsLoaded = false;
                    PoolsLoaded = false;
                    break;
                case 1:
                    PostsViewer_FirstLoad();
                    PoolsLoaded = false;
                    break;
                case 2:
                    PostsLoaded = false;
                    PoolsViewer_FirstLoad();
                    break;
            }
            SetWhatsNew();
            UpdataBookmark();
        }

        MoePic.Models.MoePost FirstPost;
        MoePic.Models.MoePost LastPost;

        async void PostsViewer_FirstLoad()
        {
            if (!backing)
            {
                PostsViewer.LoadStart();
                PostsViewer.Clear();
                List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromPage(1, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
                if (PostList.Count > 0)
                {
                    FirstPost = PostList.First();
                    if (MoePic.Models.Settings.Current.Website == "K")
                    {
                        MoePic.Models.Settings.Current.NewLastPostK = (int)FirstPost.id;
                    }
                    else
                    {
                        MoePic.Models.Settings.Current.NewLastPostY = (int)FirstPost.id;
                    }
                    LastPost = PostList.Last();
                    foreach (var item in PostList)
                    {
                        PostsViewer.AddPost(item);
                    }
                }
                PostsViewer.LoadOver();
            }
        }

        private async System.Threading.Tasks.Task<bool> PostsViewer_RequestLoadData(object sender, EventArgs e)
        {
            if (!backing && PostsLoaded)
            {
                if(LastPost != null)
                {
                    List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromMax((int)LastPost.id, 1, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
                    if (PostList.Count > 0)
                    {
                        FirstPost = PostList.First();
                        LastPost = PostList.Last();
                        if (MoePic.Models.Settings.Current.Website == "Y")
                        {
                            MoePic.Models.Settings.Current.YLast = (int)FirstPost.id;
                        }
                        else
                        {
                            MoePic.Models.Settings.Current.KLast = (int)FirstPost.id;
                        }
                        foreach (var item in PostList)
                        {
                            PostsViewer.AddPost(item);
                        }
                    }
                }
            }
            return true;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("SearchPage.xaml");
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("AboutPage.xaml");
        }

        bool ExitConfirm = false;

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if(!e.Cancel)
            {
                if (!ExitConfirm)
                {
                    e.Cancel = true;
                    ExitConfirm = true;
                    MoePic.Models.ToastService.Show(AppResources.BackAgain, (s, ea) => { ExitConfirm = false; }, (s, ea) => { ExitConfirm = false; }, (s, ea) => { ExitConfirm = false; });
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("DownListPage.xaml");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("FavoritePage.xaml");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("SettingPage.xaml");
        }

        bool backing = false;

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PostsLoaded = true;
            Viewer.SelectedIndex = 1;
            if (!backing && (MoePic.Models.Settings.Current.Website == "Y" ? MoePic.Models.Settings.Current.YLast : MoePic.Models.Settings.Current.KLast) != -1)
            {
                PostsViewer.LoadStart();
                backing = true;
                PostsViewer.Clear();
                List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromMax((MoePic.Models.Settings.Current.Website == "Y" ? MoePic.Models.Settings.Current.YLast : MoePic.Models.Settings.Current.KLast) + 1, 1,MoePic.Models.Settings.Current.Limit,MoePic.Models.Settings.Current.Rating);
                if (PostList.Count > 0)
                {
                    FirstPost = PostList.First();
                    LastPost = PostList.Last();

                    if (MoePic.Models.Settings.Current.Website == "Y")
                    {
                        MoePic.Models.Settings.Current.YLast = (int)FirstPost.id;
                    }
                    else
                    {
                        MoePic.Models.Settings.Current.KLast = (int)FirstPost.id;
                    }
                    foreach (var item in PostList)
                    {
                        PostsViewer.AddPost(item);
                    }
                }
                PostsViewer.LoadOver();
                backing = false;
            }
            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("RankingPage.xaml");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("PostViewPage.xaml", "Random");
            
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (NewPostsList != null && NewPostsList.Count != 0)
            {
                MoePic.Models.NavigationService.Navigate("WhatsNewPage.xaml",NewPostsList);
            }
            else
            {
                MoePic.Models.ToastService.Show(AppResources.NoNewPost);
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

            MoePic.Models.NavigationService.Navigate("UserPage.xaml");
        }

        int poolPage = 1;

        async void PoolsViewer_FirstLoad()
        {
            PoolsViewer.LoadStart();
            PoolsViewer.Clear();
            List<MoePic.Models.MoePool> PoolList = await MoePic.Models.MoebooruAPI.GetPoolFormPage(poolPage++, 16);
            if (PoolList.Count > 0)
            {
                foreach (var item in PoolList)
                {
                    PoolsViewer.AddPool(item);
                }
            }
            PoolsViewer.LoadOver();
        }

        private async System.Threading.Tasks.Task<bool> PoolsViewer_RequestLoadData(object sender, EventArgs e)
        {
            if(PoolsLoaded)
            {
                List<MoePic.Models.MoePool> PoolList = await MoePic.Models.MoebooruAPI.GetPoolFormPage(poolPage++, 16);
                if (PoolList.Count > 0)
                {
                    foreach (var item in PoolList)
                    {
                        PoolsViewer.AddPool(item);
                    }
                }

            }
            return true;
        }

        bool PostsLoaded = false;
        bool PoolsLoaded = false;

        private void Viewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(Viewer.SelectedIndex)
            {
                case 0:
                    indexViewer.Visibility = System.Windows.Visibility.Visible;
                    PostsViewer.Visibility = System.Windows.Visibility.Collapsed;
                    PoolsViewer.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 1:
                    indexViewer.Visibility = System.Windows.Visibility.Collapsed;
                    PostsViewer.Visibility = System.Windows.Visibility.Visible;
                    PoolsViewer.Visibility = System.Windows.Visibility.Collapsed;
                    if(!PostsLoaded)
                    {
                        PostsLoaded = true;
                        PostsViewer_FirstLoad();
                    }
                    break;
                case 2:
                    indexViewer.Visibility = System.Windows.Visibility.Collapsed;
                    PostsViewer.Visibility = System.Windows.Visibility.Collapsed;
                    PoolsViewer.Visibility = System.Windows.Visibility.Visible;
                    if (!PoolsLoaded)
                    {
                        PoolsLoaded = true;
                        PoolsViewer_FirstLoad();
                    }
                    break;
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("HistoryPage.xaml");
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            switch (Viewer.SelectedIndex)
            {
                case 0:
                case 1:
                    var s = PostsViewer.Source.ToList();
                    PostsViewer.Source.Clear();
                    //PostsViewer.ToTop();
                    foreach (var item in s)
                    {
                        PostsViewer.Source.Add(item);
                    }
                    break;
                case 2:
                    var c = PoolsViewer.Source.ToList();
                    PoolsViewer.Source.Clear();
                    //PoolsViewer.ToTop();
                    foreach (var item in c)
                    {
                        PoolsViewer.Source.Add(item);
                    }
                    break;
                default:
                    break;
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            PostsLoaded = false;
            PoolsLoaded = false;
            ToastService.Show("已刷新列表.");
        }



        public async void SetPostListStart(int id)
        {
            PostsLoaded = true;
            Viewer.SelectedIndex = 1;
            if (!backing)
            {
                PostsViewer.LoadStart();
                backing = true;
                PostsViewer.Clear();
                List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromMax(id +1, 1, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
                if (PostList.Count > 0)
                {
                    FirstPost = PostList.First();
                    LastPost = PostList.Last();

                    if (MoePic.Models.Settings.Current.Website == "Y")
                    {
                        MoePic.Models.Settings.Current.YLast = (int)FirstPost.id;
                    }
                    else
                    {
                        MoePic.Models.Settings.Current.KLast = (int)FirstPost.id;
                    }
                    foreach (var item in PostList)
                    {
                        PostsViewer.AddPost(item);
                    }
                }
                PostsViewer.LoadOver();
                backing = false;
            }
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask task = new Microsoft.Phone.Tasks.EmailComposeTask();
            task.To = "higan@live.cn";
            task.Subject = "MoePic 用户反馈";
            task.Body = "欢迎您对MoePic提出意见或者建议,有任何问题都可以发送邮件给我,我会尽快回复您.";
            task.Show();
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri("http://higan.me/index.php/archives/51/");
            task.Show();
        }

        public void UpdataBookmark()
        {
            if (Settings.Current.Website == "K")
            {
                if (Settings.Current.BookMarkK[0] != null)
                {
                    bookmarkImage1.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkK[0].preview_url));
                }
                else
                {
                    bookmarkImage1.Source = null;
                }
                if (Settings.Current.BookMarkK[1] != null)
                {
                    bookmarkImage2.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkK[1].preview_url));
                }
                else
                {
                    bookmarkImage2.Source = null;
                }
                if (Settings.Current.BookMarkK[2] != null)
                {
                    bookmarkImage3.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkK[2].preview_url));
                }
                else
                {
                    bookmarkImage3.Source = null;
                }
                if (Settings.Current.BookMarkK[3] != null)
                {
                    bookmarkImage4.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkK[3].preview_url));
                }
                else
                {
                    bookmarkImage4.Source = null;
                }
                if (Settings.Current.BookMarkK[4] != null)
                {
                    bookmarkImage5.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkK[4].preview_url));
                }
                else
                {
                    bookmarkImage5.Source = null;
                }
                if (Settings.Current.BookMarkK[5] != null)
                {
                    bookmarkImage6.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkK[5].preview_url));
                }
                else
                {
                    bookmarkImage6.Source = null;
                }
            }
            else
            {
                if (Settings.Current.BookMarkY[0] != null)
                {
                    bookmarkImage1.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkY[0].preview_url));
                }
                else
                {
                    bookmarkImage1.Source = null;
                }
                if (Settings.Current.BookMarkY[1] != null)
                {
                    bookmarkImage2.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkY[1].preview_url));
                }
                else
                {
                    bookmarkImage2.Source = null;
                }
                if (Settings.Current.BookMarkY[2] != null)
                {
                    bookmarkImage3.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkY[2].preview_url));
                }
                else
                {
                    bookmarkImage3.Source = null;
                }
                if (Settings.Current.BookMarkY[3] != null)
                {
                    bookmarkImage4.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkY[3].preview_url));
                }
                else
                {
                    bookmarkImage4.Source = null;
                }
                if (Settings.Current.BookMarkY[4] != null)
                {
                    bookmarkImage5.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkY[4].preview_url));
                }
                else
                {
                    bookmarkImage5.Source = null;
                }
                if (Settings.Current.BookMarkY[5] != null)
                {
                    bookmarkImage6.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Settings.Current.BookMarkY[5].preview_url));
                }
                else
                {
                    bookmarkImage6.Source = null;
                }
            }
        }

        public String ExOnlyKey = "";

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            if(!noClick)
            {
                if(!(App.Current as App).IsTrial)
                {
                    ExOnlyKey += (sender as Button).Tag.ToString();
                    if (ExOnlyKey.Contains("220111") && !Settings.Current.IsExOnlyAble && Settings.Current.IsExAble)
                    {
                        Settings.Current.IsExOnlyAble = true;
                        ToastService.Show("已解锁ExOnly分级.");
                        ExOnlyKey = "";
                    }
                    if (ExOnlyKey.Contains("122222"))
                    {
                        Settings.Current.ThemeColorUnlock = 8;
                        ToastService.Show("已解锁全部主题色.");
                        ExOnlyKey = "";
                    }
                    if (ExOnlyKey.Contains("521333"))
                    {
                        Controls.ColorControl.Init();
                        ExOnlyKey = "";
                    }
                    if (ExOnlyKey.Contains("040404"))
                    {
                        if (Models.MemoryDiagnosticsHelper.IsOpen)
                        {
                            Models.MemoryDiagnosticsHelper.Stop();
                        }
                        else
                        {
                            Models.MemoryDiagnosticsHelper.Start();
                        }
                        ExOnlyKey = "";
                    }
                }

                int n = int.Parse((sender as Button).Tag.ToString());

                if (Settings.Current.Website == "K" ? Settings.Current.BookMarkK[n] == null : Settings.Current.BookMarkY[n] == null)
                {
                    if (Settings.Current.Website == "K")
                    {
                        Settings.Current.BookMarkK[n] = FirstPost;
                    }
                    else
                    {
                        Settings.Current.BookMarkY[n] = FirstPost;
                    }
                }
                else
                {
                    if (Settings.Current.Website == "K")
                    {
                        SetPostListStart((int)Settings.Current.BookMarkK[n].id);
                    }
                    else
                    {
                        SetPostListStart((int)Settings.Current.BookMarkY[n].id);
                    }
                }
                UpdataBookmark();
            }
            else
            {
                noClick = false;
            }
        }

        bool noClick = false;

        private void Button_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            noClick = true;
            int n = int.Parse((sender as Button).Tag.ToString());

            if (Settings.Current.Website == "K")
            {
                Settings.Current.BookMarkK[n] = null;
            }
            else
            {
                Settings.Current.BookMarkY[n] = null;
            }
            UpdataBookmark();
        }
    
    }
}