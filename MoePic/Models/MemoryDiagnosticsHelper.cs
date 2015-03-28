using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Phone.Info;
using System.Diagnostics;
using System.Collections.Generic;
namespace MoePic.Models
{
    public static class MemoryDiagnosticsHelper
    {
        static Popup Popup;
        static Controls.MemoryDiagnostics me;

        public static bool IsOpen
        {
            get
            {
                return Popup != null && me != null;
            }
        }

        public static void Start()
        {
            if (Popup == null || me == null)
            {
                Popup = new Popup();
                me = new Controls.MemoryDiagnostics();
                Popup.Child = me;
                Popup.IsOpen = true;
            }
        }

        public static void Stop()
        {
            if (Popup != null && me != null)
            {
                Popup.IsOpen = false;
            }
            Popup = null;
            me = null;
        }
    }

}
