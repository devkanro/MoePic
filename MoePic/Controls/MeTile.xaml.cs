using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Controls
{
    public partial class MeTile : UserControl
    {
        public MeTile()
        {
            InitializeComponent();
        }

        private void image_ImageOpened(object sender, RoutedEventArgs e)
        {
            ImageOpened.Begin();
        }

        private void ImageOpened_Completed(object sender, EventArgs e)
        {
            RollDown.Begin();
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set 
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(MeTile), null);


        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(MeTile), null);

        
        
    }
}
