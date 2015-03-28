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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Media;

using MoePic.Models;

namespace MoePic.Controls
{
    public partial class AchievementBox : UserControl
    {
        public AchievementBox()
        {
            InitializeComponent();
        }

        public AchievementBox(Achievement achievement)
        {
            Achievement = achievement;
            InitializeComponent();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            (sender as System.Windows.Threading.DispatcherTimer).Stop();
            BoxIn.Begin();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            effect.Width = content.ActualWidth - 30;
            (color.Background as SolidColorBrush).Color = GetColor();
            BoxOut.Begin();
            var stream = TitleContainer.OpenStream("Assets/Sound/Achievement Unlocked.wav");
            SoundEffect sound = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            sound.Play();
            stream.Close();

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 5)
            };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public System.Windows.Media.Color GetColor()
        {
            switch (Achievement.Type)
            {
                case AchievementType.Gold:
                    return System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00);
                case AchievementType.Silver:
                    return System.Windows.Media.Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0);
                case AchievementType.Copper:
                    return System.Windows.Media.Color.FromArgb(0xFF, 0xD2, 0x69, 0x1E);
            }
            return System.Windows.Media.Color.FromArgb(0xFF, 0xD2, 0x69, 0x1E);
        }


        public Achievement Achievement
        {
            get { return (Achievement)GetValue(AchievementProperty); }
            set { SetValue(AchievementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Achievement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AchievementProperty =
            DependencyProperty.Register("Achievement", typeof(Achievement), typeof(AchievementBox), null);

        
        


        private void BoxIn_Completed(object sender, EventArgs e)
        {

            (this.Parent as Popup).IsOpen = false;
        }
    }
}
