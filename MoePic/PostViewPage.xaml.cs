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

namespace MoePic
{
    public partial class PostViewPage : Models.MoePicPage
    {
        public PostViewPage()
        {
            InitializeComponent();
        }

        MoePost Post { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.NavigationMode == NavigationMode.Back)
            {
                Post = (App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack.Peek();
            }

            if(e.NavigationMode == NavigationMode.New)
            {
                if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) is MoePost)
                {
                    Post = MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) as MoePost;
                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack.Push(Post);
                }
                else if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) is String && (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) as String) == "Random" && Post == null)
                {
                    MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Settings.Current.WebSiteLogo, MoePic.Resources.AppResources.Random, false, true);
                    MoebooruAPI.partWebsite = Settings.Current.WebSiteLogo == Controls.Logos.Yandere ? Models.MoebooruAPI.Yandere :  Models.MoebooruAPI.Konachan;
                    MoePic.Models.StatusBarService.Init(view);
                    MoePic.Models.StatusBarService.Change(view);
                    loadRandom();
                    return;
                }
                else if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) == null)
                {
                    try
                    {
                        var id = int.Parse(NavigationContext.QueryString["id"]);
                        postViewer.SetLoad(id);
                        MoePic.Controls.StatusBarView view = null;
                        if (NavigationContext.QueryString["web"] == "konachan")
                        {
                            view = new MoePic.Controls.StatusBarView(Controls.Logos.Konachan, String.Format("Post {0}", id), false, true);
                            MoebooruAPI.partWebsite = Models.MoebooruAPI.Konachan;
                        }
                        else
                        {
                            view = new MoePic.Controls.StatusBarView(Controls.Logos.Yandere, String.Format("Post {0}", id), false, true);
                            MoebooruAPI.partWebsite = Models.MoebooruAPI.Yandere;
                        }
                        MoePic.Models.StatusBarService.Init(view);
                        MoePic.Models.StatusBarService.Change(view);
                        loadPost(id);
                        return;
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
                        MessageBoxService.Show("导航参数错误", "由于提供的导航参数不符合格式,无法获取您请求的图片.\r\n以下是您提供的导航参数:\r\n" + args, false, new Controls.Command(MoePic.Resources.AppResources.Cancel, (s, a) => { App.Exit(); }));
                        return;
                    }

                }
            }
            

            if(Post != null)
            {
                MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Post.preview_url.Contains("yande") ? Controls.Logos.Yandere : Controls.Logos.Konachan, String.Format("Post {0}", Post.id), false, true);
                MoebooruAPI.partWebsite = Post.preview_url.Contains("yande") ? Models.MoebooruAPI.Yandere : Models.MoebooruAPI.Konachan;
                MoePic.Models.StatusBarService.Init(view);
                MoePic.Models.StatusBarService.Change(view);
                if(postViewer.Post == null)
                {
                    postViewer.SetPost(Post as MoePost);
                }
            }
            if (e.NavigationMode == NavigationMode.New)
            {
                
            }
            base.OnNavigatedTo(e);
        }
        public async void loadPost(int id)
        {
            var post = await MoebooruAPI.GetPostFormID(id, MoebooruAPI.partWebsite);
            Post = post;
            postViewer.SetPost(Post);
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack.Push(Post);
        }
        

        public async void loadRandom()
        {
            var Posts = await MoebooruAPI.GetPostsRandom(5, Settings.Current.Rating, MoebooruAPI.partWebsite);
            reload:
            if(Posts.Count == 0)
            {
                Posts = await MoebooruAPI.GetPostsRandom(5, Settings.Current.Rating, MoebooruAPI.partWebsite);
                goto reload;
            }
            Post = Posts[0];
            postViewer.SetPost(Post);
            MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(postViewer.Post.preview_url.Contains("yande") ? Controls.Logos.Yandere : Controls.Logos.Konachan, String.Format("Post {0}", Post.id), false, true);
            MoebooruAPI.partWebsite = postViewer.Post.preview_url.Contains("yande") ? Models.MoebooruAPI.Yandere : Models.MoebooruAPI.Konachan;
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack.Push(Post);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            postViewer.ReleaseImage();
            if ((App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack.Count > 0 && (App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack.Peek() == Post)
            {
                (App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack.Pop();
            }
            MoebooruAPI.partWebsite = null;
            base.OnBackKeyPress(e);
        }
    }
}