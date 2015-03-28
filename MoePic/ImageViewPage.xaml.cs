using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using MoePic.Models;
using Microsoft.Xna.Framework.Media;

namespace MoePic
{
    public partial class ImageViewPage : Models.MoePicPage
    {
        bool orientationChange = false;

        const double MaxScale = 10;

        double _scale = 1.0;
        double _scaleOriginalV;
        double _scaleOriginalH;
        double _minScale;
        double _coercedScale;
        double _originalScale;


        Size _viewportSize;
        bool _pinching;
        Point _screenMidpoint;
        Point _relativeMidpoint;

        System.Windows.Media.Imaging.BitmapImage _bitmap;

        public ImageViewPage()
        {
            InitializeComponent();
            this.ApplicationBar.BackgroundColor = System.Windows.Media.Color.FromArgb(0xFF, 0x1F, 0x1F, 0x1F);
        }

        void ImageViewPage_DownloadProgress(object sender, System.Windows.Media.Imaging.DownloadProgressEventArgs e)
        {
            downProgress.Value = e.Progress;
        }


        void viewport_ViewportChanged(object sender, System.Windows.Controls.Primitives.ViewportChangedEventArgs e)
        {
            Size newSize = new Size(viewport.Viewport.Width, viewport.Viewport.Height);
            if (newSize != _viewportSize)
            {
                _viewportSize = newSize;
                CoerceScale(true);
                ResizeImage(false);
                if (orientationChange)
                {
                    switch (Orientation)
                    {
                        case PageOrientation.Landscape:
                        case PageOrientation.LandscapeLeft:
                        case PageOrientation.LandscapeRight:
                            _scaleOriginalV = _scale;
                            break;
                        case PageOrientation.Portrait:
                        case PageOrientation.PortraitUp:
                            _scaleOriginalH = _scale;
                            break;
                    }
                    orientationChange = false;
                }
            }
        }

