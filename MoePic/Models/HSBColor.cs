using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Models
{
    public class HSBColor : DependencyObject
    {
        public HSBColor()
        {

        }

        public HSBColor(Color color)
        {
            double h = 0, s = 0, b = 0;
            RGBtoHSB(color.R, color.G, color.B, out h, out s, out b);
            Hues = h;
            Saturation = s;
            Brightness = b;
        }

        public HSBColor(byte red,byte green,byte bule)
        {
            double h = 0, s = 0, b = 0;
            RGBtoHSB(red, green, bule, out h, out s, out b);
            Hues = h;
            Saturation = s;
            Brightness = b;
        }

        public HSBColor(double h, double s, double b)
        {
            Hues = h;
            Saturation = s;
            Brightness = b;
        }

        public double Hues
        {
            get { return (double)GetValue(HuesProperty); }
            set { SetValue(HuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HuesProperty =
            DependencyProperty.Register("Hues", typeof(double), typeof(HSBColor), null);




        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Saturation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(HSBColor), null);




        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Brightness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(HSBColor), null);


        public Color GetColor()
        {
            byte r=0,g=0,b=0;
            return HSBtoRGB(Hues,Saturation,Brightness,out r,out g,out b);
        }


        public static HSBColor RGBtoHSB(byte red, byte green, byte blue, out double hu, out double sa, out double br)
        {

            double r = ((double)red / 255.0);
            double g = ((double)green / 255.0);
            double b = ((double)blue / 255.0);


            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double h = 0.0;
            if (max == r && g >= b)
            {
                h = 60 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            double s = (max == 0) ? 0.0 : (1.0 - (min / max));

            hu = h;
            sa = s;
            br = (double)max;

            return new HSBColor(h, s, (double)max);
        }

        public static Color HSBtoRGB(double hi, double si, double bi, out byte red,out byte green,out byte blue)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (si == 0)
            {
                r = g = b = bi;
            }
            else
            {
                double sectorPos = hi / 60.0;
                int sectorNumber = (int)(Math.Floor(sectorPos));

                double fractionalSector = sectorPos - sectorNumber;

                double p = bi * (1.0 - si);
                double q = bi * (1.0 - (si * fractionalSector));
                double t = bi * (1.0 - (si * (1 - fractionalSector)));

                switch (sectorNumber)
                {
                    case 0:
                        r = bi;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = bi;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = bi;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = bi;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = bi;
                        break;
                    case 5:
                        r = bi;
                        g = p;
                        b = q;
                        break;
                }
            }

            red = Convert.ToByte(Double.Parse(String.Format("{0:0.00}", r * 255.0)));
            green = Convert.ToByte(Double.Parse(String.Format("{0:0.00}", g * 255.0)));
            blue = Convert.ToByte(Double.Parse(String.Format("{0:0.00}", b * 255.0)));

            return Color.FromArgb(0xFF, red, green, blue);
        }

        public void change()
        {
            
        }
    }

    public class HSBColorToRGBColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as HSBColor).GetColor();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double h=0,s=0,b=0;
            return HSBColor.RGBtoHSB(((Color)value).R,((Color)value).G,((Color)value).B,out h,out s,out b);
        }
    }
}
