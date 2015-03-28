using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Controls
{
    public partial class PeopleTile : UserControl
    {
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        Random ran = new Random();

        public List<String> Source { get; set; }

        public PeopleTile()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 0, 0, 2);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        Queue<PeopleTileUpdataTask> UpdataTask = new Queue<PeopleTileUpdataTask>();

        void Timer_Tick(object sender, EventArgs e)
        {
            Brush content = null;
            if(UpdataTask.Count == 0)
            {
                if(Source != null && Source.Count != 0)
                {
                    int x = ran.Next(1, 7);
                    int y = ran.Next(1, 4);
                    int r = ran.Next(1, 101);

                    if (r < 11)
                    {
                        content = new SolidColorBrush(Colors.Transparent);
                        GetItemFormPoint(x, y).SetContent(content);
                    }
                    else if (r < 21)
                    {
                        content = new SolidColorBrush(Color.FromArgb(0x55, 0, 0, 0));
                        GetItemFormPoint(x, y).SetContent(content);
                    }
                    else if (r < 31)
                    {
                        content = new SolidColorBrush(Color.FromArgb(0xAA, 0, 0, 0));
                        GetItemFormPoint(x, y).SetContent(content);
                    }
                    else if (r > 80 && x < 6 && y < 3)
                    {
                        content = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(Source[ran.Next(Source.Count)])),
                        };

                        List<PeopleTileUpdataTask> tasks = new List<PeopleTileUpdataTask>();
                        tasks.Add(new PeopleTileUpdataTask(GetItemFormPoint(x, y), content, 1));
                        tasks.Add(new PeopleTileUpdataTask(GetItemFormPoint(x + 1, y), content, 2));
                        tasks.Add(new PeopleTileUpdataTask(GetItemFormPoint(x, y + 1), content, 3));
                        tasks.Add(new PeopleTileUpdataTask(GetItemFormPoint(x + 1, y + 1), content, 4));

                        List<int> i = new List<int> {0, 1, 2, 3 };
                        for (int n = 3; n >=0; n--)
                        {
                            int m = ran.Next(n + 1);
                            UpdataTask.Enqueue(tasks[i[m]]);
                            i.RemoveAt(m);
                        }

                        Timer_Tick(sender, e);
                        return;
                    }
                    else
                    {
                        content = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(Source[ran.Next(Source.Count)])),
                        };
                        GetItemFormPoint(x, y).SetContent(content);
                    }
                }
                else
                {
                    int x = ran.Next(1, 7);
                    int y = ran.Next(1, 4);

                    int r = ran.Next(1, 4);
                    switch (r)
                    {
                        case 1:
                            content = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 2:
                            content = new SolidColorBrush(Color.FromArgb(0x55,0,0,0));
                            break;
                        case 3:
                            content = new SolidColorBrush(Color.FromArgb(0xAA, 0, 0, 0));
                            break;
                        default:
                            break;
                    }
                    GetItemFormPoint(x, y).SetContent(content);
                }
                Timer.Interval = new TimeSpan(ran.Next(10000000,30000000));
            }
            else
            {
                PeopleTileUpdataTask task = UpdataTask.Dequeue();
                task.Item.SetContent(task.Content, task.Type);
                Timer.Interval = new TimeSpan(ran.Next(5000000, 10000000));
            }
        }

        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(PeopleTile), null);

        PeopleTileItem GetItemFormPoint(int x, int y)
        {
            return MoePic.Models.FindElement.FindVisualElementFormName(this, String.Format("item{0}{1}", y, x)) as PeopleTileItem;
        }



        public string Tilte
        {
            get { return (string)GetValue(TilteProperty); }
            set { SetValue(TilteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tilte.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TilteProperty =
            DependencyProperty.Register("Tilte", typeof(string), typeof(PeopleTile), null);

        

    }

    public class PeopleTileUpdataTask
    {
        public PeopleTileItem Item { get; set; }
        public Brush Content { get; set; }
        public int Type { get; set; }

        public PeopleTileUpdataTask(PeopleTileItem item,Brush content,int type)
        {
            Item = item;
            Content = content;
            Type = type;
        }
    }

    public class Doublex:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * Double.Parse(parameter as String);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / Double.Parse(parameter as String);
        }
    }
}
