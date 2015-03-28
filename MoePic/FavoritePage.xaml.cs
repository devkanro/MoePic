using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoePic.Controls;
namespace MoePic
{
    public partial class FavoritePage : Models.MoePicPage
    {
        public FavoritePage()
        {
            InitializeComponent();
            FavPostList.Source = MoePic.Models.FavoriteHelp.FavoriteList;
            tagList.ItemsSource = MoePic.Models.FavoriteHelp.FavoriteTagList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Controls.Logos.Favorite, MoePic.Resources.AppResources.Fav, false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            TagListItem.TagListItemClick += TagListItem_TagListItemClick;
            base.OnNavigatedTo(e);
        }

        void TagListItem_TagListItemClick(object sender, TagListItemClickEventArgs e)
        {
            try
            {
                var kv = searchTag.Children.First((k) => Models.TagComparer.EqualsStatic((k as TagSearchItem).Tag, e.Tag));
                if ((kv as TagSearchItem).TagLogic != e.Logic)
                {
                    searchTag.Children.Remove(kv);
                    var item = new TagSearchItem() { Tag = e.Tag, TagLogic = e.Logic };
                    item.Click += item_Click;
                    searchTag.Children.Add(item);
                }
            }
            catch (Exception)
            {
                var item = new TagSearchItem() { Tag = e.Tag, TagLogic = e.Logic };
                item.Click += item_Click;
                searchTag.Children.Add(item);
            }
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            searchTag.Children.Remove(sender as TagSearchItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String searchText = "";
            foreach (var item in searchTag.Children)
            {
                switch ((item as TagSearchItem).TagLogic)
                {
                    case TagLogic.And:
                        searchText += String.Format("{0}+", (item as TagSearchItem).Tag.name);
                        break;
                    case TagLogic.Or:
                        searchText += String.Format("~{0}+", (item as TagSearchItem).Tag.name);
                        break;
                    case TagLogic.Not:
                        searchText += String.Format("-{0}+", (item as TagSearchItem).Tag.name);
                        break;
                    default:
                        break;
                }
            }
            if(searchText != "")
            {
                MoePic.Models.NavigationService.Navigate("SearchPage.xaml", searchText);
            }
            else
            {
                Models.ToastService.Show(MoePic.Resources.AppResources.NoTagsSelect);
            }
        }


    }
}