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
    public partial class HubTile : UserControl
    {
        public HubTile()
        {
            Status = HubTileStatus.Stop;
            InitializeComponent();
        }

        bool Image1Ok = false;
        bool Image2Ok = false;

        public HubTileStatus Status { get; private set; }

        private void ImageBrush_ImageOpened(object sender, RoutedEventArgs e)
        {
            Image1Ok = true;
            if (Status == HubTileStatus.WaitForStart)
            {
                AnimeStart.Begin();
            }
            else if (Status == HubTileStatus.WaitForImage1)
            {
                SwitchImage1.Begin();
            }
        }

        public void StartAnime()
        {
            if (Source != null && Source.Count != 0)
            {
                Content1.ImageSource = new System.Windows.Media.Imaging.BitmapImage(Source[Index]);
                if (Status == HubTileStatus.Stop)
                {
                    if (Image1Ok)
                    {
                        Status = HubTileStatus.Runing;
                        AnimeStart.Begin();
                    }
                    else
                    {
                        Status = HubTileStatus.WaitForStart;
                    }
                }
                else if (Source.Count != 1 && Status == HubTileStatus.WaitForMoreImage)
                {
                    Status = HubTileStatus.ChangeToImage2;
                    ChangeImage();
                }

            }
        }

        int Index = 0;


        public List<Uri> Source
        {
            get { return (List<Uri>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(List<Uri>), typeof(HubTile), null);

        private void ImageBrush_ImageOpened_1(object sender, RoutedEventArgs e)
        {
            Image2Ok = true;
            if (Status == HubTileStatus.WaitForImage2)
            {
                SwitchImage2.Begin();
            }
        }

        private void AnimeStart_Completed(object sender, EventArgs e)
        {
            Status = HubTileStatus.ChangeToImage2;
            if (Source.Count == 1)
            {
                Status = HubTileStatus.WaitForMoreImage;
            }
            else
            {
                ChangeImage();
            }
        }

        private void SwitchImage2_Completed(object sender, EventArgs e)
        {
            Status = HubTileStatus.ChangeToImage1;
            ChangeImage();
        }

        private void SwitchImage1_Completed(object sender, EventArgs e)
        {
            Status = HubTileStatus.ChangeToImage2;
            ChangeImage();
        }

        void ChangeImage()
        {
            if (Status == HubTileStatus.ChangeToImage1)
            {
                Index++;
                if (Index == Source.Count)
                {
                    Index = 0;
                }
                Image1Ok = false;
                Status = HubTileStatus.WaitForImage1;
                Content1.ImageSource = new System.Windows.Media.Imaging.BitmapImage(Source[Index]);
                if (Image1Ok)
                {
                    Status = HubTileStatus.Runing;
                    SwitchImage1.Begin();
                }
            }
            else if (Status == HubTileStatus.ChangeToImage2)
            {
                Index++;
                if (Index == Source.Count)
                {
                    Index = 0;
                }
                Image2Ok = false;
                Status = HubTileStatus.WaitForImage2;
                Content2.ImageSource = new System.Windows.Media.Imaging.BitmapImage(Source[Index]);
                if (Image2Ok)
                {
                    Status = HubTileStatus.Runing;
                    SwitchImage2.Begin();
                }
            }
        }

        void Init()
        {
            Image1.Width = this.ActualWidth;
            Image2.Height = this.ActualHeight * 1.5;

            startFrame1.Value = this.ActualHeight;
            startFrame2.Value = this.ActualHeight * -0.5;
            startFrame3.To = this.ActualHeight;
            startFrame4.Value = this.ActualHeight * -0.5;

            switch2Frame1.Value = this.ActualHeight * -0.5;
            switch2Frame2.Value = this.ActualHeight * -1.5;
            switch2Frame3.Value = this.ActualHeight;
            switch2Frame4.Value = this.ActualHeight;
            switch2Frame5.Value = this.ActualHeight * -0.5;
            switch2Frame6.Value = this.ActualHeight;
            switch2Frame7.Value = this.ActualHeight * -0.5;

            switch1Frame1.Value = this.ActualHeight * -0.5;
            switch1Frame2.Value = this.ActualHeight * -1.5;
            switch1Frame3.Value = this.ActualHeight * -1.5;
            switch1Frame4.Value = this.ActualHeight;
            switch1Frame5.Value = this.ActualHeight;
            switch1Frame6.Value = this.ActualHeight;
            switch1Frame7.Value = this.ActualHeight * -0.5;
            switch1Frame8.Value = this.ActualHeight;
            switch1Frame9.Value = this.ActualHeight * -0.5;
        }

        public double TileWidth
        {
            get { return (double)GetValue(TileWidthProperty); }
            set { SetValue(TileWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileWidthProperty =
            DependencyProperty.Register("TileWidth", typeof(double), typeof(HubTile), null);



        public double TileHeight
        {
            get { return (double)GetValue(TileHeightProperty); }
            set { SetValue(TileHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileHeightProperty =
            DependencyProperty.Register("TileHeight", typeof(double), typeof(HubTile), null);




        public String TileTilte
        {
            get { return (String)GetValue(TileTilteProperty); }
            set { SetValue(TileTilteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileTilte.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileTilteProperty =
            DependencyProperty.Register("TileTilte", typeof(String), typeof(HubTile), null);

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.ActualHeight != 0)
            {
                Init();
            }
        }

    }

    public enum HubTileStatus
    {
        WaitForStart,
        WaitForImage1,
        WaitForImage2,
        WaitForMoreImage,
        Runing,
        Stop,
        ChangeToImage1,
        ChangeToImage2
    }
}
