using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Controls
{
    public partial class ColorfulButton : Button
    {
        public ColorfulButton()
        {
            InitializeComponent();
        }


        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ColorfulButton), null);

        public Brush ContentBrush
        {
            get { return (Brush)GetValue(ContentBrushProperty); }
            set { SetValue(ContentBrushProperty, value); }
        }

        public static readonly DependencyProperty ContentBrushProperty = DependencyProperty.Register("ContentBrush", typeof(Brush), typeof(ColorfulButton), null);



        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ColorfulButton), null);
    }
}
