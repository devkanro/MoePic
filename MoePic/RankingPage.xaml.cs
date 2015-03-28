using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Newtonsoft.Json;
using MoePic.Models;

namespace MoePic
{
    public partial class RankingPage : Models.MoePicPage
    {
        public RankingPage()
        {
            InitializeComponent();
            this.ApplicationBar.BackgroundColor = (App.Current.Resources["ThemeColor"] as ThemeColor).StatusBar.Color;
        }

        bool dayload = false;
        bool weekload = false;
        bool monthload = false;
        bool yearload = false;


        private async void day_FirstLoad()
        {
            if (!dayload)
            {
                day.Clear();
                dayload = true;
                day.LoadStart();
                List<MoePost> postList = await MoebooruAPI.GetDayRankingPost(DayDate.Value);
                foreach (var item in postList)
                {
                    day.AddPost(item);
                }
                day.LoadOver();
            }
        }

        private async void week_FirstLoad()
        {
            if (!weekload)
            {
                week.Clear();
                weekload = true;
                week.LoadStart();
                List<MoePost> postList = await MoebooruAPI.GetWeekRankingPost(WeekDate.Value);
                foreach (var item in postList)
                {
                    week.AddPost(item);
                }
                week.LoadOver();
            }
        }

        private async void month_FirstLoad()
        {
            if (!monthload)
            {
                month.Clear();
                monthload = true;
                month.LoadStart();
                List<MoePost> postList = await MoebooruAPI.GetMonthRankingPost(MonthDate.Value);
                foreach (var item in postList)
                {
                    month.AddPost(item);
                }
                month.LoadOver();
            }
        }


        private async void year_FirstLoad()
        {
            if (!yearload)
            {
                year.Clear();
                yearload = true;
                year.LoadStart();
                List<MoePost> postList = await MoebooruAPI.GetRankingPost(Period.y);
                foreach (var item in postList)
                {
                    year.AddPost(item);
                }
                year.LoadOver();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Controls.StatusBarView view = new Controls.StatusBarView(MoePic.Models.Settings.Current.WebSiteLogo, MoePic.Resources.AppResources.Ranking, false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            if(DayDate == null)
            {
                DayDate = DateTime.Now;
            }
            if (WeekDate == null)
            {
                WeekDate = DateTime.Now;
            }
            if (MonthDate == null)
            {
                MonthDate = DateTime.Now;
            }
            base.OnNavigatedTo(e);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (viewer.SelectedIndex)
            {
                case 0:
                    day.Visibility = System.Windows.Visibility.Visible;
                    week.Visibility = System.Windows.Visibility.Collapsed;
                    month.Visibility = System.Windows.Visibility.Collapsed;
                    year.Visibility = System.Windows.Visibility.Collapsed;
                    this.ApplicationBar.IsVisible = true;
                    day_FirstLoad();
                    break;
                case 1:
                    day.Visibility = System.Windows.Visibility.Collapsed;
                    week.Visibility = System.Windows.Visibility.Visible;
                    month.Visibility = System.Windows.Visibility.Collapsed;
                    year.Visibility = System.Windows.Visibility.Collapsed;
                    this.ApplicationBar.IsVisible = true;
                    week_FirstLoad();
                    break;
                case 2:
                    day.Visibility = System.Windows.Visibility.Collapsed;
                    week.Visibility = System.Windows.Visibility.Collapsed;
                    month.Visibility = System.Windows.Visibility.Visible;
                    year.Visibility = System.Windows.Visibility.Collapsed;
                    this.ApplicationBar.IsVisible = true;
                    month_FirstLoad();
                    break;
                case 3:
                    day.Visibility = System.Windows.Visibility.Collapsed;
                    week.Visibility = System.Windows.Visibility.Collapsed;
                    month.Visibility = System.Windows.Visibility.Collapsed;
                    year.Visibility = System.Windows.Visibility.Visible;
                    this.ApplicationBar.IsVisible = false;
                    year_FirstLoad();
                    break;
                default:
                    break;
            }
        }

        public DateTime? DayDate { get; set; }
        public DateTime? WeekDate { get; set; }
        public DateTime? MonthDate { get; set; }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            DatePickerPage.DateSelected += DatePickerPage_DateSelected;
            switch (viewer.SelectedIndex)
            {
                case 0:
                    DatePickerPage.Show(DayDate.Value);
                    break;
                case 1:
                    DatePickerPage.Show(WeekDate.Value);
                    break;
                case 2:
                    DatePickerPage.Show(MonthDate.Value);
                    break;
            }
        }

        void DatePickerPage_DateSelected(object sender, EventArgs e)
        {
            if(DatePickerPage.IsCancel != true)
            {
                switch (viewer.SelectedIndex)
                {
                    case 0:
                        DayDate = DatePickerPage.Date;
                        dayload = false;
                        day_FirstLoad();
                        break;
                    case 1:
                        WeekDate = DatePickerPage.Date;
                        weekload = false;
                        week_FirstLoad();
                        break;
                    case 2:
                        MonthDate = DatePickerPage.Date;
                        monthload = false;
                        month_FirstLoad();
                        break;
                }
            }
        }

    }
}