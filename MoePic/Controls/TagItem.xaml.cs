using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Text.RegularExpressions;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Globalization;

using MoePic.Models;

namespace MoePic.Controls
{
    public partial class TagItem
    {
        public TagItem()
        {
            TagColor = new SolidColorBrush(Colors.DarkGray);
            InitializeComponent();
        }

        public TagItem(String tag)
        {
            tagName = tag;
            TagColor = new SolidColorBrush(Colors.DarkGray);
            InitializeComponent();
        }

        public TagItem(String tag ,PostViewer viewer)
        {
            this.viewer = viewer;
            tagName = tag;
            TagColor = new SolidColorBrush(Colors.DarkGray);
            InitializeComponent();
        }

        public TagItem(MoeTag tag, PostViewer viewer)
        {
            this.viewer = viewer;
            Tag = tag;
            tagName = tag.name;
            TagLoaded = true;
            TagLoad = true;
            TagColor = new SolidColorBrush(GetColorFormTagType((TagType)tag.type));
            InitializeComponent();
        }

        PostViewer viewer;
        string tagName;

        public new MoeTag Tag
        {
            get { return (MoeTag)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }

        public new static readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(MoeTag), typeof(TagItem), null);


        public SolidColorBrush TagColor
        {
            get { return (SolidColorBrush)GetValue(TagColorProperty); }
            set { SetValue(TagColorProperty, value); }
        }

        public static readonly DependencyProperty TagColorProperty =
            DependencyProperty.Register("TagColor", typeof(SolidColorBrush), typeof(TagItem), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));



        Point DragStartPoint;
        Thickness OldMargin;

        private void GestureListener_DragStarted(object sender, DragStartedGestureEventArgs e)
        {
            e.Handled = true;
            bool ok = this.CaptureMouse();
            if (viewer != null)
            {
                Canvas.SetZIndex(this, viewer.zindex++);
            }
            DragStartPoint = e.GetPosition(this.Parent as UIElement);
            OldMargin = Margin;
            
        }

        private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            e.Handled = true;
            if (IsButtonsOut)
            {
                IsButtonsOut = false;
                ButtonsIn.Begin();
            }
            if (IsButtonShow)
            {
                IsButtonShow = false;
                HideButton.Begin();
            }
            Point point = e.GetPosition(this.Parent as UIElement);
            double x = DragStartPoint.X - point.X;
            double y = DragStartPoint.Y - point.Y;

