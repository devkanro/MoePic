using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoePic.Models;

namespace MoePic.Controls
{
    public partial class TagSearchItem : UserControl
    {
        public TagSearchItem()
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
            DependencyProperty.Register("Tag", typeof(MoeTag), typeof(TagSearchItem), new PropertyMetadata(TagChanged));



        public TagLogic TagLogic
        {
            get { return (TagLogic)GetValue(TagLogicProperty); }
            set { SetValue(TagLogicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TagLogic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TagLogicProperty =
            DependencyProperty.Register("TagLogic", typeof(TagLogic), typeof(TagSearchItem), new PropertyMetadata(TagChanged));

        public static String GetTagText(string TagName)
        {
            return Regex.Replace(TagName.Replace('_', ' '), "\\w+", match => match.Value[0].ToString().ToUpper() + match.Value.Substring(1));
        }

        public static void TagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as TagSearchItem).Tag != null)
            {
                switch ((d as TagSearchItem).TagLogic)
                {
                    case TagLogic.And:
                        (d as TagSearchItem).tagText.Text = String.Format("+{0}", GetTagText((d as TagSearchItem).Tag.name));
                        break;
                    case TagLogic.Or:
                        (d as TagSearchItem).tagText.Text = String.Format("?{0}", GetTagText((d as TagSearchItem).Tag.name));
                        break;
                    case TagLogic.Not:
                        (d as TagSearchItem).tagText.Text = String.Format("-{0}", GetTagText((d as TagSearchItem).Tag.name));
                        break;
                    default:
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Click != null)
            {
                Click(this, e);
            }
        }

        public event RoutedEventHandler Click;
    }

    public enum TagLogic
    {
        And,
        Or,
        Not
    }
}
