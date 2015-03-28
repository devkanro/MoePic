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
    public partial class TagListItem : ListBoxItem
    {
        public TagListItem()
        {
            InitializeComponent();
        }




        public new MoeTag Tag
        {
            get { return (MoeTag)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tag.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(MoeTag), typeof(TagListItem), null);

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(TagListItemClick != null)
            {
                TagListItemClick(this, new TagListItemClickEventArgs(Tag, TagLogic.Or));
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (TagListItemClick != null)
            {
                TagListItemClick(this, new TagListItemClickEventArgs(Tag, TagLogic.Not));
            }
        }

        private void listBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (TagListItemClick != null)
            {
                TagListItemClick(this, new TagListItemClickEventArgs(Tag, TagLogic.And));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToastService.Show(String.Format("已从收藏列表移除{0}.", TagItem.GetTagText(Tag.name)));
            FavoriteHelp.DelTag(Tag);
        }

        public static event EventHandler<TagListItemClickEventArgs> TagListItemClick; 
        
    }

    public class TagListItemClickEventArgs:EventArgs
    {
        public MoeTag Tag { get; set; }
        public TagLogic Logic { get; set; }

        public TagListItemClickEventArgs(MoeTag tag,TagLogic logic)
        {
            Tag = tag;
            Logic = logic;
        }
    }
}
