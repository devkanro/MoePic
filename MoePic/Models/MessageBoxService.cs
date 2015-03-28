using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;

using MoePic.Controls;

namespace MoePic.Models
{
    public static class MessageBoxService
    {
        public static List<Popup> MessageBoxStack = new List<Popup>();
        public static Queue<Popup> WaitMessageBoxQueue = new Queue<Popup>();

        public static void Show(String title,String content ,bool canClose, params Command[] buttons)
        {
            Popup popup = new Popup();
            MessageBox messageBox = new MessageBox(title, content, buttons)
            {
                CanClose = canClose
            };
            popup.Closed += popup_Closed;
            popup.Child = messageBox;
            if (MessageBoxStack.Count == 0)
            {
                popup.IsOpen = true;
            }
            else
            {
                WaitMessageBoxQueue.Enqueue(popup);
            }
            MessageBoxStack.Add(popup);
        }

        static void popup_Closed(object sender, EventArgs e)
        {
            MessageBoxStack.Remove(sender as Popup);
            if(WaitMessageBoxQueue.Count > 0)
            {
                WaitMessageBoxQueue.Dequeue().IsOpen = true;
            }
        }

        public static bool CloseLast()
        {
            if (MessageBoxStack.Count > 0)
            {
                if((MessageBoxStack.Last().Child as MessageBox).CanClose)
                {
                    MessageBoxStack.Last().IsOpen = false;
                }
                return true;
            }
            return false;
        }
    }
}
