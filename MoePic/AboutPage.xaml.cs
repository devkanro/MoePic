using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Marketplace;
using MoePic.Models;
using MoePic.Resources;

namespace MoePic
{
    public partial class AboutPage : MoePicPage
    {
        bool IsTrial;
        public AboutPage()
        {
            IsTrial = (App.Current as App).IsTrial;
            PulsColor1 = 0;
            PulsColor2 = 90;
            InitializeComponent();
            
            LogoAnime.Begin();
        }

        public double PulsColor1
        {
            get { return (double)GetValue(PulsColor1Property); }
            set { SetValue(PulsColor1Property, value); }
        }

        // Using a DependencyProperty as the backing store for PulsColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PulsColor1Property =
            DependencyProperty.Register("PulsColor1", typeof(double), typeof(AboutPage),null);

        static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public double PulsColor2
        {
            get { return (double)GetValue(PulsColor2Property); }
            set { SetValue(PulsColor2Property, value); }
        }

        // Using a DependencyProperty as the backing store for PulsColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PulsColor2Property =
            DependencyProperty.Register("PulsColor2", typeof(double), typeof(AboutPage), null);

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Controls.Logos.MoePic, AppResources.About, false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
#if PREVIEW 
                VersionText.Text = String.Format("Ver.0.4.2.0 {0}", "Preview!");
#else
            if ((App.Current as App).IsTrial)
            {
                VersionText.Text = String.Format("Ver.{1} {0}", AppResources.Trial, App.GetVersion());
                
            }
            else
            {
                VersionText.Text = String.Format("Ver.{0} Plus!", App.GetVersion());
                getLongTrial.Visibility = System.Windows.Visibility.Collapsed;
                ColorAnime();
                PulsAnime.Begin();
            }
#endif

            base.OnNavigatedTo(e);
        }



        public void ColorAnime()
        {
            Storyboard sb1 = new Storyboard();
            DoubleAnimation da1 = new DoubleAnimation();
            Storyboard.SetTarget(da1, this);
            Storyboard.SetTargetProperty(da1, new PropertyPath(AboutPage.PulsColor1Property));
            da1.From = 30;
            da1.To = 390;
            da1.Duration = new Duration(new TimeSpan(0, 0, 25));
            sb1.Children.Add(da1);

            Storyboard sb2 = new Storyboard();
            DoubleAnimation da2 = new DoubleAnimation();
            Storyboard.SetTarget(da2, this);
            Storyboard.SetTargetProperty(da2, new PropertyPath(AboutPage.PulsColor2Property));
            da2.From = 0;
            da2.To = 360;
            da2.Duration = new Duration(new TimeSpan(0, 0, 25));
            sb2.Children.Add(da2);

            sb1.RepeatBehavior = RepeatBehavior.Forever;
            sb2.RepeatBehavior = RepeatBehavior.Forever;

            sb1.Begin();
            sb2.Begin();
        }

        int TapCount = 0;

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(++TapCount == 10)
            {
                Settings.Current.IsExAble = true;
                ToastService.Show("已解锁新分级.");
                TapCount = 0;
            }
        }

        private void rectangle1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AchievementService.Test();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.MarketplaceReviewTask task = new Microsoft.Phone.Tasks.MarketplaceReviewTask();
            task.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri("http://www.weibo.com/u/5318360558");
            task.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri("http://www.higan.me");
            task.Show();
        }

        private void getLongTrial_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask task = new Microsoft.Phone.Tasks.EmailComposeTask()
            {
                Body = 
                "您可以申请延长MoePic的试用期,但是您需要填写以下内容:\r\n"+
                "微软帐号:\r\n"+
                "用户ID:" + (App.Current.Resources["RuntimeResources"] as RuntimeResources).UserID +
                "\r\n申请理由:\r\n\r\n我们将会尽快处理您的申请,并答复您.",
                To = "higan@live.cn",
                Subject = "MoePic试用期延长申请"
            };
            task.Show();
        }

    }

    public class HuesToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte r = 0, g = 0, b = 0;
            string[] args;
            if(parameter == null)
            {
                args = new string[] { "1", "1" };
            }
            else
            {
                args = (parameter as string).Split(',');
            }
            
            double hues = ((double)value) % 360;
            return HSBColor.HSBtoRGB(hues, double.Parse(args[0]), double.Parse(args[1]), out r, out g, out b);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double h = 0, s = 0, b = 0;
            return HSBColor.RGBtoHSB(((Color)value).R, ((Color)value).G, ((Color)value).B, out h, out s, out b).Hues;
        }
    }
}