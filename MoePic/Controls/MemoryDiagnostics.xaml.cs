using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;

using System.Windows.Threading;

namespace MoePic.Controls
{
    public partial class MemoryDiagnostics : UserControl
    {
        DispatcherTimer timer;

        public MemoryDiagnostics()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        bool IsShow = false;
        bool high = false;

        void timer_Tick(object sender, EventArgs e)
        {

            usages.Text = String.Format("{0:F2}MB", 1.0 * Windows.System.MemoryManager.AppMemoryUsage / 1024 / 1024);
            limit.Text = String.Format("{0:F2}MB", 1.0 * Windows.System.MemoryManager.AppMemoryUsageLimit / 1024 / 1024);
            switch (Windows.System.MemoryManager.AppMemoryUsageLevel)
            {
                case Windows.System.AppMemoryUsageLevel.High:
                    level.Text = "高";
                    level.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                    break;
                case Windows.System.AppMemoryUsageLevel.Low:
                    level.Text = "低";
                    level.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                    break;
                case Windows.System.AppMemoryUsageLevel.Medium:
                    level.Text = "中";
                    level.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
                    break;
            }
            if (!high && !IsShow &&Windows.System.MemoryManager.AppMemoryUsageLevel == Windows.System.AppMemoryUsageLevel.High) 
            {
                high = true;
                IsShow = true;
                Models.MessageBoxService.Show("高内存使用率", "当前内存使用率高,建议您保存当前操作并重新启动应用程序.", true, new Command("取消", (s, a) => { IsShow = false; }));
            }
            else
            {
                high = false;
            }
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Models.AchievementService.Test();
        }
    }
}