            x = x + OldMargin.Right;
            y = y + OldMargin.Bottom;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            Margin = new Thickness(0, 0, x + ActualWidth > (this.Parent as FrameworkElement).ActualWidth ? (this.Parent as FrameworkElement).ActualWidth - ActualWidth : x, y + ActualHeight > (this.Parent as FrameworkElement).ActualHeight ? (this.Parent as FrameworkElement).ActualHeight - ActualHeight : y);
        }

        private void GestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            e.Handled = true;
            this.ReleaseMouseCapture();
        }
        bool IsButtonsOut = false;
        private void colorfulButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsButtonsOut)
            {
                IsButtonsOut = false;
                ButtonsIn.Begin();
            }
            else
            {
                IsButtonsOut = true;
                ButtonsOut.Begin();
            }
        }

        bool IsButtonShow = false;


        bool firstload = true;

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        static Color GetColorFormTagType(TagType type)
        {
            switch (type)
            {
                case TagType.General://#FF5809
                    return Color.FromArgb(0xFF, 0xFF, 0x58, 0x09);
                case TagType.artist://#FFD306
                    return Color.FromArgb(0xFF, 0xFF, 0xD3, 0x06);
                case TagType.copyright://#9F35FF
                    return Color.FromArgb(0xFF, 0x9F, 0x35, 0xFF);
                case TagType.character://#8CEA00
                    return Color.FromArgb(0xFF, 0x8C, 0xEA, 0x00);
                case TagType.circle://#46A3FF
                    return Color.FromArgb(0xFF, 0x46, 0xA3, 0xFF);
                case TagType.faults://#EA0000
                    return Color.FromArgb(0xFF, 0xEA, 0x00, 0x00);
            }
            return Colors.DarkGray;
        }

        public void SetTagColorNoAnime(Color color)
        {
            TagColor.Color = color;
        }

        public void SetTagColor(Color color)
        {
            Storyboard sb = new Storyboard();
            ColorAnimation colorAnime = new ColorAnimation();
            Storyboard.SetTarget(colorAnime, TagColor);
            Storyboard.SetTargetProperty(colorAnime, new PropertyPath(SolidColorBrush.ColorProperty));
            colorAnime.From = TagColor.Color;
            colorAnime.To = color;
            colorAnime.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            sb.Children.Add(colorAnime);
            sb.Begin();
        }

        bool TagLoaded = false;

        public async void LoadTag()
        {
            if (!TagLoaded)
            {
                TagLoaded = true;
                int page = 1;
            Reload:
                List<MoeTag> tagList = await MoebooruAPI.GetTagsFormName(tagName, 0, page++, TagOrder.name, MoebooruAPI.partWebsite);
                Tag = tagList.Find((MoeTag item) =>
                {
                    if (item.name == tagName)
                    {
                        return true;
                    }
                    return false;
                });
                if (Tag == null)
                {
                    if (tagList.Count != 0)
                    {
                        goto Reload;
                    }
                    else
                    {
                        return;
                    }
                }
                SetTagColor(GetColorFormTagType((TagType)Tag.type));
                if (IsFav = FavoriteHelp.ContainsTag(Tag))
                {
                    IsFav = true;
                    colorfulButton2.ContentBrush = new System.Windows.Media.ImageBrush() { ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Icons/favs.png", UriKind.Relative)) };
                }
                else
                {
                    IsFav = false; colorfulButton2.ContentBrush = new System.Windows.Media.ImageBrush() { ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Icons/favs.addto.png", UriKind.Relative)) };
                }
                TagLoad = true;

            }
        }

        bool IsFav = false;

        private void TagName_Loaded(object sender, RoutedEventArgs e)
        {
            TagName.Text = GetTagText(tagName);
        }

        public static String GetTagText(string TagName)
        {
            return Regex.Replace(TagName.Replace('_', ' '), "\\w+", match => match.Value[0].ToString().ToUpper() + match.Value.Substring(1));
        }

        private void TagText_Loaded(object sender, RoutedEventArgs e)
        {
            TagText.Text = GetTagText(tagName);
        }

        private void LayoutRoot_LayoutUpdated(object sender, EventArgs e)
        {
            if (firstload && tagContent.ActualWidth != 0 && tagContent.ActualWidth != 30)
            {

                LayoutRoot.Width = tagContent.ActualWidth + 8;
                ShowButtonKeyFrame1.Value = LayoutRoot.Width;
                ShowButtonKeyFrame2.Value = LayoutRoot.Width + 30;
                HideButtonKeyFrame2.Value = LayoutRoot.Width;
                HideButtonKeyFrame1.Value = LayoutRoot.Width + 30;

                double x = 0, y = 0;
                x = x + Margin.Right;
                y = y + Margin.Bottom;

                x = x < 0 ? 0 : x;
                y = y < 0 ? 0 : y;

                Margin = new Thickness(0, 0, x + LayoutRoot.Width > (this.Parent as FrameworkElement).ActualWidth ? (this.Parent as FrameworkElement).ActualWidth - LayoutRoot.Width : x, y + LayoutRoot.Height > (this.Parent as FrameworkElement).ActualHeight ? (this.Parent as FrameworkElement).ActualHeight - LayoutRoot.Height : y);


                TagItemLoaded.Begin();
                if(!Settings.Current.AutoHideTags)
                {
                    LoadTag();
                }

                firstload = false;
            }

        }

        bool TagLoad = false;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TagLoad)
            {
                if (viewer.TagMoede != 'D')
                {
                    if (IsButtonShow)
                    {
                        if (IsButtonsOut)
                        {
                            IsButtonsOut = false;
                            ButtonsIn.Begin();
                        }
                        IsButtonShow = false;
                        HideButton.Begin();
                    }
                    else
                    {
                        IsButtonShow = true;
                        ShowButton.Begin();
                    }
                }
                else
                {
                    TagHide.Begin();
                }
            }

        }

        private void colorfulButton1_Click(object sender, RoutedEventArgs e)
        {
            MoePic.Models.NavigationService.Navigate("SearchPage.xaml", Tag.name);
        }

        private void Button_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void colorfulButton4_Click(object sender, RoutedEventArgs e)
        {
            TagHide.Begin();
        }

        private void TagHide_Completed(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void colorfulButton2_Click(object sender, RoutedEventArgs e)
        {
            if(IsFav)
            {
                colorfulButton2.ContentBrush = new System.Windows.Media.ImageBrush() { ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Icons/favs.addto.png", UriKind.Relative)) };
                IsFav = !FavoriteHelp.DelTag(Tag);
                ToastService.Show(String.Format(MoePic.Resources.AppResources.DelTagFav,GetTagText(Tag.name)));
            }
            else
            {
                IsFav = FavoriteHelp.AddTag(Tag);
                ToastService.Show(String.Format(MoePic.Resources.AppResources.AddTagFav, GetTagText(Tag.name)));
                colorfulButton2.ContentBrush = new System.Windows.Media.ImageBrush() { ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Icons/favs.png", UriKind.Relative)) };

            }
        }
    }
}
