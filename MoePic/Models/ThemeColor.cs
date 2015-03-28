using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MoePic.Models
{
    public class ThemeColor : DependencyObject
    {
        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(ThemeColor), null);



        public SolidColorBrush NavigateBar
        {
            get { return (SolidColorBrush)GetValue(NavigateBarProperty); }
            set { SetValue(NavigateBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigateBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigateBarProperty =
            DependencyProperty.Register("NavigateBar", typeof(SolidColorBrush), typeof(ThemeColor), null);




        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(ThemeColor), null);




        public SolidColorBrush Text
        {
            get { return (SolidColorBrush)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(SolidColorBrush), typeof(ThemeColor), null);




        public SolidColorBrush Text2
        {
            get { return (SolidColorBrush)GetValue(Text2Property); }
            set { SetValue(Text2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Text2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Text2Property =
            DependencyProperty.Register("Text2", typeof(SolidColorBrush), typeof(ThemeColor), null);

        

        public SolidColorBrush StatusBar
        {
            get { return (SolidColorBrush)GetValue(StatusBarProperty); }
            set { SetValue(StatusBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarProperty =
            DependencyProperty.Register("StatusBar", typeof(SolidColorBrush), typeof(ThemeColor), null);

        public ThemeColorsEnum Color { get; set; }

        public void Init()
        {
			Foreground = new SolidColorBrush(ThemeColors.Cyan.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Cyan.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Cyan.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Cyan.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Cyan.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Cyan.Text2);
        }

        public void Init(ThemeColorsEnum color)
        {
            switch (color)
            {
                case ThemeColorsEnum.Cyan:
                    Foreground = new SolidColorBrush(ThemeColors.Cyan.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Cyan.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Cyan.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Cyan.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Cyan.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Cyan.Text2);
                    break;
                case ThemeColorsEnum.Pink:
                    Foreground = new SolidColorBrush(ThemeColors.Pink.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Pink.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Pink.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Pink.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Pink.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Pink.Text2);
                    break;
                case ThemeColorsEnum.Red:
                    Foreground = new SolidColorBrush(ThemeColors.Red.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Red.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Red.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Red.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Red.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Red.Text2);
                    break;
                case ThemeColorsEnum.Green:
                    Foreground = new SolidColorBrush(ThemeColors.Green.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Green.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Green.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Green.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Green.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Green.Text2);
                    break;
                case ThemeColorsEnum.Orange:
                    Foreground = new SolidColorBrush(ThemeColors.Orange.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Orange.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Orange.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Orange.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Orange.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Orange.Text2);
                    break;
                case ThemeColorsEnum.Violet:
                    Foreground = new SolidColorBrush(ThemeColors.Violet.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Violet.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Violet.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Violet.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Violet.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Violet.Text2);
                    break;
                case ThemeColorsEnum.Blue:
                    Foreground = new SolidColorBrush(ThemeColors.Blue.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Blue.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Blue.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Blue.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Blue.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Blue.Text2);
                    break;
                case ThemeColorsEnum.Grey:
                    Foreground = new SolidColorBrush(ThemeColors.Grey.Foreground);
                    Background = new SolidColorBrush(ThemeColors.Grey.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.Grey.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.Grey.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.Grey.Text);
                    Text2 = new SolidColorBrush(ThemeColors.Grey.Text2);
                    break;
                case ThemeColorsEnum.BlackishGreen:
                    Foreground = new SolidColorBrush(ThemeColors.BlackishGreen.Foreground);
                    Background = new SolidColorBrush(ThemeColors.BlackishGreen.Background);
                    StatusBar = new SolidColorBrush(ThemeColors.BlackishGreen.StatusBar);
                    NavigateBar = new SolidColorBrush(ThemeColors.BlackishGreen.NavigateBar);
                    Text = new SolidColorBrush(ThemeColors.BlackishGreen.Text);
                    Text2 = new SolidColorBrush(ThemeColors.BlackishGreen.Text2);
                    break;
            }
        }

        public void SetColor(ThemeColorsEnum color)
        {
            if(color != Color)
            {
                Color = color;
                ColorAnimation ForegroundAnime = new ColorAnimation();
                ColorAnimation BackgroundAnime = new ColorAnimation();
                ColorAnimation StatusBarAnime = new ColorAnimation();
                ColorAnimation NavigateBarAnime = new ColorAnimation();
                ColorAnimation TextAnime = new ColorAnimation();
                ColorAnimation Text2Anime = new ColorAnimation();

                ForegroundAnime.From = Foreground.Color;
                BackgroundAnime.From = Background.Color;
                StatusBarAnime.From = StatusBar.Color;
                NavigateBarAnime.From = NavigateBar.Color;
                TextAnime.From = Text.Color;
                Text2Anime.From = Text2.Color;

                ForegroundAnime.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                BackgroundAnime.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                StatusBarAnime.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                NavigateBarAnime.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                TextAnime.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                Text2Anime.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));

                ForegroundAnime.BeginTime = new TimeSpan(0);
                BackgroundAnime.BeginTime = new TimeSpan(0);
                StatusBarAnime.BeginTime = new TimeSpan(0);
                NavigateBarAnime.BeginTime = new TimeSpan(0);
                TextAnime.BeginTime = new TimeSpan(0);
                Text2Anime.BeginTime = new TimeSpan(0);

                Storyboard.SetTarget(ForegroundAnime, Foreground);
                Storyboard.SetTarget(BackgroundAnime, Background);
                Storyboard.SetTarget(StatusBarAnime, StatusBar);
                Storyboard.SetTarget(NavigateBarAnime, NavigateBar);
                Storyboard.SetTarget(TextAnime, Text);
                Storyboard.SetTarget(Text2Anime, Text2);

                Storyboard.SetTargetProperty(ForegroundAnime, new PropertyPath(SolidColorBrush.ColorProperty));
                Storyboard.SetTargetProperty(BackgroundAnime, new PropertyPath(SolidColorBrush.ColorProperty));
                Storyboard.SetTargetProperty(StatusBarAnime, new PropertyPath(SolidColorBrush.ColorProperty));
                Storyboard.SetTargetProperty(NavigateBarAnime, new PropertyPath(SolidColorBrush.ColorProperty));
                Storyboard.SetTargetProperty(TextAnime, new PropertyPath(SolidColorBrush.ColorProperty));
                Storyboard.SetTargetProperty(Text2Anime, new PropertyPath(SolidColorBrush.ColorProperty));

                Storyboard ColorAnime = new Storyboard();

                switch (Color)
                {
                    case ThemeColorsEnum.Cyan:
                    ForegroundAnime.To = ThemeColors.Cyan.Foreground;
                    BackgroundAnime.To = ThemeColors.Cyan.Background;
                    StatusBarAnime.To = ThemeColors.Cyan.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Cyan.NavigateBar;
                    TextAnime.To = ThemeColors.Cyan.Text;
                    Text2Anime.To = ThemeColors.Cyan.Text2;
                    break;
                case ThemeColorsEnum.Pink:
                    ForegroundAnime.To = ThemeColors.Pink.Foreground;
                    BackgroundAnime.To = ThemeColors.Pink.Background;
                    StatusBarAnime.To = ThemeColors.Pink.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Pink.NavigateBar;
                    TextAnime.To = ThemeColors.Pink.Text;
                    Text2Anime.To = ThemeColors.Pink.Text2;
                    break;
                case ThemeColorsEnum.Red:
                    ForegroundAnime.To = ThemeColors.Red.Foreground;
                    BackgroundAnime.To = ThemeColors.Red.Background;
                    StatusBarAnime.To = ThemeColors.Red.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Red.NavigateBar;
                    TextAnime.To = ThemeColors.Red.Text;
                    Text2Anime.To = ThemeColors.Red.Text2;
                    break;
                case ThemeColorsEnum.Green:
                    ForegroundAnime.To = ThemeColors.Green.Foreground;
                    BackgroundAnime.To = ThemeColors.Green.Background;
                    StatusBarAnime.To = ThemeColors.Green.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Green.NavigateBar;
                    TextAnime.To = ThemeColors.Green.Text;
                    Text2Anime.To = ThemeColors.Green.Text2;
                    break;
                case ThemeColorsEnum.Orange:
                    ForegroundAnime.To = ThemeColors.Orange.Foreground;
                    BackgroundAnime.To = ThemeColors.Orange.Background;
                    StatusBarAnime.To = ThemeColors.Orange.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Orange.NavigateBar;
                    TextAnime.To = ThemeColors.Orange.Text;
                    Text2Anime.To = ThemeColors.Orange.Text2;
                    break;
                case ThemeColorsEnum.Violet:
                    ForegroundAnime.To = ThemeColors.Violet.Foreground;
                    BackgroundAnime.To = ThemeColors.Violet.Background;
                    StatusBarAnime.To = ThemeColors.Violet.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Violet.NavigateBar;
                    TextAnime.To = ThemeColors.Violet.Text;
                    Text2Anime.To = ThemeColors.Violet.Text2;
                    break;
                case ThemeColorsEnum.Blue:
                    ForegroundAnime.To = ThemeColors.Blue.Foreground;
                    BackgroundAnime.To = ThemeColors.Blue.Background;
                    StatusBarAnime.To = ThemeColors.Blue.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Blue.NavigateBar;
                    TextAnime.To = ThemeColors.Blue.Text;
                    Text2Anime.To = ThemeColors.Blue.Text2;
                    break;
                case ThemeColorsEnum.Grey:
                    ForegroundAnime.To = ThemeColors.Grey.Foreground;
                    BackgroundAnime.To = ThemeColors.Grey.Background;
                    StatusBarAnime.To = ThemeColors.Grey.StatusBar;
                    NavigateBarAnime.To = ThemeColors.Grey.NavigateBar;
                    TextAnime.To = ThemeColors.Grey.Text;
                    Text2Anime.To = ThemeColors.Grey.Text2;
                    break;
                case ThemeColorsEnum.BlackishGreen:
                    ForegroundAnime.To = ThemeColors.BlackishGreen.Foreground;
                    BackgroundAnime.To = ThemeColors.BlackishGreen.Background;
                    StatusBarAnime.To = ThemeColors.BlackishGreen.StatusBar;
                    NavigateBarAnime.To = ThemeColors.BlackishGreen.NavigateBar;
                    TextAnime.To = ThemeColors.BlackishGreen.Text;
                    Text2Anime.To = ThemeColors.BlackishGreen.Text2;
                    break;
                }


                ColorAnime.Children.Add(ForegroundAnime);
                ColorAnime.Children.Add(BackgroundAnime);
                ColorAnime.Children.Add(StatusBarAnime);
                ColorAnime.Children.Add(NavigateBarAnime);
                ColorAnime.Children.Add(TextAnime);
                ColorAnime.Children.Add(Text2Anime);

                ColorAnime.Begin();
            }

        }

        public ThemeColor()
        {
            Init();
        }
    }

    public static class ThemeColors
    {
        /// <summary>
        /// 青色(默认)
        /// </summary>
        public static class Cyan
        {
            public static Color Background 
            { 
                get
                {
                    return Color.FromArgb(0xFF, 0xE6, 0xF2, 0xF7);
                }
            }

            public static Color Foreground
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x3F, 0x84, 0xB5);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x00, 0x85, 0xB5);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xC0, 0xDE, 0xED);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x58, 0xAC, 0xED);
                }
            }
        }

        /// <summary>
        /// 粉色
        /// </summary>
        public static class Pink
        {
            public static Color Background
            {
                get
                {
                    //FFE2FF
                    return Color.FromArgb(0xFF, 0xFF, 0xE2, 0xFF);
                }
            }

            public static Color Foreground
            {
                get
                {
                    //CC6E9A
                    return Color.FromArgb(0xFF, 0xCC, 0x6E, 0x9A);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    //E985B5
                    return Color.FromArgb(0xFF, 0xE9, 0x85, 0xB5);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    //EABCE5
                    return Color.FromArgb(0xFF, 0xEA, 0xBC, 0xE5);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    //D567B8
                    return Color.FromArgb(0xFF, 0xD5, 0x67, 0xB8);
                }
            }
        }

        /// <summary>
        /// 红色
        /// </summary>
        public static class Red
        {
            public static Color Background
            {
                get
                {
                    //FFF4F4
                    return Color.FromArgb(0xFF, 0xFF, 0xF4, 0xF4);
                }
            }

            public static Color Foreground
            {
                get
                {
                    //CC4343
                    return Color.FromArgb(0xFF, 0xCC, 0x43, 0x43);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    //E93D3D
                    return Color.FromArgb(0xFF, 0xE9, 0x3D, 0x3D);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    //EA9292
                    return Color.FromArgb(0xFF, 0xEA, 0x92, 0x92);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    //D54E4E
                    return Color.FromArgb(0xFF, 0xD5, 0x4E, 0x4E);
                }
            }
        }

        /// <summary>
        /// 绿色
        /// </summary>
        public static class Green
        {
            public static Color Background
            {
                get
                {
                    //F3FFF4
                    return Color.FromArgb(0xFF, 0xF3, 0xFF, 0xF4);
                }
            }

            public static Color Foreground
            {
                get
                {
                    //6ADA4E
                    return Color.FromArgb(0xFF, 0x6A, 0xDA, 0x4E);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    //1A965E
                    return Color.FromArgb(0xFF, 0x1A, 0x96, 0x5E);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    //BDEFB9
                    return Color.FromArgb(0xFF, 0xBD, 0xEF, 0xB9);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    //6ADA4E
                    return Color.FromArgb(0xFF, 0x6A, 0xDA, 0x4E);
                }
            }
        }

        /// <summary>
        /// 橙色
        /// </summary>
        public static class Orange
        {
            public static Color Background
            {
                get
                {
                    //FFFFF4
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xF4);
                }
            }

            public static Color Foreground
            {
                get
                {
                    //EE9005
                    return Color.FromArgb(0xFF, 0xEE, 0x90, 0x05);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    //EDA424
                    return Color.FromArgb(0xFF, 0xED, 0xA4, 0x24);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    //FFEAB2
                    return Color.FromArgb(0xFF, 0xFF, 0xEA, 0xB2);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    //FFA34E
                    return Color.FromArgb(0xFF, 0xFF, 0xA3, 0x4E);
                }
            }
        }

        /// <summary>
        /// 紫色
        /// </summary>
        public static class Violet
        {
            public static Color Background
            {
                get
                {
                    //EAF7FF
                    return Color.FromArgb(0xFF, 0xEA, 0xF7, 0xFF);
                }
            }

            public static Color Foreground
            {
                get
                {
                    //8D4AE0
                    return Color.FromArgb(0xFF, 0x8D, 0x4A, 0xE0);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    //613CE1
                    return Color.FromArgb(0xFF, 0x61, 0x3C, 0xE1);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    //B499FF
                    return Color.FromArgb(0xFF, 0xB4, 0x99, 0xFF);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    //7453D8
                    return Color.FromArgb(0xFF, 0x74, 0x53, 0xD8);
                }
            }
        }

        /// <summary>
        /// 蓝色
        /// </summary>
        public static class Blue
        {
            public static Color Background
            {
                get
                {
                    //E7FAFF
                    return Color.FromArgb(0xFF, 0xE7, 0xFA, 0xFF);
                }
            }

            public static Color Foreground
            {
                get
                {
                    //4A6AE0
                    return Color.FromArgb(0xFF, 0x4A, 0x6A, 0xE0);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    //2966E1
                    return Color.FromArgb(0xFF, 0x29, 0x66, 0xE1);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    //83C7FF
                    return Color.FromArgb(0xFF, 0x83, 0xC7, 0xFF);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    //2374D8
                    return Color.FromArgb(0xFF, 0x23, 0x74, 0xD8);
                }
            }
        }

        /// <summary>
        /// 灰色
        /// </summary>
        public static class Grey
        {
            public static Color Background
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Foreground
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x4B, 0x4B, 0x4B);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x68, 0x68, 0x68);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x76, 0x76, 0x76);
                }
            }
        }

        /// <summary>
        /// 墨绿色
        /// </summary>
        public static class BlackishGreen
        {
            public static Color Background
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Foreground
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x00, 0x4B, 0x4B);
                }
            }

            public static Color StatusBar
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x00, 0x68, 0x68);
                }
            }

            public static Color NavigateBar
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xCC, 0xE5, 0xE5);
                }
            }

            public static Color Text
            {
                get
                {
                    return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }

            public static Color Text2
            {
                get
                {
                    return Color.FromArgb(0xFF, 0x00, 0x76, 0x76);
                }
            }
        }
    }

    public enum ThemeColorsEnum
    {
        Cyan,
        Pink,
        Red,
        Green,
        Orange,
        Violet,
        Blue,
        Grey,
        BlackishGreen
    }
}
