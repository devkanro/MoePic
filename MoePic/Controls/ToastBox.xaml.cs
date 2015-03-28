using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Controls
{
    public partial class ToastBox : UserControl
    {
        bool _ToastBoxButtonDown = false;
        bool _ToastBoxButtonClick = false;
        Point _ButtonDownPoint = new Point();
        DispatcherTimer Timer = new DispatcherTimer();
        double X;
        Popup Popup = null;

        public ToastBox(Popup popup)
        {
            Popup = popup;
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 0, 5);
            Timer.Tick += Timer_Tick;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            ToastHide.Begin();
            if(TimeOver != null)
            {
                TimeOver(this, new EventArgs());
            }
        }

        public void ShowToast(Uri image, String content, EventHandler<EventArgs> click, EventHandler<EventArgs> timeOver, EventHandler<EventArgs> close)
        {
            icon.Source = new System.Windows.Media.Imaging.BitmapImage(image);
            text.Text = content;
            Click += click;
            TimeOver += timeOver;
            Close += close;
            Timer.Start();
            ToastShow.Begin();
        }

        private void ToastHide_Completed(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        public event EventHandler<EventArgs> Click;
        public event EventHandler<EventArgs> TimeOver;
        public event EventHandler<EventArgs> Close;

        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Timer.Stop();
            LayoutRoot.CaptureMouse();
            _ToastBoxButtonDown = true;
            _ToastBoxButtonClick = true;
            _ButtonDownPoint = e.GetPosition(this);
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_ToastBoxButtonDown)
            {
                LayoutRoot.ReleaseMouseCapture();
                if (_ToastBoxButtonClick)
                {
                    ToastHide.Begin();
                    if (Click != null)
                    {
                        Click(this, new EventArgs());
                    }
                }
                else
                {
                    if (X < 240)
                    {
                        ToastBackFrame1.Value = X;
                        ToastBack.Begin();
                        Timer.Start();
                    }
                    else
                    {
                        ToastOutFrame1.Value = X;
                        ToastOutFrame2.Value = X + 240;
                        ToastOut.Begin();
                        if (Close != null)
                        {
                            Close(this, new EventArgs());
                        }
                    }
                }

                _ToastBoxButtonDown = false;
            }
        }

        private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_ToastBoxButtonDown)
            {
                _ToastBoxButtonClick = false;
                X = e.GetPosition(this).X - _ButtonDownPoint.X;
                if (X < 0)
                {
                    X = 0;
                }
                transform.TranslateX = X;
            }
        }

    }
}