        void Load()
        {
            System.Windows.Media.Imaging.BitmapImage Source = new System.Windows.Media.Imaging.BitmapImage(CDNHelper.GetCDNUri(MoePic.Models.DownloadTask.GetImageUri(Info.Key, Info.Value)));
            Source.DownloadProgress += ImageViewPage_DownloadProgress;
            Source.ImageFailed += Source_ImageFailed;
            TestImage.Source = Source;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            if (e.NavigationMode == NavigationMode.New)
            {
                if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) != null && MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) is KeyValuePair<MoePic.Models.MoePost, MoePic.Models.ImageType>)
                {
                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).ImageInfo = Info = (KeyValuePair<MoePic.Models.MoePost, MoePic.Models.ImageType>)MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext);
                    
                }
                MoePic.Models.StatusBarService.HideStatusBar();
            }
            else
            {
                Info = (App.Current.Resources["RuntimeResources"] as RuntimeResources).ImageInfo;
            }

            Load();

            IsFav = FavoriteHelp.ContainsPost(Info.Key);
            if (IsFav)
            {
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IconUri = new Uri("/Assets/Icons/favs.png", UriKind.Relative);
            }
            else
            {
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IconUri = new Uri("/Assets/Icons/favs.addto.png", UriKind.Relative);
            }
            base.OnNavigatedTo(e);
        }

        void Source_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            failText.Visibility = System.Windows.Visibility.Visible;
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (TestImage != null && TestImage.Source != null)
            {
                (TestImage.Source as System.Windows.Media.Imaging.BitmapImage).UriSource = null;
                TestImage.Source = null;
            }
            MoePic.Models.StatusBarService.ShowStatusBar();
            base.OnBackKeyPress(e);
        }

        bool first = true;


        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            Opened = true;
            downProgress.Visibility = System.Windows.Visibility.Collapsed;
            downProgress2.Visibility = System.Windows.Visibility.Collapsed;
            _bitmap = TestImage.Source as System.Windows.Media.Imaging.BitmapImage;

            _scale = 0;
            CoerceScale(true);
            _scale = _coercedScale;
            ResizeImage(true);


            if (first)
            {
                switch (Orientation)
                {
                    case PageOrientation.Landscape:
                    case PageOrientation.LandscapeLeft:
                    case PageOrientation.LandscapeRight:
                        _scaleOriginalH = _scale;
                        break;
                    case PageOrientation.Portrait:
                    case PageOrientation.PortraitUp:
                        _scaleOriginalV = _scale;
                        break;
                }
                first = false;
            }
        }


        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            orientationChange = true;
        }


        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _pinching = false;
            _originalScale = _scale;
        }

        void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;

                if (!_pinching)
                {
                    _pinching = true;
                    Point center = e.PinchManipulation.Original.Center;
                    _relativeMidpoint = new Point(center.X / TestImage.ActualWidth, center.Y / TestImage.ActualHeight);

                    var xform = TestImage.TransformToVisual(viewport);
                    _screenMidpoint = xform.Transform(center);
                }

                _scale = _originalScale * e.PinchManipulation.CumulativeScale;


                switch (Orientation)
                {
                    case PageOrientation.Landscape:
                    case PageOrientation.LandscapeLeft:
                    case PageOrientation.LandscapeRight:
                        if (_scale > _scaleOriginalH)
                        {
                            this.ApplicationBar.IsVisible = false;
                        }
                        else
                        {
                            this.ApplicationBar.IsVisible = true;
                        }
                        break;
                    case PageOrientation.Portrait:
                    case PageOrientation.PortraitUp:
                        if (_scale > _scaleOriginalV)
                        {
                            this.ApplicationBar.IsVisible = false;
                        }
                        else
                        {
                            this.ApplicationBar.IsVisible = true;
                        }
                        break;
                }


                CoerceScale(false);
                ResizeImage(false);
            }
            else if (_pinching)
            {
                _pinching = false;
                _originalScale = _scale = _coercedScale;
            }
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            _scale = _coercedScale;
        }

        void ResizeImage(bool center)
        {
            if (_coercedScale != 0 && _bitmap != null)
            {

                double newWidth = canvas.Width = Math.Round(_bitmap.PixelWidth * _coercedScale);
                double newHeight = canvas.Height = Math.Round(_bitmap.PixelHeight * _coercedScale);

                xform.ScaleX = xform.ScaleY = _coercedScale;

                viewport.Bounds = new Rect(0, 0, newWidth, newHeight);

                if (center)
                {
                    viewport.SetViewportOrigin(
                        new Point(
                            Math.Round((newWidth - viewport.ActualWidth) / 2),
                            Math.Round((newHeight - viewport.ActualHeight) / 2)
                            ));
                }
                else
                {
                    Point newImgMid = new Point(newWidth * _relativeMidpoint.X, newHeight * _relativeMidpoint.Y);
                    Point origin = new Point(newImgMid.X - _screenMidpoint.X, newImgMid.Y - _screenMidpoint.Y);
                    viewport.SetViewportOrigin(origin);
                }
            }
        }


        void CoerceScale(bool recompute)
        {
            if (recompute && _bitmap != null && viewport != null)
            {
                // Calculate the minimum scale to fit the viewport 
                double minX = viewport.ActualWidth / _bitmap.PixelWidth;
                double minY = viewport.ActualHeight / _bitmap.PixelHeight;

                _minScale = Math.Min(minX, minY);
            }

            _coercedScale = Math.Min(MaxScale, Math.Max(_scale, _minScale));
        }

        private void GestureListener_Tap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            switch (Orientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                case PageOrientation.LandscapeRight:
                    if (_scale > _scaleOriginalH)
                    {
                        this.ApplicationBar.IsVisible = false;
                    }
                    else
                    {
                        if (this.ApplicationBar.IsVisible)
                        {
                            this.ApplicationBar.IsVisible = false;
                        }
                        else
                        {
                            this.ApplicationBar.IsVisible = true;
                        }
                    }
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    if (_scale > _scaleOriginalV)
                    {
                        this.ApplicationBar.IsVisible = false;
                    }
                    else
                    {
                        if (this.ApplicationBar.IsVisible)
                        {
                            this.ApplicationBar.IsVisible = false;
                        }
                        else
                        {
                            this.ApplicationBar.IsVisible = true;
                        }
                    }
                    break;
            }

        }

        KeyValuePair<MoePic.Models.MoePost, MoePic.Models.ImageType> Info;

        private void GestureListener_DoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            if (_scale != 1.0)
            {
                _scale = 0;
                CoerceScale(true);
                _scale = _coercedScale = 1.0;
                ResizeImage(true);
            }
            else
            {
                _scale = 0;
                CoerceScale(true);
                _scale = _coercedScale;
                ResizeImage(true);
            }

        }
        bool Saving = false;
        bool Opened = false;
        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (Opened)
            {
                if (!Saving)
                {
                    Saving = true;
                    String name = null;

                    if (Info.Key.preview_url.Contains("yande"))
                    {
                        name = String.Format("{0} - {1}.{2}", "yande.re", Info.Key.id, System.IO.Path.GetExtension(DownloadTask.GetImageUri(Info.Key, Info.Value)));
                    }
                    else
                    {
                        name = String.Format("{0} - {1}.{2}", "Konachan", Info.Key.id, System.IO.Path.GetExtension(DownloadTask.GetImageUri(Info.Key, Info.Value)));
                    }

                    System.Net.HttpUtility.UrlDecode(System.IO.Path.GetFileName(DownloadTask.GetImageUri(Info.Key, Info.Value)));
                    await ImageSaveHelp.SaveImage(name, TestImage.Source as System.Windows.Media.Imaging.BitmapImage);
                    MoePic.Models.ToastService.Show(new Uri(Info.Key.preview_url), "图片已保存在图库");
                    Saving = false;
                }
                else
                {
                    MoePic.Models.ToastService.Show("图片保存中...");
                }
            }
            else
            {
                MoePic.Models.ToastService.Show("图片下载中...");
            }

        }

        private async void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            if (Opened)
            {
                Microsoft.Phone.Tasks.ShareMediaTask smt = new Microsoft.Phone.Tasks.ShareMediaTask();
                smt.FilePath = await ImageSaveHelp.SaveImageToIso("jpg", TestImage.Source as System.Windows.Media.Imaging.BitmapImage);
                smt.Show();
            }
            else
            {
                MoePic.Models.ToastService.Show("图片下载中...");
            }

        }

        bool IsFav = false;

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            if (IsFav)
            {
                IsFav = false;
                FavoriteHelp.DelFavorite(Info.Key);
                ToastService.Show(new Uri(Info.Key.preview_url), MoePic.Resources.AppResources.DelFav);
            }
            else
            {
                IsFav = true;
                FavoriteHelp.AddFavorite(Info.Key);
                ToastService.Show(new Uri(Info.Key.preview_url), MoePic.Resources.AppResources.AddFav, (s, a) => { Models.NavigationService.Navigate("FavoritePage.xaml"); }, null, null);
            }
            if (IsFav)
            {
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IconUri = new Uri("/Assets/Icons/favs.png", UriKind.Relative);
            }
            else
            {
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IconUri = new Uri("/Assets/Icons/favs.addto.png", UriKind.Relative);
            }
        }

        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            if(Opened)
            {
                this.ApplicationBar.IsVisible = false;
                ShareAPI.Shared += (s, a) =>
                {
                    this.ApplicationBar.IsVisible = true;
                };
                ShareAPI.Share("分享图片", TestImage.Source as System.Windows.Media.Imaging.BitmapImage, Info.Key);
            }
            else
            {
                ToastService.Show("图片下载中...");
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(UriHelp.GetShareUrl(Info.Key));
            ToastService.Show("已复制链接分享到剪切板");
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            if(Opened)
            {
            Clipboard.SetText(UriHelp.GetShareUrl(Info.Key));
            ShareAPI.ShareToWX(Info.Key, TestImage.Source as System.Windows.Media.Imaging.BitmapImage);
            }
            else
            {
                ToastService.Show("图片下载中...");
            }
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            ShareAPI.ShareToQzone(Info.Key);
        }
    }
}