using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Controls
{
    public partial class PostItem : UserControl
    {
        bool IsWaterFall;
        public PostItem()
        {
            IsWaterFall = true;
            InitializeComponent();
        }

        public PostItem(bool isWaterFall = true)
        {
            IsWaterFall = isWaterFall;
            InitializeComponent();
            if (!IsWaterFall)
            {
                Width = 150;
                Height = 150;
                Margin = new Thickness(5);
            }
        }

        public MoePic.Models.MoePost Post
        {
            get { return (MoePic.Models.MoePost)GetValue(PostProperty); }
            set
            {

                SetValue(PostProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Post.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostProperty =
            DependencyProperty.Register("Post", typeof(MoePic.Models.MoePost), typeof(PostItem), new PropertyMetadata(null, PostChanged));

        public static void PostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && (d as PostItem).IsWaterFall)
            {
                (d as PostItem).Height = 1.0 * 237 / (e.NewValue as MoePic.Models.MoePost).actual_preview_width * (e.NewValue as MoePic.Models.MoePost).actual_preview_height;
            }
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            ImageOpend.Begin();
        }

        private void LayoutRoot_Click(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("PostViewPage.xaml", Post);
        }
    }
}
