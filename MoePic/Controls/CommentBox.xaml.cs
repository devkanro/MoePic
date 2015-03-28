using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using MoePic.Models;

namespace MoePic.Controls
{
    public partial class CommentBox : UserControl
    {
        public CommentBox()
        {
            InitializeComponent();
        }

        public MoeComment Comment
        {
            get { return (MoeComment)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register("Comment", typeof(MoeComment), typeof(CommentBox), new PropertyMetadata(CommentChanged));

        public static void CommentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CommentBox).avatar.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(MoebooruAPI.GetUserAvatar((int)(e.NewValue as MoeComment).creator_id,MoebooruAPI.partWebsite)));
            (d as CommentBox).id.Text = (e.NewValue as MoeComment).creator.ToUpper();
            String text;
            Regex reg = new Regex(@"(?:\[quote\](.*)\\r\\n\[/quote\]\\r\\n\\r\\n)?(.*)");
            Match m = reg.Match(text = (e.NewValue as MoeComment).body.Replace("\r\n", @"\r\n"));
            if(m.Groups[1].Value != "")
            {
                (d as CommentBox).quoteBox.Visibility = Visibility.Visible;
                (d as CommentBox).quote.Text = m.Groups[1].Value.Replace(@"\r\n", "\r\n");
            }
            else
            {
                (d as CommentBox).quoteBox.Visibility = Visibility.Collapsed;
            }
            (d as CommentBox).content.Text = m.Groups[2].Value.Replace(@"\r\n", "\r\n");
            (d as CommentBox).date.Text = (e.NewValue as MoeComment).created_at.ToString("M/d,H:m");
        }
    }
}
