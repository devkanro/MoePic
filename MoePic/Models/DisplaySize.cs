using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoePic.Models
{
    /// <summary>
    /// 提供对当前设备的屏幕尺寸的简易调用
    /// </summary>
    public class DisplaySize
    {
        static double height;
        static double width;

        static double actualHeight;
        static double actualWidth;

        static DisplaySize _Current = new DisplaySize();
        /// <summary>
        /// 对 DisplaySize 一个实例的访问
        /// </summary>
        public static DisplaySize Current
        {
            get
            {
                return _Current;
            }
        }

        public DisplaySize()
        {
            width = System.Windows.Application.Current.Host.Content.ActualWidth;
            height = System.Windows.Application.Current.Host.Content.ActualHeight;

            switch (System.Windows.Application.Current.Host.Content.ScaleFactor)
            {
                case 100:
                    actualHeight = 800;
                    actualWidth = 480;
                    break;
                case 112:
                    actualHeight = 960;
                    actualWidth = 540;
                    break;
                case 150:
                    actualHeight = 1280;
                    actualWidth = 720;
                    break;
                case 160:
                    actualHeight = 1280;
                    actualWidth = 768;
                    break;
                case 225:
                    actualHeight = 1920;
                    actualWidth = 1080;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 获取当前设备屏幕的虚拟高度
        /// </summary>
        public double Height
        {
            get
            {
                return height;
            }
        }

        /// <summary>
        /// 获取当前设备屏幕的虚拟宽度
        /// </summary>
        public double Width
        {
            get
            {
                return width;
            }
        }

        /// <summary>
        /// 获取当前设备屏幕的真实高度
        /// </summary>
        public double ActualHeight
        {
            get
            {
                return actualHeight;
            }
        }
        /// <summary>
        /// 获取当前设备屏幕的真实宽度
        /// </summary>
        public double ActualWidth
        {
            get
            {
                return actualWidth;
            }
        }

        private static object GetProperty(object instance, string name)
        {
            var getMethod = instance.GetType().GetProperty(name).GetGetMethod();
            return getMethod.Invoke(instance, null);
        }
    }
}
