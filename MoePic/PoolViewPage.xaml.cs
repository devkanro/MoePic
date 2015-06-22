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
    public partial class PoolViewPage : Models.MoePicPage
    {
        public PoolViewPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.9;
            ApplicationBar.BackgroundColor = (App.Current.Resources["ThemeColor"] as Models.ThemeColor).StatusBar.Color;

            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/favs.addto.png", UriKind.Relative));
            appBarButton.Click += AddFavClick; ;
            appBarButton.Text = AppResources.Fav;
            ApplicationBar.Buttons.Add(appBarButton);
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/share.png", UriKind.Relative));
            appBarButton.Click += ShareClick;
            appBarButton.Text = "链接";
            ApplicationBar.Buttons.Add(appBarButton);
        }

        private void ShareClick(object sender, EventArgs e)
        {
            Clipboard.SetText(Models.UriHelp.GetShareUrl(Pool));
            Models.ToastService.Show("已复制链接分享到剪切板");
        }

        private void AddFavClick(object sender, EventArgs e)
        {
            if(Pool.posts.Count != 0)
            {
                foreach (var item in Pool.posts)
                {
                    Models.FavoriteHelp.AddFavorite(item);
                }

                Models.ToastService.Show($"已经将Pool {Pool.id}加入喜爱列表.");
            }
        }

        public MoePic.Models.MoePool Pool { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) != null)
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    (App.Current.Resources["RuntimeResources"] as Models.RuntimeResources).Pool = Pool = MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) as MoePic.Models.MoePool;
                }
                else
                {
                    Pool = (App.Current.Resources["RuntimeResources"] as Models.RuntimeResources).Pool;
                }
                MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(MoePic.Models.Settings.Current.WebSiteLogo, String.Format("Pool {0}", Pool.id), false, true);
                MoePic.Models.StatusBarService.Init(view);
                MoePic.Models.StatusBarService.Change(view);
            }
            else
            {
                try
                {
                    var id = int.Parse(NavigationContext.QueryString["id"]);
                    viewer.LoadStart();
                    MoePic.Controls.StatusBarView view = null;
                    if (NavigationContext.QueryString["web"] == "konachan")
                    {
                        view = new MoePic.Controls.StatusBarView(Controls.Logos.Konachan, String.Format("Pool {0}", id), false, true);
                        Models.MoebooruAPI.partWebsite = Models.MoebooruAPI.Konachan;
                    }
                    else
                    {
                        view = new MoePic.Controls.StatusBarView(Controls.Logos.Yandere, String.Format("Pool {0}", id), false, true);
                        Models.MoebooruAPI.partWebsite = Models.MoebooruAPI.Yandere;
                    }
                    MoePic.Models.StatusBarService.Init(view);
                    MoePic.Models.StatusBarService.Change(view);

                    if (e.NavigationMode == NavigationMode.New)
                    {
                        loadPool(id);
                    }
                }
                catch (Exception)
                {
                    String args = String.Empty;
                    foreach (var item in NavigationContext.QueryString)
                    {
                        if (args != String.Empty)
                        {
                            args += String.Format("&{0}={1}", item.Key, item.Value);
                        }
                        else
                        {
                            args += String.Format("{0}={1}", item.Key, item.Value);
                        }

                    }
                    Models.MessageBoxService.Show("导航参数错误", "由于提供的导航参数不符合格式,无法获取您请求的图集.\r\n以下是您提供的导航参数:\r\n" + args, false, new Controls.Command(MoePic.Resources.AppResources.Cancel, (s, a) => { App.Exit(); }));
                    return;
                }
                
            }
            base.OnNavigatedTo(e);
        }

        async void loadPool(int id)
        {
            var pool = await Models.MoebooruAPI.GetPoolFormId(id,Models.MoebooruAPI.partWebsite);
            (App.Current.Resources["RuntimeResources"] as Models.RuntimeResources).Pool = Pool = pool;

            if (Pool != null)
            {
                foreach (var item in Pool.posts)
                {
                    viewer.AddPost(item);
                }
            }
            viewer.LoadOver();
        }

        private async System.Threading.Tasks.Task<bool> viewer_FirstLoad(object sender, EventArgs e)
        {
            if (Pool != null)
            {
                foreach (var item in Pool.posts)
                {
                    viewer.AddPost(item);

                }
            }

            return true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            (App.Current.Resources["RuntimeResources"] as Models.RuntimeResources).Pool = null;
            base.OnBackKeyPress(e);
        }
    }
}