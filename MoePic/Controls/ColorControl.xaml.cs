using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using MoePic.Models;

namespace MoePic.Controls
{
    public partial class ColorControl : UserControl
    {
        public ColorControl()
        {
            InitializeComponent();
        }

        public Color Color;

        public bool ReadColor = false;

        private void defaultImageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loaded)
            {
                switch (defaultImageType.SelectedIndex)
                {
                    case 0:
                        Color = (App.Current.Resources["ThemeColor"] as ThemeColor).Foreground.Color;
                        break;
                    case 1:
                        Color = (App.Current.Resources["ThemeColor"] as ThemeColor).Background.Color;
                        break;
                    case 2:
                        Color = (App.Current.Resources["ThemeColor"] as ThemeColor).Text2.Color;
                        break;
                    case 3:
                        Color = (App.Current.Resources["ThemeColor"] as ThemeColor).NavigateBar.Color;
                        break;
                    case 4:
                        Color = (App.Current.Resources["ThemeColor"] as ThemeColor).StatusBar.Color;
                        break;
                }
                ReadColor = true;
                Red.Value = Color.R;
                Green.Value = Color.G;
                Blue.Value = Color.B;
                ColorText.Text = String.Format("#{0:X}{1:X}{2:X}", (int)Red.Value, (int)Green.Value, (int)Blue.Value);
                ReadColor = false;
            }
            
        }

        private void Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(!ReadColor)
            {
                Color = Color.FromArgb(0xFF, (byte)Red.Value, (byte)Green.Value, (byte)Blue.Value);
                switch (defaultImageType.SelectedIndex)
                {
                    case 0:
                        (App.Current.Resources["ThemeColor"] as ThemeColor).Foreground.Color = Color;
                        break;
                    case 1:
                        (App.Current.Resources["ThemeColor"] as ThemeColor).Background.Color = Color;
                        break;
                    case 2:
                        (App.Current.Resources["ThemeColor"] as ThemeColor).Text2.Color = Color;
                        break;
                    case 3:
                        (App.Current.Resources["ThemeColor"] as ThemeColor).NavigateBar.Color = Color;
                        break;
                    case 4:
                        (App.Current.Resources["ThemeColor"] as ThemeColor).StatusBar.Color = Color;
                        break;
                }
                ColorText.Text = String.Format("#{0:X}{1:X}{2:X}", (int)Red.Value, (int)Green.Value, (int)Blue.Value);
            }
        }

        Point DownPoint;
        Point NowPoint;

        private void ColorfulButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            moveButton.CaptureMouse();
            NowPoint = new Point(move.TranslateX, move.TranslateY);
            DownPoint = e.GetPosition(App.RootFrame);
        }

        private void moveButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point MovePoint = e.GetPosition(App.RootFrame);
            move.TranslateX = NowPoint.X + (MovePoint.X - DownPoint.X);
            move.TranslateY = NowPoint.Y + (MovePoint.Y - DownPoint.Y);
        }

        static Popup popup;

        public static void Init()
        {
            if(popup == null)
            {
                popup = new Popup()
                {
                    Child = new ColorControl()
                };
                popup.IsOpen = true;
            }
        }

        bool loaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

        private void moveButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            moveButton.ReleaseMouseCapture();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as System.Windows.Controls.Primitives.Popup).IsOpen = false;
            popup = null;
        }
    }
}
