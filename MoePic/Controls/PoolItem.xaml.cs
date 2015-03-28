using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using MoePic.Models;

namespace MoePic.Controls
{
    public partial class PoolItem : UserControl
    {
        public PoolItem()
        {
            InitializeComponent();
        }

        private void image_ImageOpened(object sender, RoutedEventArgs e)
        {
            Loaded.Begin();
        }

        public MoePool Pool
        {
            get { return (MoePool)GetValue(PoolProperty); }
            set { SetValue(PoolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Pool.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PoolProperty =
            DependencyProperty.Register("Pool", typeof(MoePool), typeof(PoolItem),new PropertyMetadata(PoolChanged));

        public static void PoolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as UserControl).Width = 237;
            (d as UserControl).Height = 1.0 * 237 / (e.NewValue as MoePic.Models.MoePool).posts[0].actual_preview_width * (e.NewValue as MoePic.Models.MoePool).posts[0].actual_preview_height;
        }

        private void LayoutRoot_Click(object sender, RoutedEventArgs e)
        {
            Models.NavigationService.Navigate("PoolViewPage.xaml", Pool);
        }
    }

    public class PoolNameConvert : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as String).Replace('_', ' ');
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return (value as String).Replace(' ', '_');
        }
    }
}
