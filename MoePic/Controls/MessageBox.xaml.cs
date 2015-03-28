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

namespace MoePic.Controls
{
    public partial class MessageBox : UserControl
    {
        public bool CanClose { get; set; }

        public MessageBox(String Title,String Content, params Command[] buttons)
        {
            InitializeComponent();
            Width = Models.DisplaySize.Current.Width;
            Height = Models.DisplaySize.Current.Height;
            tilteText.Text = Title;
            contentText.Text = Content;
            if(buttons.Length != 0)
            {
                foreach (var item in buttons)
                {
                    Button b = new Button() { Style = MessageBoxButtonStyle, Content = item.Content };
                    if(buttonsViewer.Children.Count + 1 == buttons.Length)
                    {
                        b.Foreground = new SolidColorBrush(Colors.White);
                        b.Background = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                    }
                    else
                    {
                        b.Foreground = new SolidColorBrush(Colors.Black);
                        b.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                    }

                    if(item.Event != null)
                    {
                        b.Click += CloseMessageBox;
                        b.Click += item.Event;
                    }
                    else
                    {
                        b.Click += CloseMessageBox;
                    }

                    buttonsViewer.Children.Add(b);
                }
                
            }
            else
            {
                Button b = new Button() { Style = MessageBoxButtonStyle, Content = "取消", Foreground = new SolidColorBrush(Colors.White), Background = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80)) };
                b.Click += CloseMessageBox;
                buttonsViewer.Children.Add(b);
            }
        }

        public void CloseMessageBox(object sender, RoutedEventArgs e)
        {
            (this.Parent as Popup).IsOpen = false;
        }

    }

    public class Command
    {
        public String Content { get; set; }
        public RoutedEventHandler Event { get; set; }
        public Command(String content, RoutedEventHandler @event)
        {
            Content = content;
            Event = @event;
        }
    }
}
