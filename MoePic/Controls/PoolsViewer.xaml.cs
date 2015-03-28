using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MoePic.Controls
{
    public partial class PoolsViewer : UserControl
    {
        public PoolsViewer()
        {
            InitializeComponent();
        }public ObservableCollection<MoePic.Models.MoePool> Source = new ObservableCollection<MoePic.Models.MoePool>();

        bool IsLoading = false;

        public void AddPool(MoePic.Models.MoePool pool)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Source.Add(pool);
            });
        }

        public event RequestRefreshEventHandler RequestRefresh;
        public event RequestLoadDataEventHandler RequestLoadData;
        public event FirstLoadEventHandler FirstLoad;

        private async void OnRequestRefresh()
        {
            if (RequestRefresh != null && !IsLoading)
            {
                IsLoading = true;
                LoadingBar.Visibility = System.Windows.Visibility.Visible;
                bool ok = await RequestRefresh(this, new EventArgs());
                LoadingBar.Visibility = System.Windows.Visibility.Collapsed;
                IsLoading = false;
            }
        }

        public void ToTop()
        {
            if (ScrollViewer == null)
            {
                ScrollBar = MoePic.Models.FindElement.FindVisualElement<ScrollBar>(list);
                ScrollViewer = MoePic.Models.FindElement.FindVisualElementFormName(list, "PART_ManipulationContainer") as ScrollViewer;
            }
            ScrollViewer.ScrollToVerticalOffset(0);
        }

        public void Clear()
        {
            Source.Clear();
        }

        private async void OnRequestLoadData()
        {
            if (!IsFirstLoad && RequestLoadData != null && !IsLoading)
            {
                IsLoading = true;
                LoadingBar.Visibility = System.Windows.Visibility.Visible;
                bool ok = await RequestLoadData(this, new EventArgs());
                LoadingBar.Visibility = System.Windows.Visibility.Collapsed;
                IsLoading = false;
            }
        }

        private async void OnFirstLoad()
        {
            if (FirstLoad != null && !IsLoading)
            {
                IsLoading = true;
                LoadingBar.Visibility = System.Windows.Visibility.Visible;
                bool ok = await FirstLoad(this, new EventArgs());
                LoadingBar.Visibility = System.Windows.Visibility.Collapsed;
                IsLoading = false;
            }
        }

        public bool LoadStart()
        {
            if (!IsLoading)
            {
                IsLoading = true;
                LoadingBar.Visibility = System.Windows.Visibility.Visible;
                return true;
            }
            return false;
        }

        public void LoadOver()
        {
            LoadingBar.Visibility = System.Windows.Visibility.Collapsed;
            IsLoading = false;
        }

        bool IsFirstLoad = true;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsFirstLoad)
            {
                IsFirstLoad = false;
                OnFirstLoad();
            }
        }

        ScrollBar ScrollBar;
        public ScrollViewer ScrollViewer;

        private void list_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Source;
        }


        private void list_DataRequested(object sender, EventArgs e)
        {
            OnRequestLoadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ScrollBar.Value == 0)
            {
                OnRequestRefresh();
            }
            else
            {

            }
        }


    }

    public delegate Task<bool> FirstLoadEventHandler(object sender, EventArgs e);
    public delegate Task<bool> RequestRefreshEventHandler(object sender, EventArgs e);
    public delegate Task<bool> RequestLoadDataEventHandler(object sender, EventArgs e);
}
