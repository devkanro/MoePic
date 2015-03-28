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
    public partial class PeopleTileItem : UserControl
    {
        public PeopleTileItem()
        {
            InitializeComponent();
        }

        Brush newContent;
        int newType;

        public void SetContent(Brush content)
        {
            SetContent(content, 0);
        }

        public void SetContent(Brush content, int type)
        {
            newContent = content;
            newType = type;
            if(content is ImageBrush)
            {
                (content as ImageBrush).AlignmentY = AlignmentY.Top;
                (content as ImageBrush).Stretch = Stretch.UniformToFill;
            }
            RollDown.Begin();
        }

        private void RollDown_Completed(object sender, EventArgs e)
        {
            switch (newType)
            {
                case 0:
                    grid.Width = 240;
                    grid.Height = 240;
                    grid.Margin = new Thickness(0);
                    grid.Background = newContent;
                    break;
                case 1:
                    grid.Width = 480;
                    grid.Height = 480;
                    grid.Margin = new Thickness(0);
                    grid.Background = newContent;
                    break;
                case 2:
                    grid.Width = 480;
                    grid.Height = 480;
                    grid.Margin = new Thickness(-240,0,0,0);
                    grid.Background = newContent;
                    break;
                case 3:
                    grid.Width = 480;
                    grid.Height = 480;
                    grid.Margin = new Thickness(0,-240,0,0);
                    grid.Background = newContent;
                    break;
                case 4:
                    grid.Width = 480;
                    grid.Height = 480;
                    grid.Margin = new Thickness(-240,-240,240,240);
                    grid.Background = newContent;
                    break;
                default:
                    break;
            }
            RollUp.Begin();
        }
    }
}
