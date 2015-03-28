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
using MoePic.Controls;

namespace MoePic
{
    public partial class WhatsNewPage : Models.MoePicPage
    {
        public WhatsNewPage()
        {
            InitializeComponent();
        }

        int MinPost;

        int page = 2;

        private async System.Threading.Tasks.Task<bool> postsViewer_FirstLoad(object sender, EventArgs e)
        {
            list = Models.NavigationService.GetNavigateArgs(NavigationContext) as List<MoePost>;

            if (Settings.Current.Website == "K")
            {
                MinPost = Settings.Current.LastPostK;
            }
            else
            {
                MinPost = Settings.Current.LastPostY;
            }

            if (list != null)
            {

                foreach (var item in list)
                {
                    postsViewer.AddPost(item);
                }
            }
            return true;
        }

        List<MoePost> list;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StatusBarView view = new StatusBarView(MoePic.Models.Settings.Current.WebSiteLogo, "What's New!", false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            base.OnNavigatedTo(e);
        }

        private async System.Threading.Tasks.Task<bool> postsViewer_RequestLoadData(object sender, EventArgs e)
        {
            List<MoePost> postList = await MoebooruAPI.GetPostsFromMin(MinPost, page++, Settings.Current.Limit, Settings.Current.Rating);
            if (postList.Count > 0)
            {
                foreach (var item in postList)
                {
                    postsViewer.AddPost(item);
                }
            }
            return true;
        }
    }
}