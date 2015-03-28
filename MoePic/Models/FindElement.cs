using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;

namespace MoePic.Models
{
    public class FindElement
    {
        public static T FindVisualElement<T>(DependencyObject container) where T : DependencyObject
        {
            var childQueue = new Queue<DependencyObject>();

            childQueue.Enqueue(container);

            while (childQueue.Count > 0)
            {
                var current = childQueue.Dequeue();
                T result = current as T;
                if (result != null && result != container)
                {
                    return result;
                }

                int childCount = VisualTreeHelper.GetChildrenCount(current);

                for (int childIndex = 0; childIndex < childCount; childIndex++)
                {
                    childQueue.Enqueue(VisualTreeHelper.GetChild(current, childIndex));
                }
            }

            return null;
        }

        public static FrameworkElement FindVisualElementFormName(FrameworkElement container, String Name)
        {
            var childQueue = new Queue<FrameworkElement>();

            childQueue.Enqueue(container);

            while (childQueue.Count > 0)
            {
                var current = childQueue.Dequeue();
                FrameworkElement result = current;
                if (result != null && result != container && result.Name == Name)
                {
                    return result;
                }

                int childCount = VisualTreeHelper.GetChildrenCount(current);

                for (int childIndex = 0; childIndex < childCount; childIndex++)
                {
                    childQueue.Enqueue(VisualTreeHelper.GetChild(current, childIndex) as FrameworkElement);
                }
            }

            return null;
        }

        public static List<T> FindVisualElements<T>(DependencyObject container) where T : DependencyObject
        {
            var childQueue = new Queue<DependencyObject>();
            List<T> list = new List<T>();

            childQueue.Enqueue(container);

            while (childQueue.Count > 0)
            {
                var current = childQueue.Dequeue();
                T result = current as T;
                if (result != null && result != container)
                {
                    list.Add(result);
                }

                int childCount = VisualTreeHelper.GetChildrenCount(current);

                for (int childIndex = 0; childIndex < childCount; childIndex++)
                {
                    childQueue.Enqueue(VisualTreeHelper.GetChild(current, childIndex));
                }
            }

            return list;
        }

        
    }
}
