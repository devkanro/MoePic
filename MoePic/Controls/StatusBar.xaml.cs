using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Controls
{
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
            block.Height = MoePic.Models.DisplaySize.Current.Height - 117;
            StatusBarView = new Controls.StatusBarView(MoePic.Models.Settings.Current.WebSiteLogo, MoePic.Models.Settings.Current.WebSiteLogo == Controls.Logos.Yandere ? "yande.re" : "Konachan", true, true);
            SetView(StatusBarView);
        }

        public StatusBar(StatusBarView view)
        {
            InitializeComponent();
            block.Height = MoePic.Models.DisplaySize.Current.Height - 117;
            StatusBarView = view;
            SetView(StatusBarView);
        }

        public StatusBarView StatusBarView { get; set; }
        public StatusBarView NewStatusBarView { get; set; }

        Queue<Storyboard> AnimeQueue = new Queue<Storyboard>();

        void SetView(StatusBarView view)
        {
            SetLogo(view.Logo);
            TilteText.Text = view.Tilte;
            if (view.ButtonShow)
            {
                breakTran.TranslateY = buttonTran.TranslateY = 0;
            }
            else
            {
                breakTran.TranslateY = buttonTran.TranslateY = -50;
            }
        }

        public void Change(Logos logo, String tilte, bool buttonShow,bool userShow)
        {
            Change(new StatusBarView(logo, tilte, buttonShow, userShow));
        }

        public void SetUserName(String name)
        {
            textBlock.Text = name;
            userInfo.Visibility = System.Windows.Visibility.Visible;
        }


        public void SetUserPicture(BitmapImage image)
        {
                userPicture.ImageSource = image;
        }

        public void Show()
        {
            StatusBarShow.Begin();
        }

        public void Hide()
        {
            StatusBarHide.Begin();
        }

        public void SetDownloadImage(System.Windows.Media.Imaging.BitmapImage image)
        {
            DownloadImage.Source = image;
        }

        public void PopChange(StatusBarView view)
        {
            NewStatusBarView = view;
            int i = 0;

            if (StatusBarView.ButtonShow == true && (NewStatusBarView.ButtonShow == false || StatusBarView.Tilte != NewStatusBarView.Tilte))
            {
                ButtonOut.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(ButtonOut);
                i += 200;
                BreakOut.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(BreakOut);
            }

            if (StatusBarView.Tilte != NewStatusBarView.Tilte)
            {
                i += 200;
                TilteOut.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(TilteOut);
            }

            if (StatusBarView.Logo != NewStatusBarView.Logo)
            {
                i += 200;
                LogoOut.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(LogoOut);
            }

            for (; 0 < AnimeQueue.Count; )
            {
                if (AnimeQueue.Count == 1)
                {
                    AnimeQueue.Peek().Completed += StatusBar_Completed;
                }
                AnimeQueue.Dequeue().Begin();
            }
        }

        public void Change(StatusBarView view)
        {
            PopChange(view);
        }

        void StatusBar_Completed(object sender, EventArgs e)
        {
            (sender as Storyboard).Completed -= StatusBar_Completed;
            int i = 0;
            if(NewStatusBarView.Logo != StatusBarView.Logo)
            {
                SetLogo(NewStatusBarView.Logo);
                LogoIn.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(LogoIn);
                i += 200;
            }
            if (NewStatusBarView.Tilte != StatusBarView.Tilte)
            {
                TilteText.Text = NewStatusBarView.Tilte;
                TilteIn.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(TilteIn);
                i += 200;
            }
            if(NewStatusBarView.ButtonShow)
            {
                BreakIn.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(BreakIn);
                i += 200;

                ButtonIn.BeginTime = new TimeSpan(0, 0, 0, 0, i);
                AnimeQueue.Enqueue(ButtonIn);
                i += 200;
            }

            for (; 0 < AnimeQueue.Count; )
            {
                AnimeQueue.Dequeue().Begin();
            }

            StatusBarView = NewStatusBarView;
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WebsiteBoxIn.Begin();
            block.Visibility = System.Windows.Visibility.Visible;
        }

        private void block_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WebsiteBoxOut.Begin();
            block.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void SetLogo(Logos logo)
        {
            switch (logo)
            {
                case Logos.Yandere:
                    websiteLogo.Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri("/Assets/Logos/yandere.png", UriKind.Relative)),
                    };
                    websiteLogo.Visibility = System.Windows.Visibility.Visible;
                    iconLogo.Visibility = System.Windows.Visibility.Collapsed;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.Konachan:
                    websiteLogo.Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri("/Assets/Logos/konachan.png", UriKind.Relative)),
                    };
                    websiteLogo.Visibility = System.Windows.Visibility.Visible;
                    iconLogo.Visibility = System.Windows.Visibility.Collapsed;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.Microsoft:
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Collapsed;
                    microsoftLogo.Visibility = System.Windows.Visibility.Visible;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.Search:
                    iconLogo.Source = new BitmapImage(new Uri("/Assets/Icons/feature.search.png", UriKind.Relative));
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Visible;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.Debug:
                    iconLogo.Source = new BitmapImage(new Uri("/Assets/Icons/appbar.bug.png", UriKind.Relative));
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Visible;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.MoePic:
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Collapsed;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Visible;
                    break;
                case Logos.Download:
                    iconLogo.Source = new BitmapImage(new Uri("/Assets/Icons/appbar.download.png", UriKind.Relative));
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Visible;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.Favorite:
                    iconLogo.Source = new BitmapImage(new Uri("/Assets/Icons/Favourites.png", UriKind.Relative));
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Visible;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.Setting:
                    iconLogo.Source = new BitmapImage(new Uri("/Assets/Icons/appbar.settings.png", UriKind.Relative));
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Visible;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Logos.History:
                    iconLogo.Source = new BitmapImage(new Uri("/Assets/Icons/History.png", UriKind.Relative));
                    websiteLogo.Visibility = System.Windows.Visibility.Collapsed;
                    iconLogo.Visibility = System.Windows.Visibility.Visible;
                    microsoftLogo.Visibility = System.Windows.Visibility.Collapsed;
                    moeLogo.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MoePic.Models.Settings.Current.Website = "Y";
            Models.MoebooruAPI.SetWebsiteY();
            WebsiteBoxOut.Begin();
            block.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MoePic.Models.Settings.Current.Website = "K";
            Models.MoebooruAPI.SetWebsiteK();
            WebsiteBoxOut.Begin();
            block.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void WebsiteBoxOut_Completed(object sender, EventArgs e)
        {
            if(MoePic.Models.Settings.Current.WebSiteLogo != StatusBarView.Logo)
            {
                PopChange(new StatusBarView(MoePic.Models.Settings.Current.WebSiteLogo, MoePic.Models.Settings.Current.WebSiteLogo == Logos.Yandere ? "yande.re" : "Konachan", true, true));
                if(WebSiteChange != null)
                {
                    WebSiteChange(this, new EventArgs());
                }

            }
            
        }

        public event EventHandler WebSiteChange;

        private void ImageBrush_ImageOpened(object sender, RoutedEventArgs e)
        {
            AvatarOk.Begin();
        }
    }

    public enum Logos
    {
        Yandere,
        Konachan,
        Microsoft,
        Search,
        Debug,
        MoePic,
        Download,
        Favorite,
        Setting,
        History
    }

    public class StatusBarView
    {
        public String Tilte { get; set; }

        public Logos Logo { get; set; }

        public bool ButtonShow { get; set; }

        public bool UserShow { get; set; }

        public StatusBarView(Logos logo, String tilte, bool buttonShow, bool userShow)
        {
            Tilte = tilte;
            Logo = logo;
            ButtonShow = buttonShow;
            UserShow = userShow;
        }

        public StatusBarView()
        {

        }
    }
}
